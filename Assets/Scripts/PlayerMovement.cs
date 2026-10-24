using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float backwardSpeed = 2.5f;
    [SerializeField] private float rotationSpeed = 140f;

    [Header("Jump and Gravity")]
    [SerializeField] private float jumpHeight = 1.5f;
    [SerializeField] private float gravity = -20f;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    private CharacterController controller;

    private InputAction moveAction;
    private InputAction jumpAction;

    private Vector2 moveInput;
    private float verticalVelocity;
    private bool isGrounded;

    private static readonly int SpeedHash =
        Animator.StringToHash("Run");

    private static readonly int JumpHash =
        Animator.StringToHash("Jump");

    private static readonly int GroundedHash =
        Animator.StringToHash("IsGrounded");

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        CreateInputActions();
    }

    private void OnEnable()
    {
        moveAction.Enable();
        jumpAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
        jumpAction.Disable();
    }

    private void Update()
    {
        ReadInput();
        HandleMovement();
        UpdateAnimations();
    }

    private void CreateInputActions()
    {
        moveAction = new InputAction(
            name: "Move",
            type: InputActionType.Value
        );

        // Klaviatura: WASD
        moveAction.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/w")
            .With("Down", "<Keyboard>/s")
            .With("Left", "<Keyboard>/a")
            .With("Right", "<Keyboard>/d");

        // Klaviatura: strelkalar
        moveAction.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/upArrow")
            .With("Down", "<Keyboard>/downArrow")
            .With("Left", "<Keyboard>/leftArrow")
            .With("Right", "<Keyboard>/rightArrow");

        // Gamepad va mobile On-Screen Stick
        moveAction.AddBinding("<Gamepad>/leftStick");

        jumpAction = new InputAction(
            name: "Jump",
            type: InputActionType.Button
        );

        jumpAction.AddBinding("<Keyboard>/space");

        // Gamepad va mobile On-Screen Button
        jumpAction.AddBinding("<Gamepad>/buttonSouth");
    }

    private void ReadInput()
    {
        moveInput = moveAction.ReadValue<Vector2>();
    }

    private void HandleMovement()
    {
        isGrounded = controller.isGrounded;

        if (isGrounded && verticalVelocity < 0f)
        {
            // Playerni yerda barqaror ushlab turadi
            verticalVelocity = -2f;
        }

        if (isGrounded && jumpAction.WasPressedThisFrame())
        {
            verticalVelocity =
                Mathf.Sqrt(jumpHeight * -2f * gravity);

            if (animator != null)
            {
                animator.ResetTrigger(JumpHash);
                animator.SetTrigger(JumpHash);
            }
        }

        verticalVelocity += gravity * Time.deltaTime;

        // X qiymati chap va o‘ngga buradi
        float turnInput = moveInput.x;

        if (Mathf.Abs(turnInput) > 0.01f)
        {
            float rotationAmount =
                turnInput * rotationSpeed * Time.deltaTime;

            transform.Rotate(0f, rotationAmount, 0f);
        }

        // Y qiymati oldinga yoki orqaga yuradi
        float forwardInput = moveInput.y;

        float currentSpeed =
            forwardInput >= 0f
                ? moveSpeed
                : backwardSpeed;

        Vector3 movement =
            transform.forward * forwardInput * currentSpeed;

        movement.y = verticalVelocity;

        controller.Move(movement * Time.deltaTime);
    }


    private void UpdateAnimations()
    {
        if (animator == null)
            return;

        float movementAmount = Mathf.Abs(moveInput.y);

        animator.SetFloat(
            SpeedHash,
            movementAmount,
            0.1f,
            Time.deltaTime
        );

        animator.SetBool(
            GroundedHash,
            controller.isGrounded
        );
    }
}
