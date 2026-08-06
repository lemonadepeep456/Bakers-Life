using UnityEngine;

public class ItemPickupStation : MonoBehaviour
{
    //=========================================================
    // ITEM PICKUP STATION
    //=========================================================
    //
    // This station gives the player a specific ItemSO whenever
    // they press E while standing inside the station's trigger.
    //
    // Example:
    //
    // Bun Station
    // Gives:
    // Bun
    //
    // Soda Machine
    // Gives:
    // Soda
    //
    //=========================================================

    [Header("Item To Give")]

    // The ItemSO this station gives to the player.
    public ItemSO itemToGive;

    // Stores the player while they're inside the trigger.
    private PlayerHoldingScript player;

    // Tracks if the player is in range.
    private bool playerInRange;

    //=========================================================
    // PLAYER ENTERS STATION
    //=========================================================

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        player = other.GetComponent<PlayerHoldingScript>();
        playerInRange = true;
    }

    //=========================================================
    // PLAYER LEAVES STATION
    //=========================================================

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        player = null;
        playerInRange = false;
    }

    //=========================================================
    // PLAYER INTERACTION
    //=========================================================

    private void Update()
    {
        // Player isn't close enough.
        if (!playerInRange || player == null)
            return;

        // Wait until E is pressed.
        if (!Input.GetKeyDown(KeyCode.E))
            return;

        // Don't give another item if the player
        // is already holding something.
        if (player.IsHoldingItem())
        {
            Debug.Log("Player is already holding an item.");
            return;
        }

        // Safety check.
        if (itemToGive == null)
        {
            Debug.LogWarning("No ItemSO assigned!");
            return;
        }

        // Give the item to the player.
        player.HoldItem(itemToGive, itemToGive.worldPrefab);

        Debug.Log("Player picked up " + itemToGive.itemName);
    }
}