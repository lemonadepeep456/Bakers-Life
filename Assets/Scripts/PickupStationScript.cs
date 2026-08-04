using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PickupStationScript : MonoBehaviour
{
    public ItemSO item;

    [Header("Cooking")]
    public float cookTime = 5f;
    public Slider progressBar;
    private bool isCooking;
    private bool isReady;

    private PlayerHoldingScript player;
    private bool playerInRange;
    private void Start()
    {
        progressBar.gameObject.SetActive(false);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        player = other.GetComponent<PlayerHoldingScript>();
        playerInRange = true;

        Debug.Log("Player entered station");
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInRange = false;
        player = null;

        Debug.Log("Player left station");
    }

    private void Update()
    {
        if (!playerInRange || player == null)
            return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            // Player is already holding something
            if (player.IsHoldingItem())
                return;

            // Start cooking if the station is idle
            if (!isCooking && !isReady)
            {
                StartCoroutine(CookFood());
                return;
            }

            // Food is finished, give it to the player
            if (isReady)
            {
                player.HoldItem(item, item.worldPrefab);

                isReady = false;

                Debug.Log(item.itemName + " picked up!");
            }
        }
    }
    IEnumerator CookFood()
    {
        isCooking = true;
        isReady = false;

        progressBar.gameObject.SetActive(true);
        progressBar.value = 0;

        float timer = 0;

        while (timer < cookTime)
        {
            timer += Time.deltaTime;

            progressBar.value = timer / cookTime;

            yield return null;
        }

        progressBar.value = 1;

        isCooking = false;
        isReady = true;

        progressBar.gameObject.SetActive(false);

        Debug.Log(item.itemName + " is ready!");
    }
    private void Awake()
    {
        progressBar = GetComponentInChildren<Slider>();
    }
}