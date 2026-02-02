using UnityEngine;

public class EtherialObject : MonoBehaviour
{
    [SerializeField] private GameObject solidForm;
    [SerializeField] private GameObject etherialEffect;
    private bool isSolid;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        solidForm.SetActive(false);
        etherialEffect.SetActive(true);
    }

    public void OnActivated()
    {
        if (isSolid) return;

        solidForm.SetActive(true);
        etherialEffect.SetActive(false);
        isSolid = true;
    }

    public void OnDeactivated()
    {
        if (!isSolid) return;

        solidForm.SetActive(false);
        etherialEffect.SetActive(true);
        isSolid = false;
    }
}
