using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class Minigun : MonoBehaviour
{
    [SerializeField] GameObject barrel;
    [SerializeField] private float reloadTime = 0.5f;
    [Tooltip("Lower the value, faster the gun can shoot")]
    [SerializeField] private float fireRate = 0.31f;
    public int magSize = 2;
    [SerializeField] private int spreadStrength = 10;

    public GameObject pellet;
    public Transform bulletSpawn;
    public Transform cam;

    public int currentAmmo;
    private bool isReloading = false;
    private float reloadTimer = 0f;
    int layerMask;

    [Header("Wind-up")]
    [SerializeField] private float windUpTime = 0.4f;
    [SerializeField] private float windUpGracePeriod = 0.25f;

    private bool isWoundUp = false;
    private float windUpStartTime = -1f;
    private float lastShootAttemptTime = -1f;

    bool isShooting = false;
    float RotationSpeed = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentAmmo = magSize;
        layerMask = ~LayerMask.GetMask("Trigger");
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        // If Shoot() hasn't been called recently, reset wind-up
        if (isWoundUp && Time.time - lastShootAttemptTime > windUpGracePeriod)
        {
            isWoundUp = false;
            windUpStartTime = -1f;
            
        }
        //Alex trash
        if (isShooting && !isReloading)
        {
            RotationSpeed += (1f / windUpTime) * Time.deltaTime;
            if (RotationSpeed > 1) RotationSpeed = 1;
            isShooting = false;
        }
        else
        {
            RotationSpeed -= (1f / windUpTime) * Time.deltaTime;
            if (RotationSpeed < 0) RotationSpeed = 0;
        }
        Rotate();
        //
    }

    //Alex code
    //Barrel rotation
    void Rotate()
    {
        barrel.transform.Rotate(new Vector3(0,1,0), 402f * Time.deltaTime * RotationSpeed);
    }
    //

    public void Shoot()
    {
        //Alex Garbage
        isShooting = true;
        //
        float now = Time.time;

        // grace period bool
        lastShootAttemptTime = now;

        // not wound up
        if (!isWoundUp)
        {
            // Start wind-up if not already started
            if (windUpStartTime < 0f)
            {
                windUpStartTime = now;
                return;
            }

            // Still winding up
            if (now - windUpStartTime < windUpTime)
                return;

            // Wind-up complete
            isWoundUp = true;
        }

        if (isReloading) return;
        if (Time.time < reloadTimer) return;

        if (currentAmmo <= 0)
        {
            StartCoroutine(Reload());
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

        // Spawn + shoot
        Rigidbody bullet = Instantiate(pellet, bulletSpawn.position, Quaternion.identity).GetComponent<Rigidbody>();
        bullet.AddForce(dir.normalized * 531f, ForceMode.Impulse);
        GameEvents.OnMGShoot?.Invoke();

        // Spread
        bullet.AddForce(new Vector3(
            Random.Range(-spreadStrength, spreadStrength),
            Random.Range(-spreadStrength, spreadStrength),
            Random.Range(-spreadStrength, spreadStrength)
        ), ForceMode.Impulse);
    }

    IEnumerator Reload()
    {
        isReloading = true;
        GameEvents.OnMGReload?.Invoke();
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

    private void OnDrawGizmos()
    {
        //draw a line from cam forward to whatever it hits within 67 units

        Debug.DrawLine(cam.position, cam.position + cam.forward * 67f, Color.red, 2f);
    }
}
