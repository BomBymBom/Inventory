using UnityEngine;

[CreateAssetMenu(fileName = "ArmorUseData", menuName = "ItemUse/Armor")]
public class ArmorUseData : ItemUseData
{
    public override void Use(Item item, Character character)
    {
        GameManager.Instance.PlayerEquipment.EquipItem(item);
    }
}
