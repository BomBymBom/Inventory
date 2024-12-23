using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages a collection of InventorySlots and provides methods to add/remove items.
/// Also fires events when the inventory changes.
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

    /// <summary>
    /// Attempts to add the specified count of the given item.
    /// If the inventory is full and cannot accept them, returns false.
    /// Otherwise returns true and updates the slots accordingly.
    /// </summary>
    public bool AddItem(Item item, int count)
    {
        // First, try to stack into existing slots of the same item type
        foreach (var slot in slots)
        {
            if (slot.StoredItem != null && slot.StoredItem.ItemName == item.ItemName)
            {
                int added = slot.AddItem(item, count);
                count -= added;
                if (count <= 0) break;
            }
        }

        // If still have items left, try empty slots
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

        // If still items left that we couldn't add, return false
        return count <= 0;
    }

    /// <summary>
    /// Removes a certain count of a given item. Returns how many were removed.
    /// </summary>
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

    public bool IsFull()
    {
        // If there's at least one empty slot or space in a stack, not full.
        foreach (var slot in slots)
        {
            if (slot.IsEmpty()) return false;
            if (slot.CurrentStackCount < slot.StoredItem.MaxStackCount) return false;
        }
        return true;
    }

    public List<InventorySlot> GetSlots()
    {
        return slots;
    }

    public void ForceInventoryUpdate()
    {
        OnInventoryChanged?.Invoke();
    }
}
