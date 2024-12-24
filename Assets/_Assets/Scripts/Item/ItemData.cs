using UnityEngine;

/// <summary>
/// ScriptableObject that holds data for an item: type, name, icon, stack count, and associated use logic.
/// This allows item configuration from the Unity Editor.
/// </summary>
[CreateAssetMenu(fileName = "NewItemData", menuName = "ItemData/GenericItem")]
public class ItemData : ScriptableObject
{
    [SerializeField] private string itemName;
    [SerializeField] private Sprite itemIcon;
    [SerializeField] private ItemType itemType;
    [SerializeField] private int maxStackCount = 1;
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private ItemUseData itemUseData; // Could be null if no usage
    [SerializeField] private bool isStackable = true;
    public string ItemName => itemName;
    public Sprite ItemIcon => itemIcon;
    public ItemType ItemType => itemType;
    public int MaxStackCount => maxStackCount;
    public GameObject ItemPrefab => itemPrefab;
    public ItemUseData ItemUseData => itemUseData;
    public bool IsStackable => isStackable;

    public void ConfigureStackable(bool value)
    {
        isStackable = value;
    }
    public void ConfigureMaxStackCount(int value)
    {
        maxStackCount = value;
    }

}
