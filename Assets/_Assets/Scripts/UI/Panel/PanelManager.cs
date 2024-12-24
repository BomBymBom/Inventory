using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

/// <summary>
/// Manages the currently open UI panels and switches to the "UI" action map
/// </summary>
public class PanelManager : MonoBehaviour
{
    [SerializeField] private PlayerInput playerInput;
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
                playerInput.SwitchCurrentActionMap("Player");
                Cursor.lockState = CursorLockMode.Locked;

            }
        }
    }
}
