using UnityEngine;
using System.Collections.Generic;

public class ItemFactory : MonoBehaviour
{
    [SerializeField] private List<ItemData> itemDataList;

    private Dictionary<ItemType, ItemData> itemDataByType;

    private void Awake()
    {
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
                maxStackCount: data.MaxStackCount,
                itemPrefab: data.ItemPrefab,
                isStackable: data.IsStackable,
                  itemData: data
            );

            // Assign use data if available
            if (data.ItemUseData != null)
            {
                newItem.SetUseStrategy(data.ItemUseData);
            }

            return newItem;
        }

        return null;
    }

}
