using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


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
    private PlayerHoldingScript player;
    private bool playerInRange;

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
        Debug.Log("Assigned sprite: " + spriteRenderer.sprite.name);
        Invoke(nameof(CustomerLeaves), waitTime);
        OrderManagerScript.Instance.AddCustomer(this); //Adds customer to the order list
    }


    public bool ServeCustomer(ItemSO item)
    {
        if (served) return false;

        if (item == requestedItem)
        {
            served = true;
            FindObjectOfType<CustomerSpawner>().ClearSpot(spawnIndex);
            MoneyManager.Instance.AddMoney(requestedItem.price);
            Debug.Log("Correct order!");
            OrderManagerScript.Instance.RemoveCustomer(this); //Removes a customer within the order list
            return true;

        }

        return false;
    }
    void CustomerLeaves()
    {
        if (served) return;

        Debug.Log("Customer got tired and left.");
        OrderManagerScript.Instance.RemoveCustomer(this);

        FindObjectOfType<CustomerSpawner>().ClearSpot(spawnIndex);

        Destroy(gameObject);
       
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        player = other.GetComponent<PlayerHoldingScript>();
        playerInRange = true;
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
            if (player.heldItem == null)
                return;

            if (ServeCustomer(player.heldItem))
            {
                player.ClearItem();
                Destroy(gameObject);
            }
        }
    }
}