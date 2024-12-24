/// <summary>
/// Represents a single slot in the inventory.
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

    public int AddItem(Item item, int countToAdd)
    {
        if (StoredItem == null)
        {
            StoredItem = item;
            int added = UnityEngine.Mathf.Min(countToAdd, item.MaxStackCount);
            CurrentStackCount = added;
            return added;
        }
        else if (StoredItem.ItemName == item.ItemName)
        {
            int spaceLeft = StoredItem.MaxStackCount - CurrentStackCount;
            int added = UnityEngine.Mathf.Min(countToAdd, spaceLeft);
            CurrentStackCount += added;
            return added;
        }

        return 0;
    }

    public int RemoveItem(int countToRemove)
    {
        if (StoredItem == null) return 0;

        int removed = UnityEngine.Mathf.Min(countToRemove, CurrentStackCount);
        CurrentStackCount -= removed;

        if (CurrentStackCount <= 0)
        {
            StoredItem = null;
            CurrentStackCount = 0;
        }

        return removed;
    }

    public bool IsEmpty()
    {
        return StoredItem == null || CurrentStackCount == 0;
    }
}
