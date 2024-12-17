/// <summary>
/// Represents a single slot in the inventory, which can hold an Item and a certain stack count.
/// </summary>
public class InventorySlot
{
    public Item StoredItem { get; private set; }
    public int CurrentStackCount { get; private set; }

    public InventorySlot()
    {
        StoredItem = null;
        CurrentStackCount = 0;
    }

    /// <summary>
    /// Adds a certain count of the given item to this slot if it matches or is empty.
    /// Returns how many items were actually added.
    /// </summary>
    public int AddItem(Item item, int countToAdd)
    {
        if (StoredItem == null)
        {
            // Empty slot, add the item here
            StoredItem = item;
            int added = UnityEngine.Mathf.Min(countToAdd, item.MaxStackCount);
            CurrentStackCount = added;
            return added;
        }
        else if (StoredItem.ItemName == item.ItemName)
        {
            // Same item type, stack
            int spaceLeft = StoredItem.MaxStackCount - CurrentStackCount;
            int added = UnityEngine.Mathf.Min(countToAdd, spaceLeft);
            CurrentStackCount += added;
            return added;
        }

        // Different item, cannot add here
        return 0;
    }

    /// <summary>
    /// Removes a certain count of items from the slot. Returns how many were actually removed.
    /// </summary>
    public int RemoveItem(int countToRemove)
    {
        if (StoredItem == null) return 0;

        int removed = UnityEngine.Mathf.Min(countToRemove, CurrentStackCount);
        CurrentStackCount -= removed;

        if (CurrentStackCount <= 0)
        {
            // Slot is now empty
            StoredItem = null;
            CurrentStackCount = 0;
        }

        return removed;
    }

    /// <summary>
    /// Checks if this slot is empty.
    /// </summary>
    public bool IsEmpty()
    {
        return StoredItem == null || CurrentStackCount == 0;
    }
}
