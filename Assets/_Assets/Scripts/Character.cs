
using UnityEngine;

public class Character : MonoBehaviour
{
    public int Health { get; set; }
    public int MaxHealth { get; set; }

    [Header("Equipment")]
    [SerializeField] private EquipmentSystem equipmentSystem;
    // Referințe la game object-ul personajului
    [SerializeField] private GameObject weaponMesh;
    [SerializeField] private GameObject armorMesh;
    private void OnEnable()
    {
        if (equipmentSystem == null)
            equipmentSystem = GameManager.Instance.PlayerEquipment;

        if (equipmentSystem != null)
        {
            Debug.Log(1111);
            equipmentSystem.OnEquipmentChanged += UpdateModel;
        }
    }

    private void OnDisable()
    {
        if (equipmentSystem != null)
        {
            equipmentSystem.OnEquipmentChanged -= UpdateModel;
        }
    }

    private void UpdateModel()
    {
        // 1. Verificăm item-ul echipat la slot-ul "Weapon"
        var weaponSlot = equipmentSystem.GetEquipmentSlot(ItemType.Weapon);
        if (weaponSlot != null && weaponSlot.EquippedItem != null)
        {
            weaponMesh.SetActive(true);
            // Sau schimbi mesh / sprite
        }
        else
        {
            weaponMesh.SetActive(false);
        }

        // 2. Verificăm item-ul echipat la slot-ul "Armor"
        var armorSlot = equipmentSystem.GetEquipmentSlot(ItemType.Armor);
        if (armorSlot != null && armorSlot.EquippedItem != null)
        {
            armorMesh.SetActive(true);
            // ... alte schimbări, gen material, mesh etc.
        }
        else
        {
            armorMesh.SetActive(false);
        }
    }
    public void RestoreHealth(int amount)
    {
        Health = Mathf.Min(Health + amount, MaxHealth);
        Debug.Log("Character healed, current health: " + Health);
    }
}
