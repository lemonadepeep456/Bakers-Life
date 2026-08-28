using System.Collections.Generic;
using UnityEngine;

public class OrderManagerScript : MonoBehaviour
{
    public static OrderManagerScript Instance;

    public OrderSlotScript[] slots;

    public List<Customer> activeCustomers = new List<Customer>();

    public WaitressAI waitress;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        RefreshUI();
    }

    public void AddCustomer(Customer customer)
    {
        if (customer == null)
            return;

        if (activeCustomers.Contains(customer))
            return;

        activeCustomers.Add(customer);

        Debug.Log(
            "ORDER ADDED: " +
            customer.requestedItem.itemName
        );

        RefreshUI();
    }

    public void RemoveCustomer(Customer customer)
    {
        if (customer == null)
            return;

        if (!activeCustomers.Contains(customer))
            return;

        activeCustomers.Remove(customer);

        Debug.Log("ORDER REMOVED");

        RefreshUI();
    }

    public Customer FindMatchingCustomer(ItemSO item)
    {
        if (item == null)
            return null;

        foreach (Customer customer in activeCustomers)
        {
            if (customer == null)
                continue;

            if (customer.requestedItem == item)
            {
                return customer;
            }
        }

        return null;
    }

    public bool TryDeliverItem(
      ItemSO placedItem,
      Transform counterTransform,
      GameObject foodObject
  )
    {
        if (placedItem == null)
        {
            Debug.LogError("OrderManager: ItemSO is null.");
            return false;
        }

        if (foodObject == null)
        {
            Debug.LogError("OrderManager: Food object is null.");
            return false;
        }

        Customer matchingCustomer =
            FindMatchingCustomer(placedItem);

        if (matchingCustomer == null)
        {
            Debug.Log(
                "OrderManager: No customer ordered " +
                placedItem.itemName
            );

            return false;
        }

        if (waitress == null)
        {
            Debug.LogError(
                "OrderManager: Waitress is not assigned."
            );

            return false;
        }

        Debug.Log(
            "OrderManager: Found order for " +
            placedItem.itemName
        );

        waitress.StartDeliveryRoute(
            counterTransform,
            matchingCustomer.transform,
            foodObject,
            placedItem
        );

        return true;
    }
    public void RefreshUI()
    {
        if (slots == null)
        {
            Debug.LogError("OrderManager has no slots array.");
            return;
        }

        Debug.Log(
            "Refreshing order UI. Active customers: " +
            activeCustomers.Count
        );

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == null)
                continue;

            if (i < activeCustomers.Count &&
                activeCustomers[i] != null)
            {
                slots[i].SetOrder(activeCustomers[i]);
            }
            else
            {
                slots[i].ClearOrder();
            }
        }
    }
}