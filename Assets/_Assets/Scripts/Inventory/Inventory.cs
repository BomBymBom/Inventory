using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages a collection of InventorySlots and provides methods to add/remove items.
/// </summary>
public class Inventory
{
    public event Action OnInventoryChanged;

    private List<InventorySlot> slots;
    private int maxSlots;

    public Inventory(int maxSlots)
    {
        this.maxSlots = maxSlots;
        slots = new List<InventorySlot>(maxSlots);
        for (int i = 0; i < maxSlots; i++)
        {
            slots.Add(new InventorySlot());
        }
    }

    public bool AddItem(Item item, int count)
    {
        // Try to stack into existing slots of the same item type
        if (item.IsStackable)
        {
            foreach (var slot in slots)
            {
                if (slot.StoredItem != null && slot.StoredItem.ItemName == item.ItemName)
                {
                    int added = slot.AddItem(item, count);
                    count -= added;
                    if (count <= 0) break;
                }
            }
        }

        // Try empty slots
        if (count > 0)
        {
            foreach (var slot in slots)
            {
                if (slot.IsEmpty())
                {
                    int added = slot.AddItem(item, count);
                    count -= added;
                    if (count <= 0) break;
                }
            }

            if (count > 0)
            {
                GameManager.Instance.ItemSpawner.DropItemOnGround(item, count);
            }
        }

        OnInventoryChanged?.Invoke();

        return count <= 0;
    }

    public int RemoveItem(Item item, int count)
    {
        int removedCount = 0;

        foreach (var slot in slots)
        {
            if (slot.StoredItem == item)
            {
                int removed = slot.RemoveItem(count);
                removedCount += removed;
                count -= removed;
                if (count <= 0) break;
            }
        }

        OnInventoryChanged?.Invoke();

        return removedCount;
    }

    public List<InventorySlot> GetSlots()
    {
        return slots;
    }

    public void ForceInventoryUpdate()
    {
        OnInventoryChanged?.Invoke();
    }

    public void ReorganizeInventory(ItemType itemType)
    {
        List<(Item item, int quantity)> itemsToReorganize = new List<(Item, int)>();

        foreach (var slot in slots)
        {
            if (!slot.IsEmpty() && slot.StoredItem.ItemType == itemType)
            {
                itemsToReorganize.Add((slot.StoredItem, slot.CurrentStackCount));
                slot.RemoveItem(slot.CurrentStackCount);
            }
        }

        foreach (var (item, quantity) in itemsToReorganize)
        {
            AddItem(item, quantity);
        }

        OnInventoryChanged?.Invoke();
    }

}
