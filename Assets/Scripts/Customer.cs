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
        requestedItem = menuItems[
            Random.Range(0, menuItems.Length)
        ];

        if (requestedItem == null)
        {
            Debug.LogError(
                "Customer has no valid requested ItemSO!"
            );

            return;
        }

        if (orderText != null)
            orderText.text = requestedItem.itemName;

        if (orderIcon != null)
            orderIcon.sprite = requestedItem.itemSprite;

        Debug.Log(
            "CUSTOMER ORDER: " +
            requestedItem.itemName
        );

        if (customerSprites.Length > 0)
        {
            int randomSprite =
                Random.Range(0, customerSprites.Length);

            spriteRenderer.sprite =
                customerSprites[randomSprite];
        }

        Invoke(nameof(CustomerLeaves), waitTime);

        if (OrderManagerScript.Instance != null)
        {
            OrderManagerScript.Instance.AddCustomer(this);
        }
        else
        {
            Debug.LogError(
                "Customer could not find OrderManagerScript!"
            );
        }
    }

    public bool ServeCustomer(ItemSO item)
    {
        if (served)
            return false;

        if (item != requestedItem)
            return false;

        served = true;

        CancelInvoke(nameof(CustomerLeaves));

        Object.FindAnyObjectByType<CustomerSpawner>()
            .ClearSpot(spawnIndex);

        if (MoneyManager.Instance != null)
        {
            MoneyManager.Instance.AddMoney(
                requestedItem.price
            );
        }

        Debug.Log(
            "Correct order! Customer served."
        );

        if (OrderManagerScript.Instance != null)
        {
            OrderManagerScript.Instance.RemoveCustomer(this);
        }

        Destroy(gameObject);

        return true;
    }
    void CustomerLeaves()
    {
        if (served)
            return;

        Debug.Log("Customer got tired and left.");

        if (OrderManagerScript.Instance != null)
        {
            OrderManagerScript.Instance.RemoveCustomer(this);
        }

        Object.FindAnyObjectByType<CustomerSpawner>().ClearSpot(spawnIndex);

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