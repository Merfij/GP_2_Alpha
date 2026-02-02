using UnityEngine;

public class SlideDoors : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private bool isOpen = false;

    private void Start()
    {
        animator.SetBool("Open", false);
    }

    public void Open()
    {
        if(isOpen) return;

        animator.SetBool("Open", true);
        isOpen = true;
        Debug.Log("Opening");
    }

    public void Close()
    {
        if(!isOpen) return;

        animator.SetBool("Open", false);
        isOpen = false;
        Debug.Log("Closing");
    }
}
