using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// The ItemFactory now uses ScriptableObjects (ItemData) to create Items.
/// You can assign a list of ItemData in the inspector to this factory.
/// </summary>
public class ItemFactory : MonoBehaviour
{
    [SerializeField] private List<ItemData> itemDataList;

    private Dictionary<ItemType, ItemData> itemDataByType;

    private void Awake()
    {
        // Build a dictionary for quick lookup by ItemType
        itemDataByType = new Dictionary<ItemType, ItemData>();
        foreach (var data in itemDataList)
        {
            if (!itemDataByType.ContainsKey(data.ItemType))
            {
                itemDataByType[data.ItemType] = data;
            }
            else
            {
                Debug.LogWarning($"Multiple items with the same ItemType {data.ItemType} found. Using the first one.");
            }
        }
    }

    /// <summary>
    /// Creates an Item from the provided ItemType, using the configured ItemData.
    /// </summary>
    public Item CreateItem(ItemType type)
    {
        if (itemDataByType.TryGetValue(type, out ItemData data))
        {
            Item newItem = new Item(
                itemName: data.ItemName,
                icon: data.ItemIcon,
                itemType: data.ItemType,
                maxStackCount: data.MaxStackCount
            );

            // Assign use data if available
            if (data.ItemUseData != null)
            {
                newItem.SetUseStrategy(data.ItemUseData);
            }

            return newItem;
        }

        Debug.LogWarning($"No ItemData found for ItemType {type}. Returning a generic item.");
        // If no data found, return a generic item or null.
        return new Item("Generic Item", null, ItemType.None, 1);
    }
}
