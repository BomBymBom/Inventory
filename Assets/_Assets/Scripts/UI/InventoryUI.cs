using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Responsible for displaying the Inventory data on the UI.
/// Subscribes to OnInventoryChanged and updates its visual slots accordingly.
/// </summary>
public class InventoryUI : MonoBehaviour
{
    [SerializeField] private Inventory playerInventory;
    [SerializeField] private Transform inventoryPanel; // A parent Transform that holds inventory slot UI elements
    [SerializeField] private GameObject slotUIPrefab; // A prefab for each inventory slot

    private List<InventorySlotUI> slotUIs = new List<InventorySlotUI>();

    private void OnEnable()
    {
        if (playerInventory != null)
        {
            playerInventory.OnInventoryChanged += UpdateUI;
        }
    }

    private void OnDisable()
    {
        if (playerInventory != null)
        {
            playerInventory.OnInventoryChanged -= UpdateUI;
        }
    }

    private void Start()
    {
        // Create UI slots depending on how many slots the Inventory has
        var slots = playerInventory.GetSlots();
        foreach (var slot in slots)
        {
            GameObject slotObj = Instantiate(slotUIPrefab, inventoryPanel);
            InventorySlotUI slotUI = slotObj.GetComponent<InventorySlotUI>();
            slotUI.SetSlotReference(slot);
            slotUIs.Add(slotUI);
        }

        UpdateUI();
    }

    /// <summary>
    /// Called when the inventory changes, updates the icons and counts for each slot.
    /// </summary>
    private void UpdateUI()
    {
        foreach (var slotUI in slotUIs)
        {
            slotUI.Refresh();
        }
    }
}
