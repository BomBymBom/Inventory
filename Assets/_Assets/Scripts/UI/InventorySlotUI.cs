using UnityEngine;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private Text stackCountText;

    private InventorySlot slot;

    public InventorySlot Slot => slot;

    public void SetSlotReference(InventorySlot slot)
    {
        this.slot = slot;
    }

    public void Refresh()
    {
        if (slot != null && slot.StoredItem != null)
        {
            itemIcon.enabled = true;
            itemIcon.sprite = slot.StoredItem.ItemIcon;
            stackCountText.text = slot.CurrentStackCount > 1 ? slot.CurrentStackCount.ToString() : "";
        }
        else
        {
            itemIcon.enabled = false;
            stackCountText.text = "";
        }
    }
}
