using System.Collections;
using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEngine;
using static UnityEditor.Progress;

public class WaitressAI : MonoBehaviour
{
    public float moveSpeed = 5f;

    public Transform restLocation;
    public Transform holdPoint;

    private Transform targetPickup;
    private Transform targetDelivery;

    private GameObject foodObject;
    private ItemSO foodItem;
    public Animator animator;
    private bool isBusy = false;

    public void StartDeliveryRoute(
    Transform pickup,
    Transform delivery,
    GameObject food,
    ItemSO item
)
    {
        if (isBusy)
        {
            Debug.Log("Waitress is already busy.");
            return;
        }

        if (food == null)
        {
            Debug.LogError(
                "StartDeliveryRoute received NULL food!"
            );

            return;
        }

        if (item == null)
        {
            Debug.LogError(
                "StartDeliveryRoute received NULL ItemSO!"
            );

            return;
        }

        targetPickup = pickup;
        targetDelivery = delivery;

        // THIS IS THE IMPORTANT PART
        foodObject = food;
        foodItem = item;

        Debug.Log(
            "Waitress received food object: " +
            foodObject.name
        );

        StartCoroutine(DeliveryRoutine());
    }

    private IEnumerator DeliveryRoutine()
    {
        isBusy = true;
        animator.Play("Walk");
        Debug.Log("Waitress going to counter.");

        yield return StartCoroutine(
            MoveTo(targetPickup.position)
        );

        Debug.Log("Waitress reached counter.");

        PickUpFood();

        Debug.Log(
            "Waitress going to customer."
        );

        yield return StartCoroutine(
            MoveTo(targetDelivery.position)
        );

        Debug.Log("Waitress reached customer.");

        DeliverFood();

        if (restLocation != null)
        {
            Debug.Log("Waitress returning to rest.");

            yield return StartCoroutine(
                MoveTo(restLocation.position)

            );
            animator.Play("Idle");
        }

        isBusy = false;
    }

    private void PickUpFood()
    {
        animator.Play("Walk");
        if (foodObject == null)
        {
            Debug.LogError(
                "WAITRESS PICKUP FAILED: foodObject is NULL."
            );

            return;
        }

        if (foodItem == null)
        {
            Debug.LogError(
                "WAITRESS PICKUP FAILED: foodItem is NULL."
            );

            return;
        }

        if (holdPoint == null)
        {
            Debug.LogError(
                "WAITRESS PICKUP FAILED: HoldPoint is not assigned."
            );

            return;
        }

        Debug.Log(
            "Waitress picking up: " +
            foodObject.name
        );

        foodObject.transform.SetParent(holdPoint);

        foodObject.transform.localPosition = Vector3.zero;
        foodObject.transform.localRotation = Quaternion.identity;

        Collider2D collider =
            foodObject.GetComponent<Collider2D>();

        if (collider != null)
        {
            collider.enabled = false;
        }

        Rigidbody2D rb =
            foodObject.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.simulated = false;
        }

        Debug.Log(
            "WAITRESS IS NOW HOLDING: " +
            foodItem.itemName
        );
    }

    private void DeliverFood()
    {
        animator.Play("Walk");
        if (foodObject == null)
        {
            Debug.LogError(
                "Waitress has no food to deliver."
            );

            return;
        }

        if (foodItem == null)
        {
            Debug.LogError(
                "Waitress has no ItemSO."
            );

            return;
        }

        if (targetDelivery == null)
        {
            Debug.LogError(
                "Waitress has no customer target."

            );
            Destroy(foodObject);

            foodObject = null;
            foodItem = null;

            return;
        }

        Customer customer =
            targetDelivery.GetComponent<Customer>();

        if (customer == null)
        {
            Debug.LogError(
                "The delivery target does not have a Customer component."
            );

            return;
        }

        Debug.Log(
            "Waitress delivering " +
            foodItem.itemName
        );

        bool successful =
            customer.ServeCustomer(foodItem);

        if (successful)
        {
            Debug.Log(
                "ORDER COMPLETED: " +
                foodItem.itemName
            );

            Destroy(foodObject);

            foodObject = null;
            foodItem = null;
        }
        else
        {
            Debug.Log(
                "ORDER FAILED: Wrong item."
            );
        }
    }

    private IEnumerator MoveTo(Vector3 destination)
    {
        while (
            Vector3.Distance(
                transform.position,
                destination
            ) > 0.1f
        )
        {
            transform.position =
                Vector3.MoveTowards(
                    transform.position,
                    destination,
                    moveSpeed * Time.deltaTime
                );

            yield return null;
        }
    }
}