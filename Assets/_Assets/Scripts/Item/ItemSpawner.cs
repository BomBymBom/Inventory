using UnityEngine;
using System.Collections.Generic;

public class ItemSpawner : MonoBehaviour
{
    [System.Serializable]
    public struct ItemSpawnData
    {
        public ItemType type;
        public int quantity;
    }

    [SerializeField] private ItemSpawnData[] itemsToSpawn;
    [SerializeField] private List<Transform> spawnPoints;
    [SerializeField] private Transform parent;
    private List<Item> allItems = new List<Item>();
    public void InitializeSpawner()
    {
        List<Transform> availablePoints = new List<Transform>(spawnPoints);

        foreach (var itemData in itemsToSpawn)
        {
            if (availablePoints.Count == 0)
            {
                Debug.LogWarning("No more available spawn points. Skipping remaining items.");
                break;
            }

            int randomIndex = Random.Range(0, availablePoints.Count);
            Transform spawnPoint = availablePoints[randomIndex];
            availablePoints.RemoveAt(randomIndex);

            SpawnItemInScene(itemData.type, itemData.quantity, spawnPoint);
        }
    }

    public void SpawnItemInScene(ItemType type, int quantity, Transform spawnPoint)
    {
        var item = GameManager.Instance.ItemFactory.CreateItem(type);
        if (item == null)
        {
            Debug.LogWarning($"Item of type {type} could not be created!");
            return;
        }

        allItems.Add(item);

        var prefab = item.ItemPrefab;
        if (prefab == null)
        {
            Debug.LogError($"No prefab assigned for {type} in ItemFactory!");
            return;
        }

        var spawnedObject = Instantiate(prefab, spawnPoint.position, Quaternion.identity, parent);
        var itemPickup = spawnedObject.GetComponent<ItemPickup>();
        if (itemPickup != null)
        {
            itemPickup.Initialize(item, quantity);
        }
        else
        {
            Debug.LogError($"Prefab {prefab.name} does not have an ItemPickup component!");
        }
    }


    public void DropItemOnGround(Item item, int quantity)
    {
        if (item.ItemPrefab == null)
        {
            Debug.LogError($"Item {item.ItemName} does not have a prefab assigned!");
            return;
        }

        Transform playerPosition = GameManager.Instance.PlayerCharacter.transform;
        Vector3 dropPosition = playerPosition.position + playerPosition.forward * 1f;

        GameObject pickupObj = Instantiate(item.ItemPrefab, dropPosition, Quaternion.identity);

        ItemPickup itemPickup = pickupObj.GetComponent<ItemPickup>();
        if (itemPickup != null)
        {
            itemPickup.Initialize(item, quantity);
        }
        else
        {
            Debug.LogError($"Prefab {item.ItemPrefab.name} does not have an ItemPickup component!");
        }
    }

    public void UpdateItems(ItemData updatedData)
    {
        foreach (var item in allItems)
        {
            if (item.ItemType == updatedData.ItemType)
            {
                item.UpdateFromItemData(updatedData);
            }
        }
    }
}
