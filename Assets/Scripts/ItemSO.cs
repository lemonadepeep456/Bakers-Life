using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
[System.Serializable]
public class CookingRecipe
{
    [Header("Input")]

    // Item placed onto the station.
    public ItemSO inputItem;

    [Header("Output")]

    // Item received after cooking.
    public ItemSO outputItem;

    [Header("Cooking")]

    // Time this recipe takes.
    public float cookTime = 5f;
}
public class ItemSO : ScriptableObject
{
    public string itemName;
    public Sprite itemSprite;
    public GameObject worldPrefab;

    public int price;

    [Header("Cooking")]

    // Does this item need to be cooked?
    public bool requiresCooking;
}