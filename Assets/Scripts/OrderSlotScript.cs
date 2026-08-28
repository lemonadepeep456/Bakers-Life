using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OrderSlotScript : MonoBehaviour
{
    public Image customerImage;
    public Image foodImage;
    public TMP_Text foodName;

    public Customer customer;

    public void SetOrder(Customer newCustomer)
    {
        if (newCustomer == null)
            return;

        if (newCustomer.requestedItem == null)
            return;

        customer = newCustomer;

        if (customerImage != null)
            customerImage.sprite = newCustomer.spriteRenderer.sprite;

        if (foodImage != null)
            foodImage.sprite = newCustomer.requestedItem.itemSprite;

        if (foodName != null)
            foodName.text = newCustomer.requestedItem.itemName;

        gameObject.SetActive(true);
    }

    public void ClearOrder()
    {
        customer = null;

        if (customerImage != null)
            customerImage.sprite = null;

        if (foodImage != null)
            foodImage.sprite = null;

        if (foodName != null)
            foodName.text = "";

        gameObject.SetActive(false);
    }
}