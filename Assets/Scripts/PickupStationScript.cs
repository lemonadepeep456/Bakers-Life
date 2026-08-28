using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PickupStationScript : MonoBehaviour
{
    [Header("Cooking Recipes")]
    public CookingRecipe[] recipes;

    private ItemSO currentInputItem;
    private CookingRecipe currentRecipe;

    [Header("Cooking")]
    public Slider progressBar;

    private bool isCooking;
    private bool isReady;

    private PlayerHoldingScript player;

    // Determines if the player is close enough to interact.
    private bool playerInRange;

    [Header("Finished Food Hold Points")]
    public Transform[] holdPoint;

    // Keeps track of which hold points are occupied.
    private bool[] holdPointOccupied;

    // Stores the actual cooked food GameObject at each point.
    private GameObject[] cookedFoodObject;

    // Stores the ItemSO belonging to each cooked food.
    private ItemSO[] cookedFoodItems;


    //=========================================================
    // AWAKE
    //=========================================================

    private void Awake()
    {
        // Automatically finds the Slider attached to this
        // station or one of its children.
        progressBar = GetComponentInChildren<Slider>();
    }


    //=========================================================
    // START
    //=========================================================

    private void Start()
    {
        if (progressBar != null)
        {
            progressBar.gameObject.SetActive(false);
        }

        // Create arrays based on how many hold points
        // were assigned in the Inspector.
        holdPointOccupied = new bool[holdPoint.Length];

        cookedFoodObject = new GameObject[holdPoint.Length];

        cookedFoodItems = new ItemSO[holdPoint.Length];
    }


    //=========================================================
    // FIND FREE HOLD POINT
    //=========================================================

    int FindFreeHoldPoint()
    {
        for (int i = 0; i < holdPoint.Length; i++)
        {
            if (!holdPointOccupied[i])
            {
                return i;
            }
        }

        // -1 means every hold point is occupied.
        return -1;
    }


    //=========================================================
    // FIND OCCUPIED HOLD POINT
    //=========================================================

    int FindOccupiedHoldPoint()
    {
        for (int i = 0; i < holdPointOccupied.Length; i++)
        {
            if (holdPointOccupied[i])
            {
                return i;
            }
        }

        return -1;
    }


    //=========================================================
    // REMOVE BURGER
    //=========================================================

    public void RemoveBurger(HeldBurger burger)
    {
        if (burger == null)
            return;

        int index = burger.holdPointIndex;

        if (index < 0 || index >= holdPoint.Length)
            return;

        holdPointOccupied[index] = false;

        cookedFoodObject[index] = null;

        cookedFoodItems[index] = null;

        Debug.Log("Hold point " + index + " is now free.");
    }


    //=========================================================
    // PLAYER ENTERS STATION
    //=========================================================

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        player = other.GetComponent<PlayerHoldingScript>();

        if (player == null)
        {
            Debug.LogError(
                "Player does not have a PlayerHoldingScript!"
            );

            return;
        }

        playerInRange = true;

        Debug.Log("Player entered station.");
    }


    //=========================================================
    // PLAYER LEAVES STATION
    //=========================================================

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInRange = false;
        player = null;

        Debug.Log("Player left station.");
    }


    //=========================================================
    // UPDATE
    //=========================================================

    private void Update()
    {
        if (!playerInRange || player == null)
            return;

        // Only run the interaction once per E press.
        if (!Input.GetKeyDown(KeyCode.E))
            return;


        //=====================================================
        // PLAYER IS HOLDING SOMETHING
        // START COOKING
        //=====================================================

        if (player.IsHoldingItem())
        {
            // Don't allow another item to be placed while
            // this station is currently cooking.
            if (isCooking)
            {
                Debug.Log("Station is currently cooking.");
                return;
            }


            // Check if there is somewhere for the finished
            // food to go.
            int freePoint = FindFreeHoldPoint();

            if (freePoint == -1)
            {
                Debug.Log("All hold points are full!");
                return;
            }


            // Store the ItemSO that the player is putting down.
            currentInputItem = player.heldItem;

            Debug.Log(
                "Placed " +
                currentInputItem.itemName +
                " on station."
            );


            // Remove the item from the player's hands.
            player.ClearItem();


            // Find the recipe for this ItemSO.
            currentRecipe = FindRecipe(currentInputItem);


            // Station cannot cook this item.
            if (currentRecipe == null)
            {
                Debug.Log(
                    "This station cannot cook " +
                    currentInputItem.itemName
                );


                // Give the item back to the player.
                player.HoldItem(
                    currentInputItem,
                    currentInputItem.worldPrefab
                );


                currentInputItem = null;

                return;
            }


            // Start cooking.
            StartCoroutine(CookFood());

            return;
        }


        //=====================================================
        // PLAYER IS NOT HOLDING ANYTHING
        //  PICK UP FINISHED FOOD
        //=====================================================

        int occupiedPoint = FindOccupiedHoldPoint();

        if (occupiedPoint == -1)
        {
            Debug.Log("There is no cooked food to pick up.");
            return;
        }


        // Get the ItemSO belonging specifically to this
        // hold point.
        ItemSO foodItem = cookedFoodItems[occupiedPoint];

        if (foodItem == null)
        {
            Debug.LogError(
                "Cooked food ItemSO is missing at hold point " +
                occupiedPoint
            );

            return;
        }


        // Give the correct ItemSO to the player.
        player.HoldItem(
            foodItem,
            foodItem.worldPrefab
        );


        // Destroy the physical cooked food object.
        if (cookedFoodObject[occupiedPoint] != null)
        {
            Destroy(cookedFoodObject[occupiedPoint]);

            cookedFoodObject[occupiedPoint] = null;
        }


        // Free this specific hold point.
        holdPointOccupied[occupiedPoint] = false;

        cookedFoodItems[occupiedPoint] = null;


        Debug.Log(
            "Player picked up " +
            foodItem.itemName +
            " from hold point " +
            occupiedPoint
        );


        // Reset only the cooking state.
        ResetStation();
    }


    //=========================================================
    // FIND RECIPE
    //=========================================================

    private CookingRecipe FindRecipe(ItemSO inputItem)
    {
        foreach (CookingRecipe recipe in recipes)
        {
            if (recipe.inputItem == inputItem)
            {
                Debug.Log(
                    "Recipe found for " +
                    inputItem.itemName
                );

                return recipe;
            }
        }


        Debug.Log(
            "No recipe found for " +
            inputItem.itemName
        );

        return null;
    }


    //=========================================================
    // COOK FOOD
    //=========================================================

    private IEnumerator CookFood()
    {
        isCooking = true;
        isReady = false;

        progressBar.gameObject.SetActive(true);

        progressBar.value = 0;


        float cookTime = currentRecipe.cookTime;

        float timer = 0f;


        //=====================================================
        // COOKING
        //=====================================================

        while (timer < cookTime)
        {
            timer += Time.deltaTime;

            progressBar.value = timer / cookTime;

            yield return null;
        }


        //=====================================================
        // COOKING COMPLETE
        //=====================================================

        progressBar.value = 1;

        progressBar.gameObject.SetActive(false);

        isCooking = false;
        isReady = false;


        //=====================================================
        // FIND FREE HOLD POINT
        //=====================================================

        int freePoint = FindFreeHoldPoint();


        if (freePoint == -1)
        {
            Debug.Log("All hold points are full!");

            currentInputItem = null;
            currentRecipe = null;

            yield break;
        }


        //=====================================================
        // STORE ITEMSO
        //=====================================================

        cookedFoodItems[freePoint] =
            currentRecipe.outputItem;


        //=====================================================
        // SPAWN COOKED FOOD
        //=====================================================

        cookedFoodObject[freePoint] = Instantiate(
            currentRecipe.outputItem.worldPrefab,
            holdPoint[freePoint].position,
            Quaternion.identity,
            holdPoint[freePoint]
        );


        //=====================================================
        // SET HELD BURGER DATA
        //=====================================================

        HeldBurger heldBurger =
            cookedFoodObject[freePoint]
            .GetComponent<HeldBurger>();


        if (heldBurger != null)
        {
            heldBurger.holdPointIndex = freePoint;

            heldBurger.station = this;
        }


        //=====================================================
        // MARK HOLD POINT AS OCCUPIED
        //=====================================================

        holdPointOccupied[freePoint] = true;


        Debug.Log(
            currentRecipe.outputItem.itemName +
            " finished cooking at hold point " +
            freePoint
        );


        //=====================================================
        // CLEAR CURRENT COOKING DATA
        //=====================================================

        currentInputItem = null;

        currentRecipe = null;
    }


    //=========================================================
    // RESET STATION
    //=========================================================

    private void ResetStation()
    {
        currentInputItem = null;

        currentRecipe = null;

        isCooking = false;

        isReady = false;


        if (progressBar != null)
        {
            progressBar.value = 0;

            progressBar.gameObject.SetActive(false);
        }
    }
}