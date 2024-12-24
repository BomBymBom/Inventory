using UnityEngine;

/// <summary>
/// Base class for any UI panel. It notifies the PanelManager when it's opened or closed.
/// </summary>
public class UIPanel : MonoBehaviour
{
    [SerializeField] protected PanelManager panelManager;

    protected bool isOpen = false;
    public bool IsOpen => isOpen;

    protected virtual void Awake()
    {
        // Panel is off by default (optional design choice)
        if (panelManager == null)
            panelManager = FindFirstObjectByType<PanelManager>();

        isOpen = false;
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Opens this panel, notifying the manager.
    /// </summary>
    public virtual void OpenPanel()
    {
        if (!isOpen)
        {
            isOpen = true;
            gameObject.SetActive(true);
            panelManager.OnPanelOpened(this);
        }
    }

    /// <summary>
    /// Closes this panel, notifying the manager.
    /// </summary>
    public virtual void ClosePanel()
    {
        if (isOpen)
        {
            isOpen = false;
            gameObject.SetActive(false);
            panelManager.OnPanelClosed(this);
        }
    }
}
