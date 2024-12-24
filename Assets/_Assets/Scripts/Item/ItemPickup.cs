using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [SerializeField] private GameObject highlightUI;

    private Item itemData;
    private int quantity;
    private PlayerInteractions playerInteractionManager;

    private void Start()
    {
        highlightUI.SetActive(false);
    }

    public void Initialize(Item item, int quantity)
    {
        this.itemData = item;
        this.quantity = quantity;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInteractionManager = other.GetComponent<PlayerInteractions>();
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
        if (itemData == null) return;

        bool added = GameManager.Instance.PlayerInventory.AddItem(itemData, quantity);
        if (added)
        {
            playerInteractionManager?.NotifyItemOutOfRange(this);
        }
        else
        {
            Debug.Log("Inventory is full, cannot pick up item.");
            playerInteractionManager?.NotifyItemOutOfRange(this);
        }

        Destroy(gameObject);
    }

    public void SetHighlight(bool active)
    {
        if (highlightUI != null)
        {
            highlightUI.SetActive(active);
        }
    }
}
