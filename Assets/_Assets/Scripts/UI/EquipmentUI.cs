using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Displays the currently equipped items. 
/// Subscribes to OnEquipmentChanged from EquipmentSystem.
/// </summary>
public class EquipmentUI : MonoBehaviour
{
    [SerializeField] private EquipmentSystem equipmentSystem;
    [SerializeField] private Transform equipmentPanel; // parent transform for equipment slots
    [SerializeField] private GameObject equipmentSlotUIPrefab;

    private Dictionary<ItemType, EquipmentSlotUI> slotUIDict = new Dictionary<ItemType, EquipmentSlotUI>();

    private void OnEnable()
    {
        if (equipmentSystem != null)
        {
            equipmentSystem.OnEquipmentChanged += UpdateUI;
        }
    }

    private void OnDisable()
    {
        if (equipmentSystem != null)
        {
            equipmentSystem.OnEquipmentChanged -= UpdateUI;
        }
    }

    private void Start()
    {
        // Assume we know which ItemTypes we have. For simplicity, let's say we have Weapon and Armor slots.
        CreateEquipmentSlotUI(ItemType.Weapon);
        CreateEquipmentSlotUI(ItemType.Armor);

        UpdateUI();
    }

    private void CreateEquipmentSlotUI(ItemType slotType)
    {
        GameObject slotObj = Instantiate(equipmentSlotUIPrefab, equipmentPanel);
        EquipmentSlotUI slotUI = slotObj.GetComponent<EquipmentSlotUI>();
        slotUI.SetSlotType(slotType);
        slotUIDict[slotType] = slotUI;
    }

    private void UpdateUI()
    {
        foreach (var kvp in slotUIDict)
        {
            var slotType = kvp.Key;
            var slotUI = kvp.Value;
            var equipmentSlot = equipmentSystem.GetEquipmentSlot(slotType);
            slotUI.Refresh(equipmentSlot);
        }
    }
}
