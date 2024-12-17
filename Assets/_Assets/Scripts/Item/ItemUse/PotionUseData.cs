using UnityEngine;

/// <summary>
/// Item usage data for a potion that restores health.
/// </summary>
[CreateAssetMenu(fileName = "PotionUseData", menuName = "ItemUse/Potion")]
public class PotionUseData : ItemUseData
{
    [SerializeField] private int healAmount = 20;

    public override void Use(Item item, Character character)
    {
        character.RestoreHealth(healAmount);
        Debug.Log($"{item.ItemName} used. Restored {healAmount} health.");
        // You might also want to remove one stack of this item from the inventory externally.
    }
}
