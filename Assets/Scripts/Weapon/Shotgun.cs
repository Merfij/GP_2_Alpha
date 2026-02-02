using System.Collections;
using UnityEngine;

public class Shotgun : MonoBehaviour
{

    [SerializeField] private float reloadTime = 0.5f;
    [Tooltip("Lower the value, faster the gun can shoot")]
    [SerializeField] private float fireRate = 0.31f;
    public int magSize = 2;
    [SerializeField] private int pelletPerShot = 6;
    [SerializeField] private int spreadStrength = 31;

    public GameObject pellet;
    public Transform bulletSpawn;
    public cameraShake camShake;
    public float shakeLength = 0.1f;
    public float shakeMagnitude = 0.2f;

    public int currentAmmo;
    private bool isReloading = false;
    private float reloadTimer = 0f;
    public Transform cam;
    int layerMask;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentAmmo = magSize;
        layerMask = ~LayerMask.GetMask("Trigger");
    }
    public void Shoot()
    {


        if (isReloading) return;
        if (Time.time < reloadTimer) return;

        if (currentAmmo <= 0)
        {
            StartCoroutine(Reload());
            GameEvents.OnSGReload?.Invoke();
            return;
        }
        
        reloadTimer = Time.time + fireRate;
        currentAmmo--;

        Vector3 aimPoint;

        // determine aim point ignoring the "Trigger" layer and any trigger colliders
        const float maxRange = 67f;
        if (Physics.Raycast(cam.position, cam.forward, out RaycastHit hit, maxRange, layerMask, QueryTriggerInteraction.Ignore))
            aimPoint = hit.point;
        else
            aimPoint = cam.position + cam.forward * maxRange;

        const float minMuzzleDist = 0.6f;
        Vector3 dir = (aimPoint - bulletSpawn.position);
        if (dir.sqrMagnitude < minMuzzleDist * minMuzzleDist)
        {
            aimPoint = bulletSpawn.position + cam.forward * minMuzzleDist;
            dir = (aimPoint - bulletSpawn.position);
        }

        for (int i = 0; i < pelletPerShot; i++)
        {
            Rigidbody bullet = Instantiate(pellet, bulletSpawn.position, Quaternion.identity).GetComponent<Rigidbody>();
            bullet.AddForce(dir.normalized * 531f, ForceMode.Impulse);
            GameEvents.OnSGShoot?.Invoke();

            bullet.AddForce(new Vector3(Random.Range(-spreadStrength, spreadStrength), Random.Range(-spreadStrength, spreadStrength), Random.Range(-spreadStrength, spreadStrength)), ForceMode.Impulse);

        }
        //camera shake
        StartCoroutine(camShake.Shake(shakeLength, shakeMagnitude));
    }

    IEnumerator Reload()
    {
        isReloading = true;
        //rotate gun in anim
        yield return new WaitForSeconds(reloadTime);
        
        currentAmmo = magSize;
        isReloading = false;
    }

    public void TryReload()
    {
        if (isReloading) return;
        if (currentAmmo == magSize) return;
        StartCoroutine(Reload());
    }
}
