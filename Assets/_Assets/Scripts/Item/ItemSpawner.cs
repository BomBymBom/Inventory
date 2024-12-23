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

    [SerializeField] private ItemSpawnData[] itemsToSpawn; // Lista de obiecte care trebuie spaunate
    [SerializeField] private List<Transform> spawnPoints;  // Lista de puncte de spaunare

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

    /// <summary>
    /// Spaunează un item în scenă la locația specificată.
    /// </summary>
    public void SpawnItemInScene(ItemType type, int quantity, Transform spawnPoint)
    {
        // Creează un obiect folosind fabrica
        var item = GameManager.Instance.ItemFactory.CreateItem(type);
        if (item == null)
        {
            Debug.LogWarning($"Item of type {type} could not be created!");
            return;
        }

        // Generează prefab-ul asociat cu tipul de obiect
        var prefab = item.ItemPrefab; // Verificăm că prefab-ul este configurat corect
        if (prefab == null)
        {
            Debug.LogError($"No prefab assigned for {type} in ItemFactory!");
            return;
        }

        // Instanțiază prefab-ul în scenă
        var spawnedObject = Instantiate(prefab, spawnPoint.position, Quaternion.identity);
        var itemPickup = spawnedObject.GetComponent<ItemPickup>();
        if (itemPickup != null)
        {
            // Inițializează scriptul de pick-up cu tipul de obiect și cantitatea
            itemPickup.Initialize(item, quantity);
        }
        else
        {
            Debug.LogError($"Prefab {prefab.name} does not have an ItemPickup component!");
        }
    }

    /// <summary>
    /// Dropează un item pe jos la poziția specificată.
    /// </summary>
    public void DropItemOnGround(Item item, int quantity)
    {
        if (item.ItemPrefab == null)
        {
            Debug.LogError($"Item {item.ItemName} does not have a prefab assigned!");
            return;
        }

        Transform playerPosition = GameManager.Instance.PlayerCharacter.transform;
        Vector3 dropPosition = playerPosition.position + playerPosition.forward * 1f;
        // Instanțiază prefab-ul
        GameObject pickupObj = Instantiate(item.ItemPrefab, dropPosition, Quaternion.identity);

        // Configurează scriptul `ItemPickup` dacă există
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
}
