using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

/// <summary>
/// Manages player interaction with items in range.
/// </summary>
public class PlayerInteractions : MonoBehaviour
{
    private PlayerInput playerInput;
    private InputAction interactAction;
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

    public void NotifyItemInRange(ItemPickup pickup)
    {
        if (!itemsInRange.Contains(pickup))
        {
            itemsInRange.Add(pickup);
            UpdateHighlight();
        }
    }

    public void NotifyItemOutOfRange(ItemPickup pickup)
    {
        if (itemsInRange.Contains(pickup))
        {
            pickup.SetHighlight(false);
            itemsInRange.Remove(pickup);
            UpdateHighlight();
        }
    }

    private void UpdateHighlight()
    {
        for (int i = 0; i < itemsInRange.Count; i++)
        {
            bool isHighlighted = (i == 0);
            itemsInRange[i].SetHighlight(isHighlighted);
        }
    }

    private void TryPickupCurrentItem()
    {
        if (itemsInRange.Count > 0)
        {
            itemsInRange[0].TryPickupItem();
        }
    }
}
