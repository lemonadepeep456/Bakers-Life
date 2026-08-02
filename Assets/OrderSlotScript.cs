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
        customer = newCustomer;

        customerImage.sprite = newCustomer.spriteRenderer.sprite;
        foodImage.sprite = newCustomer.requestedItem.itemSprite;
        foodName.text = newCustomer.requestedItem.itemName;

        gameObject.SetActive(true);
    }

    public void ClearOrder()
    {
        customer = null;

        customerImage.sprite = null;
        foodImage.sprite = null;
        foodName.text = "";

        gameObject.SetActive(false);
    }
}