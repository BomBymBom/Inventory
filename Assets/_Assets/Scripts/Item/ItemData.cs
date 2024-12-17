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
    [SerializeField] private ItemUseData itemUseData; // Could be null if no usage

    public string ItemName => itemName;
    public Sprite ItemIcon => itemIcon;
    public ItemType ItemType => itemType;
    public int MaxStackCount => maxStackCount;
    public ItemUseData ItemUseData => itemUseData;
}
