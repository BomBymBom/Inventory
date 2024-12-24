using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

/// <summary>
/// Manages the currently open UI panels and switches to the "UI" action map
/// whenever at least one panel is open. Switches back to "Player" when all are closed.
/// Also exposes PlayerInput so panels can access it.
/// </summary>
public class PanelManager : MonoBehaviour
{
    [SerializeField] private PlayerInput playerInput;

    // Keep track of all panels that are currently open
    private HashSet<UIPanel> openPanels = new HashSet<UIPanel>();

    public PlayerInput PlayerInput => playerInput;

    private void Awake()
    {
        if (playerInput == null)
            playerInput = FindFirstObjectByType<PlayerInput>();

        playerInput.SwitchCurrentActionMap("Player");
        Cursor.lockState = CursorLockMode.Locked;

    }

    public void OnPanelOpened(UIPanel panel)
    {
        openPanels.Add(panel);
        if (openPanels.Count == 1)
        {
            // If it's the first panel, switch to UI map
            playerInput.SwitchCurrentActionMap("UI");
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void OnPanelClosed(UIPanel panel)
    {
        if (openPanels.Contains(panel))
        {
            openPanels.Remove(panel);
            if (openPanels.Count == 0)
            {
                // If no panels left open, switch back to Player map
                playerInput.SwitchCurrentActionMap("Player");
                Cursor.lockState = CursorLockMode.Locked;

            }
        }
    }
}
