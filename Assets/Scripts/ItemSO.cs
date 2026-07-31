using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemSO : ScriptableObject
{
    public string itemName;
    public Sprite itemSprite;


    public GameObject worldPrefab;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
