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
    [SerializeField] private ItemSpawner itemSpawner;

    public Character PlayerCharacter => playerCharacter;
    public Inventory PlayerInventory => playerInventory;
    public EquipmentSystem PlayerEquipment => playerEquipment;
    public ItemFactory ItemFactory => itemFactory;
    public ItemSpawner ItemSpawner => itemSpawner;

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


    private void Start()
    {
        itemSpawner?.InitializeSpawner();
    }


}
