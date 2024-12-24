using UnityEngine;

/// <summary>
/// Base class for any UI panel. Notifies the PanelManager when it's opened or closed.
/// </summary>
public class UIPanel : MonoBehaviour
{
    [SerializeField] protected PanelManager panelManager;

    protected bool isOpen = false;
    public bool IsOpen => isOpen;

    protected virtual void Awake()
    {
        if (panelManager == null)
            panelManager = FindFirstObjectByType<PanelManager>();

        isOpen = false;
        gameObject.SetActive(false);
    }

    public virtual void OpenPanel()
    {
        if (!isOpen)
        {
            isOpen = true;
            gameObject.SetActive(true);
            panelManager.OnPanelOpened(this);
        }
    }

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
