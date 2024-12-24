using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class DragDropManager : MonoBehaviour
{
    [Header("Canvas & Raycasting")]
    [SerializeField] private Canvas parentCanvas;
    [SerializeField] private GraphicRaycaster raycaster;

    private GameObject draggedItemIcon;
    private Item draggedItem;
    private InventorySlot originalSlot;
    private ItemType originalEquipSlotType;

    private bool isDragging = false;
    private Vector2 currentPointerPos;

    #region PublicMethods
    public void OnPointerMoved(Vector2 pointerPos)
    {
        currentPointerPos = pointerPos;

        if (isDragging && draggedItemIcon != null)
        {
            draggedItemIcon.transform.position = currentPointerPos;
        }
    }

    public void OnLeftClickStarted()
    {
        AttemptBeginDrag();
    }

    public void OnLeftClickEnded()
    {
        if (!isDragging) return;
        EndDraggingItem(currentPointerPos);
    }

    public void OnRightClickPerformed()
    {
        if (isDragging) return;

        AttemptRightClickAction();
    }

    #endregion

    #region Drag&Drop
    private void AttemptBeginDrag()
    {
        InventorySlotUI slotUI = RaycastFor<InventorySlotUI>(currentPointerPos);
        EquipmentSlotUI equipUI = (slotUI == null) ? RaycastFor<EquipmentSlotUI>(currentPointerPos) : null;

        if (slotUI != null && slotUI.Slot != null && slotUI.Slot.StoredItem != null)
        {
            originalSlot = slotUI.Slot;
            StartDraggingItem(slotUI.Slot.StoredItem, slotUI.transform.position);
        }
        else if (equipUI != null)
        {
            var eqSlot = GameManager.Instance.PlayerEquipment.GetEquipmentSlot(equipUI.SlotType);
            if (eqSlot != null && eqSlot.EquippedItem != null)
            {
                StartDraggingItem(eqSlot.EquippedItem, equipUI.transform.position);
                originalEquipSlotType = equipUI.SlotType;
                GameManager.Instance.PlayerEquipment.UnequipItem(equipUI.SlotType);
            }
        }
    }

    private void StartDraggingItem(Item item, Vector3 startPos)
    {
        isDragging = true;
        draggedItem = item;

        draggedItemIcon = new GameObject("DraggedItemIcon");
        draggedItemIcon.transform.SetParent(parentCanvas.transform, false);
        draggedItemIcon.transform.position = startPos;

        var img = draggedItemIcon.AddComponent<Image>();
        img.sprite = item.ItemIcon;

        var cg = draggedItemIcon.AddComponent<CanvasGroup>();
        cg.blocksRaycasts = false;

        draggedItemIcon.GetComponent<RectTransform>().sizeDelta = new Vector2(40, 40);
    }

    private void EndDraggingItem(Vector2 pointerPos)
    {
        InventorySlotUI targetSlotUI = RaycastFor<InventorySlotUI>(pointerPos);
        EquipmentSlotUI targetEquipUI = (targetSlotUI == null) ? RaycastFor<EquipmentSlotUI>(pointerPos) : null;

        int stackCount = originalSlot?.CurrentStackCount ?? 1;

        if (originalSlot != null && draggedItem != null)
        {
            HandleDropFromInventory(targetSlotUI, targetEquipUI, stackCount);
        }
        else if (originalEquipSlotType != ItemType.None && draggedItem != null)
        {
            HandleDropFromEquipment(targetSlotUI, targetEquipUI, stackCount);
        }

        CleanupDragState();
        GameManager.Instance.PlayerInventory?.ForceInventoryUpdate();
    }

    private void CleanupDragState()
    {
        if (draggedItemIcon)
        {
            Destroy(draggedItemIcon);
        }

        isDragging = false;
        draggedItemIcon = null;
        draggedItem = null;
        originalSlot = null;
        originalEquipSlotType = ItemType.None;
    }

    #endregion

    #region DropHandlers
    private void HandleDropFromInventory(InventorySlotUI targetSlotUI, EquipmentSlotUI targetEquipUI, int stackCount)
    {
        GameManager.Instance.PlayerInventory.RemoveItem(originalSlot.StoredItem, stackCount);

        if (targetSlotUI != null)
        {
            if (!targetSlotUI.Slot.IsEmpty())
            {
                // Swap
                var tempItem = targetSlotUI.Slot.StoredItem;
                int tempCount = targetSlotUI.Slot.CurrentStackCount;

                targetSlotUI.Slot.RemoveItem(tempCount);
                targetSlotUI.Slot.AddItem(draggedItem, 1);

                originalSlot.AddItem(tempItem, tempCount);
            }
            else
            {
                targetSlotUI.Slot.AddItem(draggedItem, stackCount);
            }
        }
        else if (targetEquipUI != null)
        {
            // Try to equip
            if (!GameManager.Instance.PlayerEquipment.EquipItem(draggedItem))
            {
                // If can't equip, return to inventory
                GameManager.Instance.PlayerInventory.AddItem(draggedItem, stackCount);
            }
        }
        else
        {
            // Drop on ground
            GameManager.Instance.ItemSpawner.DropItemOnGround(draggedItem, stackCount);
        }
    }

    private void HandleDropFromEquipment(InventorySlotUI targetSlotUI, EquipmentSlotUI targetEquipUI, int stackCount)
    {
        if (targetSlotUI != null)
        {
            GameManager.Instance.PlayerInventory.AddItem(draggedItem, stackCount);
        }
        else if (targetEquipUI != null && targetEquipUI.SlotType != originalEquipSlotType)
        {
            if (!GameManager.Instance.PlayerEquipment.EquipItem(draggedItem))
            {
                GameManager.Instance.PlayerInventory.AddItem(draggedItem, stackCount);
            }
        }
        else
        {
            GameManager.Instance.ItemSpawner.DropItemOnGround(draggedItem, stackCount);
        }
    }

    #endregion

    private void AttemptRightClickAction()
    {
        // Inventory item right-click => Use item
        var slotUI = RaycastFor<InventorySlotUI>(currentPointerPos);
        if (slotUI != null && slotUI.Slot != null && slotUI.Slot.StoredItem != null)
        {
            bool used = slotUI.Slot.StoredItem.UseItem(GameManager.Instance.PlayerCharacter);
            if (used)
            {
                GameManager.Instance.PlayerInventory.RemoveItem(slotUI.Slot.StoredItem, 1);
            }
            return;
        }

        // Equipment slot right-click => Unequip
        var equipUI = RaycastFor<EquipmentSlotUI>(currentPointerPos);
        if (equipUI != null)
        {
            var eqSlot = GameManager.Instance.PlayerEquipment.GetEquipmentSlot(equipUI.SlotType);
            if (eqSlot != null && eqSlot.EquippedItem != null)
            {
                Item unequippedItem = GameManager.Instance.PlayerEquipment.UnequipItem(equipUI.SlotType);
                if (unequippedItem != null)
                {
                    GameManager.Instance.PlayerInventory.AddItem(unequippedItem, 1);
                }
            }
        }
    }

    private T RaycastFor<T>(Vector2 screenPos) where T : Component
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = screenPos
        };

        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(pointerData, results);

        foreach (var res in results)
        {
            var comp = res.gameObject.GetComponent<T>();
            if (comp != null)
            {
                return comp;
            }
        }
        return null;
    }
}
