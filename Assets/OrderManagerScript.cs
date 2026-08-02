using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class OrderManagerScript : MonoBehaviour
{
    public static OrderManagerScript Instance;

    public OrderSlotScript[] slots;

    private List<Customer> customers = new List<Customer>();

    private void Awake()
    {
        Instance = this;
    }

    public void AddCustomer(Customer customer)
    {
        
        customers.Add(customer);
        RefreshUI();
    }

    public void RemoveCustomer(Customer customer)
    {
        customers.Remove(customer);
        RefreshUI();
    }

    void RefreshUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < customers.Count)
            {
                slots[i].SetOrder(customers[i]);
            }
            else
            {
                slots[i].ClearOrder();
            }
        }
    }
}