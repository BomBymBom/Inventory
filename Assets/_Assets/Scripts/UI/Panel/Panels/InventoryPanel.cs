using UnityEngine.InputSystem;

public class InventoryPanel : UIPanel
{
    private InputAction toggleInventoryAction;

    protected override void Awake()
    {
        base.Awake();

        // Player map
        var playerMap = panelManager.PlayerInput.actions.FindActionMap("Player", true);
        var playerToggleAction = playerMap.FindAction("ToggleInventory", true);
        playerToggleAction.performed += OnToggleInventory;
        playerToggleAction.Enable();

        // UI map
        var uiMap = panelManager.PlayerInput.actions.FindActionMap("UI", true);
        var uiToggleAction = uiMap.FindAction("ToggleInventory", false);

        uiToggleAction.performed += OnToggleInventory;
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
