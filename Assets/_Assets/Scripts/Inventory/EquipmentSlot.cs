/// <summary>
/// Represents a slot where an equipment item can be placed (helmet, armor, weapon, etc.).
/// </summary>
public class EquipmentSlot
{
    public Item EquippedItem { get; private set; }
    public ItemType SlotType { get; private set; }

    public EquipmentSlot(ItemType slotType)
    {
        SlotType = slotType;
        EquippedItem = null;
    }

    /// <summary>
    /// Equip the given item if it matches the slot type.
    /// Returns true if equipped successfully, false otherwise.
    /// </summary>
    public bool EquipItem(Item item)
    {
        // In a real scenario, you'd match specific item types to slot types.
        // For simplicity, let's say the slot can only hold the same ItemType.
        if (item.ItemType == SlotType)
        {
            EquippedItem = item;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Unequips the current item from this slot.
    /// </summary>
    public Item UnequipItem()
    {
        Item temp = EquippedItem;
        EquippedItem = null;
        return temp;
    }
}
