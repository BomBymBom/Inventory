using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

/// <summary>
/// Manages player interaction with items in range, highlighting only the first one.
/// </summary>
public class PlayerInteractionManager : MonoBehaviour
{
    private PlayerInput playerInput;
    private InputAction interactAction;

    // List to track all items in range, first one is the active item.
    private List<ItemPickup> itemsInRange = new List<ItemPickup>();

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        var uiMap = playerInput.actions.FindActionMap("Player", true);

        interactAction = uiMap.FindAction("Interact", true);
        interactAction.performed += OnInteractPerformed;

        interactAction.Enable();
    }

    private void OnDestroy()
    {
        if (interactAction != null)
        {
            interactAction.performed -= OnInteractPerformed;
        }
    }

    private void OnInteractPerformed(InputAction.CallbackContext context)
    {
        TryPickupCurrentItem();
    }

    /// <summary>
    /// Called when an item enters the interaction range.
    /// </summary>
    public void NotifyItemInRange(ItemPickup pickup)
    {
        if (!itemsInRange.Contains(pickup))
        {
            itemsInRange.Add(pickup);
            UpdateHighlight();
        }
    }

    /// <summary>
    /// Called when an item leaves the interaction range.
    /// </summary>
    public void NotifyItemOutOfRange(ItemPickup pickup)
    {
        if (itemsInRange.Contains(pickup))
        {
            pickup.SetHighlight(false);
            itemsInRange.Remove(pickup);
            UpdateHighlight();
        }
    }

    /// <summary>
    /// Updates the highlight state for the first item in range.
    /// </summary>
    private void UpdateHighlight()
    {
        for (int i = 0; i < itemsInRange.Count; i++)
        {
            bool isHighlighted = (i == 0);
            itemsInRange[i].SetHighlight(isHighlighted);
        }
    }

    /// <summary>
    /// Picks up the current item if available.
    /// </summary>
    private void TryPickupCurrentItem()
    {
        if (itemsInRange.Count > 0)
        {
            itemsInRange[0].TryPickupItem();
        }
    }
}
