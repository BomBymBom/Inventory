using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class UIInputManager : MonoBehaviour
{
    [SerializeField] private Canvas parentCanvas;
    [SerializeField] private GraphicRaycaster raycaster;
    [SerializeField] private PlayerInput inputActionsAsset;

    private InputAction pointAction;
    private InputAction leftClickAction;
    private InputAction rightClickAction;

    // Variabilă ce stochează permanent poziția cursorului (actualizată la pointAction.performed)
    private Vector2 currentPointerPos;

    // Variabile pentru drag & drop
    private GameObject draggedItemIcon;
    private Item draggedItem;
    private InventorySlot originalSlot;
    private ItemType originalEquipSlotType;
    private bool dragging = false;

    private void Awake()
    {
        Debug.Log("UIInputManager Awake called.");

        // 1. Găsim action map-ul "UI"
        var uiMap = inputActionsAsset.actions.FindActionMap("UI", true);
        // 2. Găsim acțiunile
        pointAction = uiMap.FindAction("Point", true);
        leftClickAction = uiMap.FindAction("Click", true);
        rightClickAction = uiMap.FindAction("RightClick", true);

        // 3. Ne abonăm la evenimente
        // Point: citim poziția cursorului la fiecare "performed"
        pointAction.performed += OnPointPerformed;

        // Click stânga: "started" -> user a apăsat butonul; "canceled" -> user a eliberat butonul
        leftClickAction.started += OnLeftClickStarted;
        leftClickAction.canceled += OnLeftClickCanceled;

        // Click dreapta: "performed" -> user a apăsat butonul (sau l-a apăsat și eliberat, depinde de interacție)
        rightClickAction.started += OnRightClickPerformed;

        pointAction.Enable();
        leftClickAction.Enable();
        rightClickAction.Enable();
    }

    private void OnDestroy()
    {
        // Deregistrăm callback-urile
        if (pointAction != null) pointAction.performed -= OnPointPerformed;
        if (leftClickAction != null)
        {
            leftClickAction.started -= OnLeftClickStarted;
            leftClickAction.canceled -= OnLeftClickCanceled;
        }
        if (rightClickAction != null) rightClickAction.performed -= OnRightClickPerformed;
    }

    // ---------------------------------------------------------------------------------
    // 1. Citire poziție mouse / cursor
    // ---------------------------------------------------------------------------------
    private void OnPointPerformed(InputAction.CallbackContext context)
    {
        currentPointerPos = context.ReadValue<Vector2>();

        if (dragging && draggedItemIcon != null)
        {
            draggedItemIcon.transform.position = currentPointerPos;
        }
    }

    // ---------------------------------------------------------------------------------
    // 2. Click stânga: START drag (similar WasPressedThisFrame)
    // ---------------------------------------------------------------------------------
    private void OnLeftClickStarted(InputAction.CallbackContext context)
    {
        Debug.Log("LeftClickStart");

        // Începem drag dacă am dat click pe un slot cu item
        // Folosim currentPointerPos, actualizat în OnPointPerformed
        InventorySlotUI slotUI = RaycastFor<InventorySlotUI>(currentPointerPos);
        EquipmentSlotUI equipUI = (slotUI == null)
            ? RaycastFor<EquipmentSlotUI>(currentPointerPos)
            : null;
        Debug.Log(slotUI);
        Debug.Log(equipUI);

        if (slotUI != null && slotUI.Slot != null && slotUI.Slot.StoredItem != null)
        {
            Debug.Log("LeftClick1");

            StartDraggingItem(slotUI.Slot.StoredItem, slotUI.transform.position);
            originalSlot = slotUI.Slot;
        }
        else if (equipUI != null)
        {
            Debug.Log("LeftClick2");

            var eqSlot = GameManager.Instance.PlayerEquipment.GetEquipmentSlot(equipUI.SlotType);
            if (eqSlot != null && eqSlot.EquippedItem != null)
            {
                Debug.Log("LeftClick3");

                // Luăm item-ul din echipament
                StartDraggingItem(eqSlot.EquippedItem, equipUI.transform.position);
                originalEquipSlotType = equipUI.SlotType;
                // Dezechipăm imediat pentru a-l "ține în mână"
                GameManager.Instance.PlayerEquipment.UnequipItem(equipUI.SlotType);
            }
        }
    }

    // ---------------------------------------------------------------------------------
    // 3. Click stânga: END drag (similar WasReleasedThisFrame)
    // ---------------------------------------------------------------------------------
    private void OnLeftClickCanceled(InputAction.CallbackContext context)
    {
        if (!dragging) return; // Nici nu am pornit drag-ul
        Debug.Log("OnLeftClickCanceled");

        EndDraggingItem(currentPointerPos);
    }

    // ---------------------------------------------------------------------------------
    // 4. Click dreapta: "performed"
    // ---------------------------------------------------------------------------------
    private void OnRightClickPerformed(InputAction.CallbackContext context)
    {
        // Dacă dragăm ceva, ignorăm right-click
        Debug.Log("OnRightClickPerformedreturn");

        if (dragging) return;
        Debug.Log("OnRightClickPerformed");

        // Verificăm slotUI sub cursor
        var slotUI = RaycastFor<InventorySlotUI>(currentPointerPos);
        if (slotUI != null && slotUI.Slot != null && slotUI.Slot.StoredItem != null)
        {
            Debug.Log("Use item");

            // Use item
            slotUI.Slot.StoredItem.UseItem(GameManager.Instance.PlayerCharacter);
            // Eliminăm unul din stack
            GameManager.Instance.PlayerInventory.RemoveItem(slotUI.Slot.StoredItem, 1);
            return;
        }

        // Verificăm echipament
        var equipUI = RaycastFor<EquipmentSlotUI>(currentPointerPos);
        if (equipUI != null)
        {
            Debug.Log("Verificăm echipament");

            var eqSlot = GameManager.Instance.PlayerEquipment.GetEquipmentSlot(equipUI.SlotType);
            if (eqSlot != null && eqSlot.EquippedItem != null)
            {
                // Dezechipăm item-ul
                Debug.Log("Dezechipăm item-ul");

                Item unequippedItem = GameManager.Instance.PlayerEquipment.UnequipItem(equipUI.SlotType);
                if (unequippedItem != null)
                {
                    GameManager.Instance.PlayerInventory.AddItem(unequippedItem, 1);
                }
            }
        }
    }

    // ---------------------------------------------------------------------------------
    // METODE AUXILIARE
    // ---------------------------------------------------------------------------------
    private void StartDraggingItem(Item item, Vector3 startPos)
    {
        Debug.Log("StartDraggingItem");

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
        Debug.Log("EndDraggingItem");

        InventorySlotUI targetSlotUI = RaycastFor<InventorySlotUI>(cursorPos);
        EquipmentSlotUI targetEquipUI = null;
        if (targetSlotUI == null)
            targetEquipUI = RaycastFor<EquipmentSlotUI>(cursorPos);

        // Dacă item-ul venea din inventar
        if (originalSlot != null && draggedItem != null)
        {
            // Scoatem item-ul din inventar (1 bucată)
            GameManager.Instance.PlayerInventory.RemoveItem(draggedItem, 1);
            if (targetSlotUI != null)
            {
                if (!targetSlotUI.Slot.IsEmpty())
                {
                    // Vrei să faci un swap
                    Item tempItem = targetSlotUI.Slot.StoredItem;
                    int tempCount = targetSlotUI.Slot.CurrentStackCount;

                    // Scoți itemul din slotul țintă
                    targetSlotUI.Slot.RemoveItem(tempCount);
                    // Pui itemul curent acolo
                    targetSlotUI.Slot.AddItem(draggedItem, 1);

                    // Re-adaugi itemul pe care l-ai scos din slotul țintă în slotul original
                    // (mai întâi scoți itemul curent din slotul original – deja îl scoți mai sus)
                    // Apoi:
                    originalSlot.AddItem(tempItem, tempCount);
                    // ... eventul OnInventoryChanged etc.
                }
                else
                {
                    targetSlotUI.Slot.AddItem(draggedItem, 1);
                }

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
                // Încearcă să echipezi în alt slot
                if (!GameManager.Instance.PlayerEquipment.EquipItem(draggedItem))
                {
                    // Dacă nu se potrivește, îl punem înapoi în inventar
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

        GameManager.Instance.PlayerInventory?.ForceInventoryUpdate();
    }

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
                return comp;
        }
        return null;
    }
}
