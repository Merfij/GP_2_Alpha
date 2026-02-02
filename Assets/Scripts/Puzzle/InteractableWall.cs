using UnityEngine;
using UnityEngine.InputSystem;

public class InteractableWall : MonoBehaviour, IInteractable
{

    public PlayerController playerController;
    private PlayerInput playerInput;
    private Vector2 moveInput;
    public float boxMoveSpeed = 5f;
    public void Interact()
    {
        playerController.disableControls();
        playerInput.enabled = true;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerInput = this.GetComponent<PlayerInput>();
    }

    // Update is called once per frame
    void Update()
    {
        //move the box on x and y axis only depending on input
        Vector3 move = new Vector3(moveInput.x, moveInput.y, 0);
        transform.position += move * Time.deltaTime * boxMoveSpeed;
    }

    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }
}
