using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PickupStationScript : MonoBehaviour
{
    //=========================================================
    // RECIPE SETTINGS
    //=========================================================
    //
    // Every station has its own list of recipes.
    //
    // Example:
    //
    // Burger Grill
    // ------------------------
    // Raw Burger -> Burger
    // Raw Steak -> Steak
    //
    // Fryer
    // ------------------------
    // Raw Fries -> Fries
    // Raw Onion Rings -> Onion Rings
    //
    // The station searches this list whenever the player
    // places an item onto it.
    [Header("Cooking Recipes")]
    public CookingRecipe[] recipes;
    //=========================================================
    // CURRENT COOKING INFORMATION
    //=========================================================

    // Stores the raw item that the player placed
    // onto this station.
    //
    // Example:
    //
    // Player places Raw Burger
    //
    // currentInputItem = Raw Burger
    //
    // This becomes null again after the player
    // picks up the cooked food.
    private ItemSO currentInputItem;


    // Stores the recipe currently being used.
    //
    // Example:
    //
    // Input:
    // Raw Burger
    //
    // Output:
    // Burger
    //
    // Cook Time:
    // 5 seconds
    //
    // Once cooking finishes, this gets reset back
    // to null so the station is ready for another item.
    private CookingRecipe currentRecipe;

    [Header("Cooking")]

    // How many seconds this station takes to cook.
    // This value is only used if the ItemSO requires cooking.
  

    // Progress bar displayed while food cooks.
    public Slider progressBar;

    // True while the station is actively cooking.
    private bool isCooking;

    // True once the food has finished cooking and is waiting
    // for the player to pick it up.
    private bool isReady;

    //=========================================================
    // COOKABLE ITEMS
    //=========================================================


    //=========================================================
    // PLAYER REFERENCES
    //=========================================================

    // Stores the player's holding script while they are
    // standing inside the trigger.
    private PlayerHoldingScript player;

    // Determines if the player is close enough to interact.
    private bool playerInRange;
    // Where the cooked food will appear.
    //=========================================================
    // STATION HOLD POINT
    //=========================================================

    // The HoldPoint is an empty GameObject that sits on top
    // of the station. It acts as an "anchor" for the finished
    // food so the burger (or any other item) appears exactly
    // where you want it.
    //
    // Example Hierarchy:
    //
    // Burger Station
    //  Sprite
    // HoldPoint   <-- Empty GameObject
    //  Canvas
    // PickupStationScript
    //
    // Because the food becomes a child of this HoldPoint,
    // if the station moves, the food will move with it.
    public Transform holdPoint;


    //=========================================================
    // COOKED FOOD OBJECT
    //=========================================================

    // This stores the ACTUAL GameObject that is currently
    // sitting on top of the station.
    //
    // This is NOT an ItemSO.
    //
    // ItemSO = Data
    // GameObject = Physical object in the scene.
    //
    // We save this GameObject reference so we can destroy
    // it later when the player picks the food up.
    private GameObject cookedFoodObject;

    private void Awake()
    {
        // Automatically finds the Slider attached to one
        // of this station's child objects.
        progressBar = GetComponentInChildren<Slider>();
    }

    private void Start()
    {
        // Hide the progress bar until cooking starts.
        progressBar.gameObject.SetActive(false);
    }

    //=========================================================
    // PLAYER ENTERS STATION
    //=========================================================

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Ignore anything that isn't the player.
        if (!other.CompareTag("Player"))
            return;

        // Save the player's holding script.
        player = other.GetComponent<PlayerHoldingScript>();

        // Allow interaction.
        playerInRange = true;

        Debug.Log("Player entered station");
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

        Debug.Log("Player left station");
    }

    //=========================================================
    // MAIN STATION LOGIC
    //=========================================================

    //=========================================================
    // PICKUP STATION SYSTEM
    //=========================================================
    //
    // This script controls a cooking station.
    //
    // The station has five main jobs:
    //
    // 1. Detect when the player walks into range.
    //
    // 2. Decide whether the assigned ItemSO
    //    requires cooking.
    //
    // 3. If cooking is required,
    //    display a progress bar while waiting.
    //
    // 4. When cooking finishes,
    //    spawn the finished food on the station's HoldPoint.
    //
    // 5. When the player presses E again,
    //    remove the food from the station and
    //    place it into the player's hands.
    //
    // This system keeps the ItemSO (data) separate from
    // the actual GameObject that appears in the world.
    //
    // ItemSO = Information
    // (Name, Sprite, Price, Prefab, Requires Cooking)
    //
    // GameObject = Physical object
    // (The burger that appears on the grill.)
    //=========================================================

    //=========================================================
    // FIND RECIPE
    //=========================================================
    //
    // Searches through every recipe assigned to this station.
    //
    // If a recipe's input item matches the item the player
    // placed onto the station, return that recipe.
    //
    // Otherwise return null.
    //
    // Returning null simply means:
    //
    // "This station doesn't know how to cook that item."
    private void Update()
    {
        //-----------------------------------------------------
        // PLAYER CHECK
        //-----------------------------------------------------

        // If the player isn't standing inside this station,
        // don't allow any interaction.
        if (!playerInRange || player == null)
            return;

        //-----------------------------------------------------
        // KEY PRESS
        //-----------------------------------------------------

        // Only continue once when E is pressed.
        if (!Input.GetKeyDown(KeyCode.E))
            return;

        //-----------------------------------------------------
        // PLACE AN ITEM ON THE STATION
        //-----------------------------------------------------

        // If the player is holding something and the station
        // currently has nothing on it...
        if (player.IsHoldingItem() && currentInputItem == null)
        {
            // Store the item that the player was holding.
            currentInputItem = player.heldItem;

            Debug.Log("Placed " + currentInputItem.itemName + " on station.");

            // Remove the item from the player's hands.
            player.ClearItem();

            //-------------------------------------------------
            // FIND A RECIPE
            //-------------------------------------------------

            currentRecipe = FindRecipe(currentInputItem);

            // If this station doesn't know how to cook
            // the item, give it back immediately.
            if (currentRecipe == null)
            {
                Debug.Log("This station cannot cook that item.");

                player.HoldItem(
                    currentInputItem,
                    currentInputItem.worldPrefab
                );

                currentInputItem = null;

                return;
            }

            //-------------------------------------------------
            // START COOKING
            //-------------------------------------------------

            StartCoroutine(CookFood());

            return;
        }

        //-----------------------------------------------------
        // PICK UP FINISHED FOOD
        //-----------------------------------------------------

        // Cooking finished.
        //
        //-----------------------------------------------------
        // PLAYER PICKS UP THE COOKED FOOD
        //-----------------------------------------------------

        if (isReady)
        {
            // Give the cooked ItemSO to the player.
            player.HoldItem(
                currentRecipe.outputItem,
                currentRecipe.outputItem.worldPrefab
            );

            //-------------------------------------------------
            // REMOVE FOOD FROM STATION
            //-------------------------------------------------

            if (cookedFoodObject != null)
            {
                Destroy(cookedFoodObject);
                cookedFoodObject = null;
            }

            //-------------------------------------------------
            // RESET THE STATION
            //-------------------------------------------------

            ResetStation();
            Debug.Log("Player picked up cooked food.");
        }
    }
    private CookingRecipe FindRecipe(ItemSO inputItem)
    {
        // Check every recipe assigned to this station.
        foreach (CookingRecipe recipe in recipes)
        {
            // Is this the recipe we're looking for?
            if (recipe.inputItem == inputItem)
            {
                Debug.Log("Recipe found for " + inputItem.itemName);

                return recipe;
            }
        }

        Debug.Log("No recipe found for " + inputItem.itemName);

        return null;
    }
    //=========================================================
    // COOKING COROUTINE
    //=========================================================

    // Coroutines allow code to pause without freezing the game.
    //
    // Instead of instantly finishing,
    // the station slowly counts up until the timer ends.
    //=========================================================
    // COOK FOOD
    //=========================================================
    //
    // Coroutines allow code to run over time without freezing
    // the rest of the game.
    //
    // Once the player places a valid item onto the station,
    // this coroutine begins counting up until the recipe's
    // cook time has been reached.
    //
    // During cooking:
    //
    // Raw Burger
    //      |
    //      V
    // Progress Bar
    //      |
    //      V
    // Burger appears on station
    //
    //=========================================================
    IEnumerator CookFood()
    {
        //-----------------------------------------------------
        // STATION STATE
        //-----------------------------------------------------

        // Tell the station that it is busy cooking.
        isCooking = true;

        // The food isn't ready yet.
        isReady = false;

        //-----------------------------------------------------
        // SHOW THE PROGRESS BAR
        //-----------------------------------------------------

        progressBar.gameObject.SetActive(true);

        // Empty the slider.
        progressBar.value = 0;

        float timer = 0;

        //-----------------------------------------------------
        // COOKING TIMER
        //-----------------------------------------------------

        // Continue cooking until enough time has passed.
        while (timer < currentRecipe.cookTime)
        {
            timer += Time.deltaTime;

            // Update the slider.
            progressBar.value = timer / currentRecipe.cookTime;

            yield return null;
        }

        //-----------------------------------------------------
        // COOKING COMPLETE
        //-----------------------------------------------------

        progressBar.value = 1;

        progressBar.gameObject.SetActive(false);

        isCooking = false;

        isReady = true;

        //-----------------------------------------------------
        // SPAWN THE COOKED FOOD
        //-----------------------------------------------------

        // Only create the cooked food if there isn't already
        // one sitting on the station.
        if (cookedFoodObject == null)
        {
            cookedFoodObject = Instantiate(
                currentRecipe.outputItem.worldPrefab,
                holdPoint.position,
                Quaternion.identity,
                holdPoint
            );
        }

        Debug.Log(currentRecipe.outputItem.itemName + " finished cooking!");
    }
    //=========================================================
    // COOKING VALIDATION
    //=========================================================

    // This function checks if THIS station is allowed to cook
    // the ItemSO that has been assigned to it.
    //
    // Example:
    //
    // Grill Station
    // Cookable Items:
    // - Burger
    // - Cheeseburger
    //
    // Fryer
    // Cookable Items:
    // - Fries
    // - Onion Rings
    //
    // If the assigned ItemSO exists inside the cookableItems array,
    // this function returns TRUE.
    //
    // Otherwise it returns FALSE, meaning this station cannot
    // cook that item.
    //=========================================================
    // FIND RECIPE
    //=========================================================
    //
    // Searches through every recipe assigned to this station.
    //
    // If a recipe's input item matches the item the player
    // placed onto the station, return that recipe.
    //
    // Otherwise return null.
    //
    // Returning null simply means:
    //
    // "This station doesn't know how to cook that item."
//=================================
    // RESET STATION
    //=========================================================
    //
    // Returns the station back to its default state.
    //
    // This allows another item to be placed on it.
    //
    // This function is useful because if you ever
    // want to cancel cooking, burn food, or clean
    // the station, you only need to call:
    //
    // ResetStation();
    //
    // instead of resetting every variable yourself.
    //=========================================================
    private void ResetStation()
    {
        currentInputItem = null;

        currentRecipe = null;

        isCooking = false;

        isReady = false;

        if (cookedFoodObject != null)
        {
            Destroy(cookedFoodObject);

            cookedFoodObject = null;
        }

        progressBar.value = 0;

        progressBar.gameObject.SetActive(false);
    }
}