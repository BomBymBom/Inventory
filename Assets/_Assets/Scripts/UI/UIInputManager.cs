using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class UIInputManager : MonoBehaviour
{
    [SerializeField] private Canvas parentCanvas;
    [SerializeField] private GraphicRaycaster raycaster;

    // Referință la Input Actions. Presupunem că ai un set de acțiuni generat (ex: PlayerUIActions)
    [SerializeField] private PlayerInput inputActionsAsset;
    private InputAction pointAction;
    private InputAction leftClickAction;
    private InputAction rightClickAction;

    private GameObject draggedItemIcon;
    private Item draggedItem;
    private InventorySlot originalSlot;
    private ItemType originalEquipSlotType;
    private bool dragging = false;

    private void Awake()
    {
        // Găsim acțiunile. Presupunem că ai un "UI" action map cu "Point", "Click", "RightClick".
        var uiMap = inputActionsAsset.actions.FindActionMap("UI", true);
        pointAction = uiMap.FindAction("Point", true);
        leftClickAction = uiMap.FindAction("Click", true);
        rightClickAction = uiMap.FindAction("RightClick", true);

        pointAction.Enable();
        leftClickAction.Enable();
        rightClickAction.Enable();
    }

    private void Update()
    {
        Vector2 cursorPos = pointAction.ReadValue<Vector2>();

        // Detectăm click dreapta
        if (rightClickAction.triggered && !dragging)
        {
            // Click dreapta - folosire item daca e pe slot
            var slotUI = RaycastFor<InventorySlotUI>(cursorPos);
            if (slotUI != null && slotUI.Slot != null && slotUI.Slot.StoredItem != null)
            {
                // Folosim item-ul
                slotUI.Slot.StoredItem.UseItem(GameManager.Instance.PlayerCharacter);
                // Eliminăm unul din stack
                GameManager.Instance.PlayerInventory.RemoveItem(slotUI.Slot.StoredItem, 1);
            }

            // Sau echipament - click dreapta poate dezechipare dacă dorim.
            var equipUI = RaycastFor<EquipmentSlotUI>(cursorPos);
            if (equipUI != null)
            {
                var eqSlot = GameManager.Instance.PlayerEquipment.GetEquipmentSlot(equipUI.SlotType);
                if (eqSlot != null && eqSlot.EquippedItem != null)
                {
                    // Dezechipăm item-ul
                    Item unequippedItem = GameManager.Instance.PlayerEquipment.UnequipItem(equipUI.SlotType);
                    if (unequippedItem != null)
                    {
                        GameManager.Instance.PlayerInventory.AddItem(unequippedItem, 1);
                    }
                }
            }
        }

        // Detectăm click stânga: poate fi folosit pentru drag & drop
        if (leftClickAction.WasPressedThisFrame())
        {
            // Începem drag dacă am dat click pe un slot cu item
            var slotUI = RaycastFor<InventorySlotUI>(cursorPos);
            var equipUI = slotUI == null ? RaycastFor<EquipmentSlotUI>(cursorPos) : null;

            if (slotUI != null && slotUI.Slot != null && slotUI.Slot.StoredItem != null)
            {
                StartDraggingItem(slotUI.Slot.StoredItem, slotUI.transform.position);
                originalSlot = slotUI.Slot;
            }
            else if (equipUI != null)
            {
                var eqSlot = GameManager.Instance.PlayerEquipment.GetEquipmentSlot(equipUI.SlotType);
                if (eqSlot != null && eqSlot.EquippedItem != null)
                {
                    // Luăm item-ul din echipament
                    StartDraggingItem(eqSlot.EquippedItem, equipUI.transform.position);
                    originalEquipSlotType = equipUI.SlotType;
                    // Dezechipăm imediat pentru a-l "ține în mână"
                    GameManager.Instance.PlayerEquipment.UnequipItem(equipUI.SlotType);
                }
            }
        }

        // Dacă dragăm un item, urmărim poziția cursorului
        if (dragging && draggedItemIcon != null)
        {
            draggedItemIcon.transform.position = cursorPos;
        }

        // Când eliberăm click stânga, finalizăm drag & drop
        if (leftClickAction.WasReleasedThisFrame() && dragging)
        {
            EndDraggingItem(cursorPos);
        }
    }

    private void StartDraggingItem(Item item, Vector3 startPos)
    {
        draggedItem = item;
        dragging = true;

        draggedItemIcon = new GameObject("DraggedItemIcon");
        draggedItemIcon.transform.SetParent(parentCanvas.transform, false);
        var img = draggedItemIcon.AddComponent<Image>();
        img.sprite = item.ItemIcon;
        var cg = draggedItemIcon.AddComponent<CanvasGroup>();
        cg.blocksRaycasts = false;
        draggedItemIcon.GetComponent<RectTransform>().sizeDelta = new Vector2(40, 40);
        draggedItemIcon.transform.position = startPos;
    }

    private void EndDraggingItem(Vector2 cursorPos)
    {
        // Verificăm unde am lăsat item-ul
        InventorySlotUI targetSlotUI = RaycastFor<InventorySlotUI>(cursorPos);
        EquipmentSlotUI targetEquipUI = null;
        if (targetSlotUI == null)
            targetEquipUI = RaycastFor<EquipmentSlotUI>(cursorPos);

        // Dacă item-ul venea din inventar
        if (originalSlot != null && draggedItem != null)
        {
            // Scoatem item-ul din inventar (1 bucată)
            GameManager.Instance.PlayerInventory.RemoveItem(draggedItem, 1);

            if (targetSlotUI != null && targetSlotUI.Slot != originalSlot)
            {
                // Mutăm item în alt slot de inventar
                GameManager.Instance.PlayerInventory.AddItem(draggedItem, 1);
            }
            else if (targetEquipUI != null)
            {
                // Încercăm să echipăm item-ul
                if (GameManager.Instance.PlayerEquipment.EquipItem(draggedItem))
                {
                    // echipat cu succes
                }
                else
                {
                    // dacă nu a putut fi echipat, îl adăugăm înapoi în inventar
                    GameManager.Instance.PlayerInventory.AddItem(draggedItem, 1);
                }
            }
            else
            {
                // Aruncăm pe jos
                GameManager.Instance.DropItemOnGround(draggedItem);
            }
        }

        // Dacă item-ul venea din echipament
        if (originalEquipSlotType != ItemType.None && draggedItem != null)
        {
            // Am deja item-ul "scos" din echipament
            if (targetSlotUI != null)
            {
                // Pune item-ul în inventar
                GameManager.Instance.PlayerInventory.AddItem(draggedItem, 1);
            }
            else if (targetEquipUI != null && targetEquipUI.SlotType != originalEquipSlotType)
            {
                // Încearcă să echipezi în alt slot (dacă tipul corespunde)
                if (!GameManager.Instance.PlayerEquipment.EquipItem(draggedItem))
                {
                    // Dacă nu se potrivește, îl punem în inventar
                    GameManager.Instance.PlayerInventory.AddItem(draggedItem, 1);
                }
            }
            else
            {
                // Aruncă pe jos
                GameManager.Instance.DropItemOnGround(draggedItem);
            }
        }

        // Curățăm starea de drag
        if (draggedItemIcon != null)
        {
            Destroy(draggedItemIcon);
        }
        draggedItemIcon = null;
        draggedItem = null;
        dragging = false;
        originalSlot = null;
        originalEquipSlotType = ItemType.None;
    }

    /// <summary>
    /// Rulare raycast grafic și returnare componentă specifică dacă există.
    /// </summary>
    private T RaycastFor<T>(Vector2 screenPos) where T : Component
    {
        var pointerData = new PointerEventData(EventSystem.current)
        {
            position = screenPos
        };

        var results = new List<RaycastResult>();
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
