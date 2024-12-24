using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemConfigUI : MonoBehaviour
{
    [SerializeField] private Toggle stackableToggle;
    [SerializeField] private TMP_InputField maxStackInput;

    private ItemData currentItemData;

    public void SetItemData(ItemData itemData)
    {
        currentItemData = itemData;

        stackableToggle.isOn = currentItemData.IsStackable;
        maxStackInput.text = currentItemData.MaxStackCount.ToString();
    }

    public void OnStackableChanged(bool value)
    {
        if (currentItemData == null) return;

        currentItemData.ConfigureStackable(value);
        if (!value)
        {
            currentItemData.ConfigureMaxStackCount(1);
            maxStackInput.text = "1";
        }

        GameManager.Instance.ItemSpawner.UpdateItems(currentItemData);
        GameManager.Instance.PlayerInventory.ReorganizeInventory(currentItemData.ItemType);
    }

    public void OnMaxStackChanged(string newValue)
    {
        if (currentItemData == null) return;

        if (int.TryParse(newValue, out int parsed))
        {
            currentItemData.ConfigureMaxStackCount(Mathf.Max(1, parsed));
        }

        GameManager.Instance.ItemSpawner.UpdateItems(currentItemData);
        GameManager.Instance.PlayerInventory.ReorganizeInventory(currentItemData.ItemType);
    }
}
