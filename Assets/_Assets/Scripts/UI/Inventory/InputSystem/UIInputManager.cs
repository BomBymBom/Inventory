using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Manages input actions and delegates drag/drop or config logic to other managers.
/// </summary>
public class UIInputManager : MonoBehaviour
{
    [Header("Managers References")]
    [SerializeField] private DragDropManager dragDropManager;
    [SerializeField] private ConfigWindowManager configWindowManager;

    [Header("Input Settings")]
    [SerializeField] private PlayerInput inputActionsAsset;

    private InputAction pointAction;
    private InputAction leftClickAction;
    private InputAction rightClickAction;
    private InputAction openConfigAction;

    private void Awake()
    {
        // If not set in inspector, try to find them on the same GameObject
        if (!dragDropManager) dragDropManager = GetComponent<DragDropManager>();
        if (!configWindowManager) configWindowManager = GetComponent<ConfigWindowManager>();

        SetupInputActions();
    }

    private void OnDestroy()
    {
        CleanupInputActions();
    }

    private void SetupInputActions()
    {
        // Find UI action map
        var uiMap = inputActionsAsset.actions.FindActionMap("UI", true);

        // Find actions
        pointAction = uiMap.FindAction("Point", true);
        leftClickAction = uiMap.FindAction("Click", true);
        rightClickAction = uiMap.FindAction("RightClick", true);
        openConfigAction = uiMap.FindAction("MiddleClick", true);

        // Subscribe to events
        pointAction.performed += OnPointPerformed;
        leftClickAction.started += OnLeftClickStarted;
        leftClickAction.canceled += OnLeftClickCanceled;
        rightClickAction.started += OnRightClickPerformed;
        openConfigAction.started += OnOpenConfigStarted;

        // Enable actions
        pointAction.Enable();
        leftClickAction.Enable();
        rightClickAction.Enable();
        openConfigAction.Enable();
    }

    private void CleanupInputActions()
    {
        if (pointAction != null) pointAction.performed -= OnPointPerformed;
        if (leftClickAction != null)
        {
            leftClickAction.started -= OnLeftClickStarted;
            leftClickAction.canceled -= OnLeftClickCanceled;
        }
        if (rightClickAction != null) rightClickAction.started -= OnRightClickPerformed;
        if (openConfigAction != null) openConfigAction.started -= OnOpenConfigStarted;
    }

    // -------------------------------------------------------------------------
    //                             INPUT CALLBACKS
    // -------------------------------------------------------------------------
    private void OnPointPerformed(InputAction.CallbackContext context)
    {
        Vector2 pointerPos = context.ReadValue<Vector2>();
        dragDropManager.OnPointerMoved(pointerPos);
    }

    private void OnLeftClickStarted(InputAction.CallbackContext context)
    {
        if (configWindowManager.isConfigOpen && !configWindowManager.IsPointerOverConfigUI())
        {
            configWindowManager.CloseConfigWindow();
        }
        else if (configWindowManager.IsPointerOverConfigUI())
            return;

        dragDropManager.OnLeftClickStarted();
    }

    private void OnLeftClickCanceled(InputAction.CallbackContext context)
    {
        dragDropManager.OnLeftClickEnded();
    }

    private void OnRightClickPerformed(InputAction.CallbackContext context)
    {
        if (configWindowManager.isConfigOpen && !configWindowManager.IsPointerOverConfigUI())
        {
            configWindowManager.CloseConfigWindow();
        }
        else if (configWindowManager.IsPointerOverConfigUI())
            return;

        dragDropManager.OnRightClickPerformed();
    }

    private void OnOpenConfigStarted(InputAction.CallbackContext context)
    {
        configWindowManager.ToggleConfigWindow();
    }
}
