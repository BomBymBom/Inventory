using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles player interactions with items on the ground using PlayerInput.
/// </summary>
public class PlayerInteractionManager : MonoBehaviour
{
    [SerializeField] private float interactionDistance = 2f;
    private PlayerInput playerInput;
    private InputAction interactAction;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        var uiMap = playerInput.actions.FindActionMap("UI", true);
        // Presupunem că există o acțiune "Interact" în action map-ul "UI"
        interactAction = uiMap.FindAction("Interact", true);
        interactAction.Enable();
    }

    private void Update()
    {
        if (interactAction.triggered)
        {
            TryInteract();
        }
    }

    private void TryInteract()
    {
        // Raycast în fața jucătorului sau în zona în care poate fi itemul
        // Presupunem că player-ul are un transform și privim în direcția forward
        Ray ray = new Ray(transform.position, transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance))
        {
            ItemPickup pickup = hit.collider.GetComponent<ItemPickup>();
            if (pickup != null)
            {
                pickup.TryPickupItem();
            }
        }
    }
}
