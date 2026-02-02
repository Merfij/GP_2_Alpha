using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerInteraction : MonoBehaviour
{
    public float interactDistance = 3f;
    private bool interactPressed;

    public void OnInteraction(InputAction.CallbackContext context)
    {
        if (context.performed)
            interactPressed = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (interactPressed)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, interactDistance))
            {
                IInteractable interactable = hit.collider.GetComponent<IInteractable>();
                if (interactable != null)
                {
                    interactable.Interact();
                }
            }
        }
       
    }
}
