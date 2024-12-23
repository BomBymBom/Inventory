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

    private void Start()
    {
        // Asigură-te că avem suficiente puncte de spawn pentru obiectele din lista itemsToSpawn
        if (itemsToSpawn.Length > spawnPoints.Count)
        {
            Debug.LogWarning("Not enough spawn points for the number of items to spawn. Some items will not be spawned.");
        }

        List<Transform> availablePoints = new List<Transform>(spawnPoints);

        foreach (var itemData in itemsToSpawn)
        {
            if (availablePoints.Count == 0)
            {
                Debug.LogWarning("No more available spawn points. Skipping remaining items.");
                break;
            }

            // Selectează un punct de spawn aleatoriu din cele disponibile
            int randomIndex = Random.Range(0, availablePoints.Count);
            Transform spawnPoint = availablePoints[randomIndex];
            availablePoints.RemoveAt(randomIndex); // Elimină punctul din lista disponibilă

            // Spaunează obiectul
            GameManager.Instance.SpawnItemInScene(itemData.type, itemData.quantity, spawnPoint);
        }
    }
}
