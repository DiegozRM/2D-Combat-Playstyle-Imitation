using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputReader : MonoBehaviour
{
    [SerializeField] private bool showDebugMessages = true;

    [Header("Movement")]
    [SerializeField, Range(0.01f, 0.99f)]
    private float axisThreshold = 0.5f;

    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction attack1Action;
    private InputAction attack2Action;

    private bool jumpQueued;
    private bool attack1Queued;
    private bool attack2Queued;

    public Vector2 Move { get; private set; }

    private void Awake()
    {
        InputActionAsset actions = InputSystem.actions;

        if (actions == null)
        {
            Debug.LogError(
                "No project-wide Input System actions are configured.",
                this
            );
            enabled = false;
            return;
        }

        moveAction = actions.FindAction("Player/Move", true);
        jumpAction = actions.FindAction("Player/Jump", true);
        attack1Action = actions.FindAction("Player/Attack1", true);
        attack2Action = actions.FindAction("Player/Attack2", true);
    }

    private void OnEnable()
    {
        if (moveAction == null) return;

        moveAction.performed += OnMove;
        moveAction.canceled += OnMoveCanceled;

        jumpAction.performed += OnJump;
        attack1Action.performed += OnAttack1;
        attack2Action.performed += OnAttack2;
    }

    private void OnDisable()
    {
        if (moveAction == null) return;

        moveAction.performed -= OnMove;
        moveAction.canceled -= OnMoveCanceled;

        jumpAction.performed -= OnJump;
        attack1Action.performed -= OnAttack1;
        attack2Action.performed -= OnAttack2;

        Move = Vector2.zero;
        jumpQueued = false;
        attack1Queued = false;
        attack2Queued = false;
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        Vector2 rawMove = context.ReadValue<Vector2>();

        Move = new Vector2(
            ToDigitalAxis(rawMove.x),
            ToDigitalAxis(rawMove.y)
        );

        if (showDebugMessages)
            Debug.Log($"Move: {Move}", this);
    }

    private float ToDigitalAxis(float value)
    {
        if (value >= axisThreshold)
            return 1f;

        if (value <= -axisThreshold)
            return -1f;

        return 0f;
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        Move = Vector2.zero;

        if (showDebugMessages)
            Debug.Log("Move: no direction", this);
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        jumpQueued = true;

        if (showDebugMessages)
            Debug.Log("Jump pressed", this);
    }

    private void OnAttack1(InputAction.CallbackContext context)
    {
        attack1Queued = true;

        if (showDebugMessages)
            Debug.Log("Attack1 pressed", this);
    }

    private void OnAttack2(InputAction.CallbackContext context)
    {
        attack2Queued = true;

        if (showDebugMessages)
            Debug.Log("Attack2 pressed", this);
    }

    public bool ConsumeJump()
    {
        bool pressed = jumpQueued;
        jumpQueued = false;
        return pressed;
    }

    public bool ConsumeAttack1()
    {
        bool pressed = attack1Queued;
        attack1Queued = false;
        return pressed;
    }

    public bool ConsumeAttack2()
    {
        bool pressed = attack2Queued;
        attack2Queued = false;
        return pressed;
    }
}