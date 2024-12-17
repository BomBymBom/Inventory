using UnityEngine;

[CreateAssetMenu(fileName = "ArmorUseData", menuName = "ItemUse/Armor")]
public class ArmorUseData : ItemUseData
{
    public override void Use(Item item, Character character)
    {
        Debug.Log($"{item.ItemName} armor equipped. (Placeholder action)");
    }
}
