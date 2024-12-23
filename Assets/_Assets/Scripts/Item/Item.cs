using UnityEngine;

public enum ItemType
{
    None, Weapon, Armor, Potion, Coin
}

public class Item
{
    public string ItemName { get; private set; }
    public Sprite ItemIcon { get; private set; }
    public ItemType ItemType { get; private set; }
    public int MaxStackCount { get; private set; }
    public GameObject ItemPrefab { get; private set; }
    private ItemUseData useStrategy;

    public Item(string itemName, Sprite icon, ItemType itemType, int maxStackCount, GameObject itemPrefab)
    {
        this.ItemName = itemName;
        this.ItemIcon = icon;
        this.ItemType = itemType;
        this.MaxStackCount = maxStackCount;
        this.ItemPrefab = itemPrefab;
    }

    public void SetUseStrategy(ItemUseData strategy)
    {
        this.useStrategy = strategy;
    }

    /// <summary>
    /// Called when the item is used by a character (e.g. player).
    /// This method delegates the use logic to the assigned strategy.
    /// </summary>
    public void UseItem(Character character)
    {
        if (useStrategy != null)
        {
            useStrategy.Use(this, character);
        }
        else
        {
            UnityEngine.Debug.LogWarning($"No use strategy set for item {ItemName}");
        }
    }
}
