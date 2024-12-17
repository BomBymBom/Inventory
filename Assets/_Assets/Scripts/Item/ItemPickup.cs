using UnityEngine;

/// <summary>
/// Represents an item lying on the ground that can be picked up by the player.
/// Attach this script to a GameObject with a collider (trigger).
/// </summary>
public class ItemPickup : MonoBehaviour
{
    [SerializeField] private ItemType itemType;
    [SerializeField] private int quantity = 1;

    private Item item;
    private bool playerInRange = false;

    private void Start()
    {
        // Create the item using the ItemFactory
        item = GameManager.Instance.ItemFactory.CreateItem(itemType);
    }

    private void OnTriggerEnter(Collider other)
    {
        // If the player enters the trigger range
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // If the player leaves the trigger range
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    /// <summary>
    /// Call this method when the player presses the interaction button (e.g., 'E') while in range.
    /// </summary>
    public void TryPickupItem()
    {
        if (playerInRange && item != null)
        {
            bool added = GameManager.Instance.PlayerInventory.AddItem(item, quantity);
            if (added)
            {
                // Item added successfully, remove from world
                Destroy(gameObject);
            }
            else
            {
                Debug.Log("Inventory is full, cannot pick up item.");
            }
        }
    }
}
