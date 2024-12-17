using UnityEngine;
using UnityEngine.UI;

public class EquipmentSlotUI : MonoBehaviour
{
    [SerializeField] private Image itemIcon;
    private ItemType slotType;

    public ItemType SlotType => slotType;

    public void SetSlotType(ItemType type)
    {
        slotType = type;
    }

    public void Refresh(EquipmentSlot slot)
    {
        if (slot != null && slot.EquippedItem != null)
        {
            itemIcon.enabled = true;
            itemIcon.sprite = slot.EquippedItem.ItemIcon;
        }
        else
        {
            itemIcon.enabled = false;
        }
    }
}
