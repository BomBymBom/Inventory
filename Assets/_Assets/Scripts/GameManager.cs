using UnityEngine;

/// <summary>
/// Central manager providing global access to PlayerInventory, PlayerEquipment, PlayerCharacter, and utility functions.
/// Attach this to a GameObject in the scene. 
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private Character playerCharacter;
    [SerializeField] private Inventory playerInventory;
    [SerializeField] private EquipmentSystem playerEquipment;
    [SerializeField] private ItemFactory itemFactory;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // If playerInventory, playerEquipment, or itemFactory are not assigned in the Inspector,
        // we could create defaults here. For example:
        if (playerInventory == null)
        {
            playerInventory = new Inventory(12); // 12 slots as an example
        }

        if (playerEquipment == null)
        {
            playerEquipment = new EquipmentSystem();
        }

        // itemFactory should be assigned in inspector. If not, you could find it in the scene:
        // itemFactory = FindObjectOfType<ItemFactory>();
    }

    public Character PlayerCharacter => playerCharacter;
    public Inventory PlayerInventory => playerInventory;
    public EquipmentSystem PlayerEquipment => playerEquipment;
    public ItemFactory ItemFactory => itemFactory;

    /// <summary>
    /// Drops the given item on the ground. In a real scenario, 
    /// this would Instantiate an ItemPickup prefab at player’s position.
    /// </summary>
    public void DropItemOnGround(Item item, int quantity)
    {
        if (item.ItemPrefab == null)
        {
            Debug.LogError($"Item {item.ItemName} does not have a prefab assigned!");
            return;
        }

        // Poziția de drop: puțin în fața jucătorului
        Vector3 dropPosition = playerCharacter.transform.position + playerCharacter.transform.forward * 1f;

        // Instanțiază prefab-ul
        GameObject pickupObj = Instantiate(item.ItemPrefab, dropPosition, Quaternion.identity);

        // Configurează scriptul `ItemPickup` dacă există
        ItemPickup itemPickup = pickupObj.GetComponent<ItemPickup>();
        if (itemPickup != null)
        {
            itemPickup.Initialize(item, quantity);
        }
    }
    public void SpawnItemInScene(ItemType type, int quantity, Transform position)
    {
        // Creează un obiect folosind fabrica
        var item = ItemFactory.CreateItem(type);
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
        var spawnedObject = Instantiate(prefab, position.position, Quaternion.identity);
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


}
