using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHoldingScript : MonoBehaviour
{
    public Transform holdPoint;      // Empty GameObject where the item appears
    public GameObject heldObject;    // Current instantiated object
    public ItemSO heldItem;          // Current item data

    public bool IsHoldingItem()
    {
        return heldObject != null;
    }

    public void HoldItem(ItemSO item, GameObject prefab)
    {
        // Prevent holding two items
        if (heldObject != null)
            Destroy(heldObject);

        heldItem = item;

        heldObject = Instantiate(prefab, holdPoint);
        heldObject.transform.localPosition = Vector3.zero;
        heldObject.transform.localRotation = Quaternion.identity;
    }

    public void ClearItem()
    {
        if (heldObject != null)
        {
            Destroy(heldObject);
        }

        heldObject = null;
        heldItem = null;
    }

    public GameObject RemoveHeldItem()
    {
        GameObject item = heldObject;

        Debug.Log("Removing held item: " + item);

        heldObject = null;
        heldItem = null;

        return item;
    }
} 
    
