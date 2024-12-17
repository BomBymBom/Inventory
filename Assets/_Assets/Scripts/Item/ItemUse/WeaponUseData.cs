using UnityEngine;

[CreateAssetMenu(fileName = "WeaponUseData", menuName = "ItemUse/Weapon")]
public class WeaponUseData : ItemUseData
{
    public override void Use(Item item, Character character)
    {
        Debug.Log($"{item.ItemName} weapon used. (Placeholder action)");
    }
}
