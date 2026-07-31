using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class Customer : MonoBehaviour
{
    public ItemSO requestedItem;
    public ItemSO[] menuItems;
    public int spawnIndex;
    public float waitTime = 20f;

    private bool served = false;

    public TMP_Text orderText;
    public Image orderIcon;
    public Sprite[] customerSprites; //Array to put in customer sprites
    public SpriteRenderer spriteRenderer;
    void Start()
    {
        // Pick a random menu item
        requestedItem = menuItems[Random.Range(0, menuItems.Length)];

        // Display the item's name
        if (orderText != null)
            orderText.text = requestedItem.itemName;

        // Display the item's icon
        if (orderIcon != null)
            orderIcon.sprite = requestedItem.itemSprite;

        Debug.Log("Customer wants " + requestedItem.itemName);
        int randomSprite = Random.Range(0, customerSprites.Length); //Customer Sprite randomizer

        spriteRenderer.sprite = customerSprites[randomSprite];
        Invoke(nameof(CustomerLeaves), waitTime);
    }

    public void ServeCustomer(ItemSO item)
    {
        if (served) return;

        if (item == requestedItem)
        {
            served = true;

            FindObjectOfType<CustomerSpawner>().ClearSpot(spawnIndex);

            Debug.Log("Correct order!");

            Destroy(gameObject); // eeewewewew
        }
    }

    void CustomerLeaves()
    {
        if (served) return;

        Debug.Log("Customer got tired and left.");

        FindObjectOfType<CustomerSpawner>().ClearSpot(spawnIndex);

        Destroy(gameObject);
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            PlayerHoldingScript player = other.GetComponent<PlayerHoldingScript>();

            if (player.heldItem == null)
                return;

            ServeCustomer(player.heldItem);

            if (served)
            {
                player.ClearItem();
            }
        }
    }
}