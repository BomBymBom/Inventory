using UnityEngine;

/// <summary>
/// Represents an item that can be picked up by the player.
/// Includes logic for managing its highlight UI.
/// </summary>
public class ItemPickup : MonoBehaviour
{
    [SerializeField] private ItemType itemType;
    [SerializeField] private int quantity = 1;
    [SerializeField] private GameObject highlightUI;

    private PlayerInteractionManager playerInteractionManager;

    private void Start()
    {
        // Create the item using the ItemFactory
        var item = GameManager.Instance.ItemFactory.CreateItem(itemType);

        // Instantiate the highlight UI, but keep it inactive
        highlightUI.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInteractionManager = other.GetComponent<PlayerInteractionManager>();
            if (playerInteractionManager != null)
            {
                playerInteractionManager.NotifyItemInRange(this);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (playerInteractionManager != null)
            {
                playerInteractionManager.NotifyItemOutOfRange(this);
                playerInteractionManager = null;
            }
        }
    }

    public void TryPickupItem()
    {
        // Player tries to pick up the item
        bool added = GameManager.Instance.PlayerInventory.AddItem(GameManager.Instance.ItemFactory.CreateItem(itemType), quantity);
        if (added)
        {
            playerInteractionManager.NotifyItemOutOfRange(this);
            playerInteractionManager = null;
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("Inventory is full, cannot pick up item.");
        }
    }

    public void SetHighlight(bool active)
    {
        if (highlightUI != null)
        {
            highlightUI.SetActive(active);
        }
    }
}
