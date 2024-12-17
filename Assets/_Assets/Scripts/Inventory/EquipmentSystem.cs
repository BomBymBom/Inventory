using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the equipped items on a character. 
/// For example: a dictionary from ItemType (like Weapon, Armor) to a specific EquipmentSlot.
/// </summary>
public class EquipmentSystem
{
    public event Action OnEquipmentChanged;

    private Dictionary<ItemType, EquipmentSlot> equipmentSlots;

    public EquipmentSystem()
    {
        equipmentSlots = new Dictionary<ItemType, EquipmentSlot>();

        // For simplicity, let's say we have a few predefined slots:
        equipmentSlots[ItemType.Weapon] = new EquipmentSlot(ItemType.Weapon);
        equipmentSlots[ItemType.Armor] = new EquipmentSlot(ItemType.Armor);
    }

    public bool EquipItem(Item item)
    {
        if (equipmentSlots.ContainsKey(item.ItemType))
        {
            bool equipped = equipmentSlots[item.ItemType].EquipItem(item);
            if (equipped)
            {
                OnEquipmentChanged?.Invoke();
                return true;
            }
        }
        return false;
    }

    public Item UnequipItem(ItemType slotType)
    {
        if (equipmentSlots.ContainsKey(slotType))
        {
            Item removed = equipmentSlots[slotType].UnequipItem();
            if (removed != null)
            {
                OnEquipmentChanged?.Invoke();
            }
            return removed;
        }
        return null;
    }

    public EquipmentSlot GetEquipmentSlot(ItemType slotType)
    {
        if (equipmentSlots.ContainsKey(slotType))
        {
            return equipmentSlots[slotType];
        }
        return null;
    }
}
