using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Mouse")]
    public float MouseSensivity = 100f;
    public float topClamp = -90f;
    public float bottomClamp = 90f;
    public Transform camPivot;

    [Header("Movement")]
    public float moveSpeed = 3.0f;
    public float jumpHeight = 1.5f;
    public float timeToJumpApex = 1;

    [Header("Jumping")]
    [Tooltip("How long (in seconds) after leaving the ground the player is still allowed to jump (coyote time).")]
    [SerializeField] private float coyoteTime = 0.2f;
    private float coyoteTimer = 0f;

    [Header("Interact")]
    [SerializeField] private float interactRange = 3f;
    [SerializeField] private LayerMask interactMask = ~0; // everything by default
    [SerializeField] private bool drawDebugRay = true;
    private bool interactHeld = false;

    private float _xRotation = 0f;
    private float _yRotation = 0f;
    private float gravity = -9.81f;
    private float jumpVelocity;

    // Catalin audio test

    public bool isWalking = false;


    private bool groundedPlayer;
    private Vector2 mouseInput;
    private Vector2 moveInput;
    private bool jump = false;
    private bool laser = false;
    private float _baseMoveSpeed;

    private Vector3 playerVelocity;
    private CharacterController _characterController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void OnEnable()
    {
        StaticVariables.PlayerCount++;
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _characterController = GetComponent<CharacterController>();

        /*if (StaticVariables.PlayerCount > 1)
        {
            AudioListener camListen = GetComponentInChildren<AudioListener>();
            camListen.enabled = false;
        }*/

        //calculate jumpVelocity and gravity
        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;


        _baseMoveSpeed = moveSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        //mouse stuff
        float mouseX = mouseInput.x * MouseSensivity * Time.deltaTime;
        float mouseY = mouseInput.y * MouseSensivity * Time.deltaTime;

        _xRotation -= mouseY;
        _xRotation = Mathf.Clamp(_xRotation, topClamp, bottomClamp);
        _yRotation += mouseX;

        transform.localRotation = Quaternion.Euler(0f, _yRotation, 0f);
        camPivot.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);

        groundedPlayer = _characterController.isGrounded;

        // --- COYOTE TIME: refresh or decrement timer depending on grounded state ---
        if (groundedPlayer)
        {
            coyoteTimer = coyoteTime;
        }
        else
        {
            coyoteTimer -= Time.deltaTime;
        }

        //movement stuff
        if (groundedPlayer)
        {
            // Slight downward velocity to keep grounded stable
            if (playerVelocity.y < -2f)
                playerVelocity.y = -2f;

            if (moveInput.x > 0f || moveInput.y > 0f)
            {
                isWalking = true;
            }
            else isWalking = false;
        }

        // Read input
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        move = Vector3.ClampMagnitude(move, 1f);

        // Jump
        // Allow jump if grounded OR within coyote time after leaving ground
        if ((groundedPlayer || coyoteTimer > 0f) && jump && !laser)
        {
            playerVelocity.y = jumpVelocity;

            // consume coyote time to avoid double use
            coyoteTimer = 0f;

            if (gameObject.name == "Exorcist(Clone)")
            {
                GameEvents.OnExorcistJump?.Invoke();
            }
            else if (gameObject.name == "Demon(Clone)")
            {
                GameEvents.OnDemonJump?.Invoke();
            }

            jump = false;
        }

        // Apply gravity
        playerVelocity.y += gravity * Time.deltaTime;

        // Move
        Vector3 finalMove = move * moveSpeed + Vector3.up * playerVelocity.y;
        _characterController.Move(finalMove * Time.deltaTime);

        //if we reach the end of update and the jump button is true
        //then we pressed jump while not grounded, we can cancel the jump
        //so they don't bounce on the floor unintentionally
        jump = false;

    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    public void OnLook(InputValue value)
    {
        mouseInput = value.Get<Vector2>();
    }

    public void OnJump(InputValue value)
    {
        jump = value.isPressed;
    }

    public void OnInteract(InputValue value)
    {
        bool pressed = value.isPressed;

        if (pressed && !interactHeld)
        {
            TryInteractRaycast();
        }

        interactHeld = pressed;
    }
    void OnLaser(InputValue button)
    {
        bool pressed = button.isPressed;
        if (pressed)
        {
            moveSpeed = _baseMoveSpeed * 0.5f;
            laser = true;
        }
        else
        {
            moveSpeed = _baseMoveSpeed;
            laser = false;
        }
    }

    //public void OnLevel2(InputValue value)
    //...
    // (unchanged commented code omitted for brevity)
    //...

    private void TryInteractRaycast()
    {
        Vector3 origin = camPivot.position;
        Vector3 direction = camPivot.forward;

        if (drawDebugRay)
            Debug.DrawRay(origin, direction * interactRange, Color.cyan, 0.25f);

        if (Physics.Raycast(origin, direction, out RaycastHit hit, interactRange, interactMask, QueryTriggerInteraction.Ignore))
        {
            var interactable = hit.collider.GetComponentInParent<IInteractable>();
            if (interactable != null)
            {
                interactable.Interact();
            }
            else
            {
                Debug.Log($"{name} hit {hit.collider.name} but it has no IInteractable.", hit.collider.gameObject);
            }
        }
    }

    //method to disable and enable the character controller, so that players can control other things without moving the character
    public void disableControls()
    {
        this.GetComponent<PlayerController>().enabled = false;
        this.GetComponent<ExorcistAbilities>().enabled = false;
        this.GetComponent<PlayerInput>().enabled = false;
        this.GetComponent<CharacterController>().enabled = false;

    }
}