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

    public bool EquipItem(Item item)
    {
        if (item.ItemType == SlotType)
        {
            if (EquippedItem != null)
            {
                GameManager.Instance.PlayerInventory.AddItem(EquippedItem, 1);
            }

            EquippedItem = item;
            return true;
        }
        return false;
    }

    public Item UnequipItem()
    {
        Item temp = EquippedItem;
        EquippedItem = null;
        return temp;
    }
}
