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

        int maxInventorySyze = 12;

        playerInventory = new Inventory(maxInventorySyze);

        playerEquipment = new EquipmentSystem();
    }


    private void Start()
    {
        itemSpawner?.InitializeSpawner();
    }


}
