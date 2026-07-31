using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupStationScript : MonoBehaviour
{
    public ItemSO item;

    private PlayerHoldingScript player;
    private bool playerInRange;

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
            Debug.Log("E pressed!");

            if (!player.IsHoldingItem())
            {
                player.HoldItem(item, item.worldPrefab);
            }
        }
    }
}