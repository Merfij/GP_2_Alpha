using UnityEngine;

public class SlidingPlatform : MonoBehaviour
{
    [SerializeField] Animator animator;

    private bool isOut;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isOut = false;
    }

    public void SlideOut()
    {
        if (isOut) return;

        animator.SetBool("slideOut", true);
        GameEvents.OnPlatformSlide?.Invoke(transform);
        isOut = true;
    }

    public void SlideIn()
    {
        if (!isOut) return;

        animator.SetBool("slideOut", false);
        GameEvents.OnPlatformSlide?.Invoke(transform);
        isOut = false;
    }
}
