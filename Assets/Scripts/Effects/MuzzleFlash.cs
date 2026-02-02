using System.Collections;
using UnityEngine;

public class MuzzleFlash : MonoBehaviour
{
    [SerializeField] GameObject muzzleFlashPrefab;
    public bool IsShotgun = false;
    private void OnEnable()
    {
        if (!IsShotgun)
            GameEvents.OnMGShoot += Flash;
        else
            GameEvents.OnSGShoot += Flash;
    }

    private void OnDisable()
    {
        if(!IsShotgun)
            GameEvents.OnMGShoot -= Flash;
        else
            GameEvents.OnSGShoot -= Flash;
    }
    void Flash()
    {
        StartCoroutine("Unflash");
        muzzleFlashPrefab.transform.Rotate(new Vector3(1,0,0), Random.Range(0, 90));
        muzzleFlashPrefab.SetActive(true);
    }

    void FlashDisable()
    {
        muzzleFlashPrefab.SetActive(false);
    }

    IEnumerator Unflash()
    {
        yield return new WaitForSeconds(0.05f);
        FlashDisable();
    }

}
