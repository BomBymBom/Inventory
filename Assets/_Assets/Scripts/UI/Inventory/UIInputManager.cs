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
    [SerializeField] private GameObject configWindow;
    private InputAction pointAction;
    private InputAction leftClickAction;
    private InputAction rightClickAction;
    private InputAction openConfigAction;

    // Variabilă ce stochează permanent poziția cursorului (actualizată la pointAction.performed)
    private Vector2 currentPointerPos;

    // Variabile pentru drag & drop
    private GameObject draggedItemIcon;
    private Item draggedItem;
    private InventorySlot originalSlot;
    private ItemType originalEquipSlotType;
    private bool dragging = false;
    private bool isConfigOpen = false;

    private void Awake()
    {
        Debug.Log("UIInputManager Awake called.");

        // 1. Găsim action map-ul "UI"
        var uiMap = inputActionsAsset.actions.FindActionMap("UI", true);
        // 2. Găsim acțiunile
        pointAction = uiMap.FindAction("Point", true);
        leftClickAction = uiMap.FindAction("Click", true);
        rightClickAction = uiMap.FindAction("RightClick", true);
        openConfigAction = uiMap.FindAction("MiddleClick", true);
        openConfigAction.started += OnOpenConfigPerformed;
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
        if (rightClickAction != null) rightClickAction.started -= OnRightClickPerformed;
        if (openConfigAction != null) openConfigAction.performed -= OnOpenConfigPerformed;
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
        if (configWindow.activeSelf && !IsPointerOverConfigUI())
        {
            CloseConfigWindow();
            return;
        }
        else if (IsPointerOverConfigUI())
        {
            return;
        }

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

            originalSlot = slotUI.Slot;
            StartDraggingItem(slotUI.Slot.StoredItem, slotUI.transform.position);
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
        if (configWindow.activeSelf && !IsPointerOverConfigUI())
        {
            CloseConfigWindow();
            return;
        }
        else if (IsPointerOverConfigUI())
        {
            return;
        }
        // Dacă dragăm ceva, ignorăm right-click
        if (dragging) return;
        // Verificăm slotUI sub cursor
        var slotUI = RaycastFor<InventorySlotUI>(currentPointerPos);
        if (slotUI != null && slotUI.Slot != null && slotUI.Slot.StoredItem != null)
        {
            Debug.Log("Use item");

            // Use item
            bool used = slotUI.Slot.StoredItem.UseItem(GameManager.Instance.PlayerCharacter);
            if (used)
            {
                // Elimină doar dacă utilizarea a fost reușită
                GameManager.Instance.PlayerInventory.RemoveItem(slotUI.Slot.StoredItem, 1);
            }
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

        int stackCount = originalSlot?.CurrentStackCount ?? 1;
        // Dacă item-ul venea din inventar
        if (originalSlot != null && draggedItem != null)
        {
            // Scoatem item-ul din inventar (1 bucată)
            GameManager.Instance.PlayerInventory.RemoveItem(originalSlot.StoredItem, stackCount);
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
                    targetSlotUI.Slot.AddItem(draggedItem, stackCount);
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
                    GameManager.Instance.PlayerInventory.AddItem(draggedItem, stackCount);
                }
            }
            else
            {
                // Aruncăm pe jos
                GameManager.Instance.ItemSpawner.DropItemOnGround(draggedItem, stackCount);
            }
        }

        // Dacă item-ul venea din echipament
        if (originalEquipSlotType != ItemType.None && draggedItem != null)
        {
            // Am deja item-ul "scos" din echipament
            if (targetSlotUI != null)
            {
                // Pune item-ul în inventar
                GameManager.Instance.PlayerInventory.AddItem(draggedItem, stackCount);
            }
            else if (targetEquipUI != null && targetEquipUI.SlotType != originalEquipSlotType)
            {
                // Încearcă să echipezi în alt slot
                if (!GameManager.Instance.PlayerEquipment.EquipItem(draggedItem))
                {
                    // Dacă nu se potrivește, îl punem înapoi în inventar
                    GameManager.Instance.PlayerInventory.AddItem(draggedItem, stackCount);
                }
            }
            else
            {
                // Aruncă pe jos
                GameManager.Instance.ItemSpawner.DropItemOnGround(draggedItem, stackCount);
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
    private void OnOpenConfigPerformed(InputAction.CallbackContext context)
    {
        if (isConfigOpen)
        {
            CloseConfigWindow();
            return;
        }

        // Detectează slotul pe care s-a făcut click
        InventorySlotUI slotUI = RaycastFor<InventorySlotUI>(currentPointerPos);
        EquipmentSlotUI equipUI = (slotUI == null) ? RaycastFor<EquipmentSlotUI>(currentPointerPos) : null;

        if (slotUI == null && equipUI == null)
        {
            Debug.LogWarning("No item selected for configuration.");
            return;
        }

        // Obține item-ul din slot sau echipament
        ItemData selectedItemData = null;

        if (slotUI != null && slotUI.Slot?.StoredItem != null)
        {
            selectedItemData = slotUI.Slot.StoredItem.ItemData;
        }
        else if (equipUI != null && GameManager.Instance.PlayerEquipment.GetEquipmentSlot(equipUI.SlotType)?.EquippedItem != null)
        {
            selectedItemData = GameManager.Instance.PlayerEquipment.GetEquipmentSlot(equipUI.SlotType).EquippedItem.ItemData;
        }

        if (selectedItemData == null)
        {
            Debug.LogWarning("No valid item found for configuration.");
            return;
        }

        // Configurează UI-ul
        var itemConfigUI = configWindow.GetComponent<ItemConfigUI>();
        itemConfigUI.SetItemData(selectedItemData);

        // Setează poziția ferestrei UI lângă mouse
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        RectTransform rectTransform = configWindow.GetComponent<RectTransform>();
        rectTransform.position = mousePosition;

        // Activează fereastra
        configWindow.SetActive(true);
        isConfigOpen = true;
    }

    private void CloseConfigWindow()
    {
        if (!isConfigOpen && !IsPointerOverConfigUI()) return;

        configWindow.SetActive(false);
        isConfigOpen = false;
        Debug.Log("Config window closed.");
    }
    private bool IsPointerOverConfigUI()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Mouse.current.position.ReadValue()
        };

        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(pointerData, results);

        foreach (var result in results)
        {
            if (result.gameObject == configWindow || result.gameObject.transform.IsChildOf(configWindow.transform))
            {
                return true; // Cursorul este peste fereastră
            }
        }
        return false; // Cursorul nu este peste fereastră
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
