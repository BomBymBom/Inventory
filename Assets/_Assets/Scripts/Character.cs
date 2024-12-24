
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
    private int Health = 20;
    private int MaxHealth = 100;

    [Header("UI Elements")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TMP_Text healthText;

    [Header("Equipment")]
    [SerializeField] private EquipmentSystem equipmentSystem;
    [SerializeField] private GameObject weapon;
    [SerializeField] private GameObject armor;
    private void OnEnable()
    {
        if (equipmentSystem == null)
            equipmentSystem = GameManager.Instance.PlayerEquipment;

        if (equipmentSystem != null)
        {
            equipmentSystem.OnEquipmentChanged += UpdateModel;
        }
        UpdateHealthUI();
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
        var weaponSlot = equipmentSystem.GetEquipmentSlot(ItemType.Weapon);
        if (weaponSlot != null && weaponSlot.EquippedItem != null)
        {
            weapon.SetActive(true);
        }
        else
        {
            weapon.SetActive(false);
        }

        var armorSlot = equipmentSystem.GetEquipmentSlot(ItemType.Armor);
        if (armorSlot != null && armorSlot.EquippedItem != null)
        {
            armor.SetActive(true);
        }
        else
        {
            armor.SetActive(false);
        }
    }
    public void RestoreHealth(int amount)
    {
        Health = Mathf.Min(Health + amount, MaxHealth);
        UpdateHealthUI();
        Debug.Log("Character healed, current health: " + Health);
    }
    private void UpdateHealthUI()
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = MaxHealth;
            healthSlider.value = Health;
        }

        if (healthText != null)
        {
            healthText.text = $"{Health} / {MaxHealth}";
        }
    }
}
