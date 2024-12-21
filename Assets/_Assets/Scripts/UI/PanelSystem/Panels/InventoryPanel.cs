using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// A concrete panel for the Inventory. It automatically listens for "ToggleInventory" 
/// in the "Player" action map and toggles itself on/off.
/// </summary>
public class InventoryPanel : UIPanel
{
    private InputAction toggleInventoryAction;

    protected override void Awake()
    {
        base.Awake();

        // *** Player map ***
        var playerMap = panelManager.PlayerInput.actions.FindActionMap("Player", true);
        var playerToggleAction = playerMap.FindAction("ToggleInventory", true);
        playerToggleAction.performed += OnToggleInventory;
        playerToggleAction.Enable();

        // *** UI map ***
        var uiMap = panelManager.PlayerInput.actions.FindActionMap("UI", true);
        var uiToggleAction = uiMap.FindAction("ToggleInventory", false);
        if (uiToggleAction != null)
        {
            uiToggleAction.performed += OnToggleInventory;
            uiToggleAction.Enable();
            uiMap?.Enable();
        }
    }

    private void OnDestroy()
    {
        if (toggleInventoryAction != null)
            toggleInventoryAction.performed -= OnToggleInventory;
    }

    private void OnToggleInventory(InputAction.CallbackContext ctx)
    {
        if (IsOpen)
            ClosePanel();
        else
            OpenPanel();
    }
}
