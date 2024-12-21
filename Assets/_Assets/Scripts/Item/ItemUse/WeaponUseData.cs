using UnityEngine;

[CreateAssetMenu(fileName = "WeaponUseData", menuName = "ItemUse/Weapon")]
public class WeaponUseData : ItemUseData
{
    public override void Use(Item item, Character character)
    {
        GameManager.Instance.PlayerEquipment.EquipItem(item);

    }
}
