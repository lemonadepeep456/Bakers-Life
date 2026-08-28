using NUnit.Framework.Internal.Execution;
using UnityEngine;
using static UnityEditor.Progress;


public class DeliveryCounter : MonoBehaviour
{
    private PlayerHoldingScript player;
    private bool playerInRange;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        player = other.GetComponent<PlayerHoldingScript>();
        playerInRange = true;

        Debug.Log("Player entered delivery counter.");
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInRange = false;
        player = null;
    }

    private void Update()
    {
        if (!playerInRange || player == null)
            return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            GiveItemToWaitress();
        }
    }

    private void GiveItemToWaitress()
    {
        if (player.heldItem == null)
        {
            Debug.Log("Player is not holding an ItemSO.");
            return;
        }

        if (player.heldObject == null)
        {
            Debug.LogError(
                "Player has an ItemSO but heldObject is NULL."
            );

            return;
        }

        if (OrderManagerScript.Instance == null)
        {
            Debug.LogError(
                "OrderManagerScript.Instance is NULL."
            );

            return;
        }

        WaitressAI waitress =
            OrderManagerScript.Instance.waitress;

        if (waitress == null)
        {
            Debug.LogError(
                "No WaitressAI is assigned to OrderManager."
            );

            return;
        }

        ItemSO item = player.heldItem;

        Customer customer =
            OrderManagerScript.Instance.FindMatchingCustomer(item);

        if (customer == null)
        {
            Debug.Log(
                "Nobody ordered " + item.itemName
            );

            return;
        }

        // Save the actual physical food object.
        GameObject foodObject = player.heldObject;

        if (foodObject == null)
        {
            Debug.LogError(
                "Food object disappeared before delivery."
            );

            return;
        }

        Debug.Log(
            "Giving food object " +
            foodObject.name +
            " to waitress."
        );

        // Remove the reference from the player's hands.
        // This does NOT destroy the food.
        player.RemoveHeldItem();

        // Give the exact same GameObject to the waitress.
        waitress.StartDeliveryRoute(
            transform,
            customer.transform,
            foodObject,
            item
        );

        Debug.Log(
            "WAITRESS RECEIVED: " +
            foodObject.name
        );
    }
}