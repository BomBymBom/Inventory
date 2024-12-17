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
    public void DropItemOnGround(Item item)
    {
        // Placeholder logic
        Debug.Log($"Dropped {item.ItemName} on the ground.");
        // Example:
        // Vector3 dropPosition = PlayerCharacter.transform.position + PlayerCharacter.transform.forward * 1f;
        // Instantiate(itemPickupPrefab, dropPosition, Quaternion.identity).SetItem(item);
    }
}
