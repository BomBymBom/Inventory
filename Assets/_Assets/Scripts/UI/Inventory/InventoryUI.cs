using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private Inventory playerInventory;
    [SerializeField] private Transform inventoryPanel;
    [SerializeField] private GameObject slotUIPrefab;

    private List<InventorySlotUI> slotUIs = new List<InventorySlotUI>();

    private void OnEnable()
    {
        if (playerInventory != null)
        {
            playerInventory.OnInventoryChanged += UpdateUI;
        }
        UpdateUI();
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
        if (playerInventory == null && GameManager.Instance != null)
        {
            playerInventory = GameManager.Instance.PlayerInventory;

            playerInventory.OnInventoryChanged += UpdateUI;
        }
        // Creăm sloturile UI în funcție de câte sloturi are inventarul
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

    // UpdateUI va reîmprospăta toate sloturile UI
    public void UpdateUI()
    {
        foreach (var slotUI in slotUIs)
        {
            slotUI.Refresh();
        }
    }
}
