using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class DemonLaser : MonoBehaviour
{
    public MeshRenderer laserOrigin;
    public Transform laserSpawn;
    public LineRenderer laserBeam;
    public Transform cam;
    
    public LayerMask enemy;

    private IAffectedByLaser currentStunnedEnemy;
    private IAbilityTarget currentTarget;
    [SerializeField] private AbilityType currentAbility;
    int layerMask;

    bool m_laserGate = false;

    private EventInstance beamInstance;

    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        laserBeam.transform.position = laserSpawn.position;
        laserBeam.transform.rotation = laserSpawn.rotation;
        layerMask = ~LayerMask.GetMask("Trigger");
    }

    public void ActivateLaser()
    {
        laserOrigin.enabled = true;
        laserBeam.enabled = true;

        if (m_laserGate == true)
            return;

        print("pls do this once only");
        m_laserGate = true;
        EventReference evt = AudioManager.Instance.GetEvent("DemonBeam");
        beamInstance = RuntimeManager.CreateInstance(evt);
        beamInstance.start();
    }

    public void DeactivateLaser()
    {
        laserOrigin.enabled = false;
        laserBeam.enabled = false;
        m_laserGate = false;

        if (beamInstance.isValid())
        {
            beamInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            beamInstance.release();
        }

        ClearTarget();
    }

    //send a raycast from the laser spawn point forward while laser is enabled
    void Update()
    {
        if (laserBeam.enabled)
        {
            RaycastHit hit;

            // Combine the public enemy LayerMask with the mask that excludes the "Trigger" layer.
            int combinedMask = ((int)enemy) & layerMask;

            // Cast the ray while ignoring trigger colliders.
            //if (Physics.Raycast(cam.position, cam.forward, out hit, 100f, combinedMask, QueryTriggerInteraction.Ignore)) //if the raycast hits something
            //{
            //    return;
            //}
            if (Physics.Raycast(cam.position, cam.forward, out hit, 100f, enemy)) //if the raycast hits something
            {
                IAffectedByLaser enemy = hit.collider.GetComponentInParent<IAffectedByLaser>();
                laserBeam.SetPosition(0, laserSpawn.position);
                laserBeam.SetPosition(1, hit.point);

                //If the laser hits an enemy, do the laser logic
                if (enemy != null)
                {
                    if (currentStunnedEnemy != enemy) //switch stunned enemy
                    {
                        currentStunnedEnemy?.ResumeEnemy();
                        currentStunnedEnemy = enemy;
                        currentStunnedEnemy.AffectedByLaser();
                    }
                    currentStunnedEnemy.AffectedByLaser();

                }
                if (hit.collider.TryGetComponent<IAbilityTarget>(out var target)) //raycast hits something that implements IAbilityTarget
                {
                    if (target != currentTarget)
                    {
                        currentTarget?.AbilityEnd(currentAbility);
                        currentTarget = target;
                        currentTarget.AbilityStart(currentAbility);
                    }
                }
                else 
                { 
                    ClearTarget(); //raycast hits something that does not implement IAbilityTarget
                } 
            }
            else
            {
                ClearTarget();
                laserBeam.SetPosition(0, laserSpawn.position);
                laserBeam.SetPosition(1, laserSpawn.position + cam.forward * 100f);

                currentStunnedEnemy?.ResumeEnemy();
                currentStunnedEnemy = null;
            }
        }
        if (beamInstance.isValid())
        {
            beamInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform));
        }
    }

    void ClearTarget()
    {
        if (currentTarget != null)
        {
            currentTarget.AbilityEnd(currentAbility);
            currentTarget = null;
        }
    }

}
