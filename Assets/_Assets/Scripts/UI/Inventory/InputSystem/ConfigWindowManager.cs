using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Handles displaying/hiding the config window and updating its data.
/// </summary>
public class ConfigWindowManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject configWindow;
    [SerializeField] private GraphicRaycaster raycaster;

    public bool isConfigOpen { get; private set; }

    public void ToggleConfigWindow()
    {
        if (isConfigOpen)
        {
            CloseConfigWindow();
            return;
        }

        Vector2 mousePos = Mouse.current.position.ReadValue();
        AttemptOpenConfigAtPosition(mousePos);
    }

    private void AttemptOpenConfigAtPosition(Vector2 pointerPos)
    {
        // Raycast for InventorySlotUI or EquipmentSlotUI
        InventorySlotUI slotUI = RaycastFor<InventorySlotUI>(pointerPos);
        EquipmentSlotUI equipUI = (slotUI == null)
            ? RaycastFor<EquipmentSlotUI>(pointerPos)
            : null;

        if (slotUI == null && equipUI == null)
        {
            Debug.LogWarning("No item selected for configuration.");
            return;
        }

        // Extract ItemData
        ItemData selectedItemData = GetSelectedItemData(slotUI, equipUI);
        if (selectedItemData == null)
        {
            Debug.LogWarning("No valid item found for configuration.");
            return;
        }

        // Position the config window near the mouse
        RectTransform rt = configWindow.GetComponent<RectTransform>();
        rt.position = pointerPos;

        // Apply data
        var itemConfigUI = configWindow.GetComponent<ItemConfigUI>();
        itemConfigUI.SetItemData(selectedItemData);

        configWindow.SetActive(true);
        isConfigOpen = true;
    }

    public void CloseConfigWindow()
    {
        if (!isConfigOpen && !IsPointerOverConfigUI()) return;

        configWindow.SetActive(false);
        isConfigOpen = false;
        Debug.Log("Config window closed.");
    }


    #region HelperMethods
    private ItemData GetSelectedItemData(InventorySlotUI slotUI, EquipmentSlotUI equipUI)
    {
        if (slotUI != null && slotUI.Slot?.StoredItem != null)
        {
            return slotUI.Slot.StoredItem.ItemData;
        }
        else if (equipUI != null)
        {
            var eqSlot = GameManager.Instance.PlayerEquipment.GetEquipmentSlot(equipUI.SlotType);
            return eqSlot?.EquippedItem?.ItemData;
        }
        return null;
    }

    public bool IsPointerOverConfigUI()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Mouse.current.position.ReadValue()
        };

        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(pointerData, results);

        foreach (var result in results)
        {
            if (result.gameObject == configWindow ||
                result.gameObject.transform.IsChildOf(configWindow.transform))
            {
                return true;
            }
        }
        return false;
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
                return comp;
        }
        return null;
    }
    #endregion
}
