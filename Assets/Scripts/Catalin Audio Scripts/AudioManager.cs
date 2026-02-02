using FMOD.Studio;
using FMODUnity;
using System.Runtime.CompilerServices;
using UnityEngine;
using static Unity.VisualScripting.Member;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] EventReference BGTheme;

    private GameObject Demon;
    private GameObject Exorcist;
    private EventInstance actionInstance;
    private EventInstance themeInstance;
    private EventInstance puzzleInstance;

    #region EventReferences
    [Header("Player Sounds")]
    [SerializeField] EventReference FootstepEvent;    // waiting on animation
    [SerializeField] EventReference ExorcistJump;
    [SerializeField] EventReference DemonJump;
    [SerializeField] EventReference DemonHit;
    [SerializeField] EventReference ExorcistHit;

    [Header("Gun Sounds")]
    [SerializeField] EventReference MGSound; 
    [SerializeField] EventReference SGSound; 
    [SerializeField] EventReference MGReload; 
    [SerializeField] EventReference SGReload; 
    //[SerializeField] EventReference HitFlesh;
    //[SerializeField] EventReference HitGround;
    //[SerializeField] EventReference HitMetal;

    [Header("Beam Sounds")]
    [SerializeField] EventReference DemonBeam; 
    [SerializeField] EventReference ExorcistBeam; 
    //[SerializeField] EventReference PulledIntoWorld;
    //[SerializeField] EventReference LightStun;

    //[Header("Enemy Sounds")]
    //[SerializeField] EventReference ImpGrowl; // not done
    /*[SerializeField] EventReference ImpDeath;
    [SerializeField] EventReference ImpHurt;
    [SerializeField] EventReference ImpLightHiss;
    [SerializeField] EventReference ShiftSound;

    [Header("UI Sounds")]
    [SerializeField] EventReference UIHover;
    [SerializeField] EventReference UIClick;
    [SerializeField] EventReference GameStart;*/

    [Header("Music")]
    [SerializeField] EventReference ActionMusic;
    //[SerializeField] EventReference PreFightMusic;
    [SerializeField] EventReference PuzzleMusic;

    [Header("Puzzle")]
    [SerializeField] EventReference Batterycharge;
    [SerializeField] EventReference BatteryDrain;
    [SerializeField] EventReference BatteryFull;
    [SerializeField] EventReference DoorOpen;
    [SerializeField] EventReference DoorClose;
    [SerializeField] EventReference PlatformSlide;
    [SerializeField] EventReference EtherealObjectAppear;
    [SerializeField] EventReference EtherealObjectDisapear;
    [SerializeField] EventReference BarrelExplosion;
    [SerializeField] EventReference PressureplatePress;
    [SerializeField] EventReference PressureplateRelease;
    #endregion

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        GameEvents.OnMGShoot += PlayMGSound;
        GameEvents.OnSGShoot += PlaySGSound;
        GameEvents.OnDemonJump += PlayDemonJump;
        GameEvents.OnExorcistJump += PlayExorcistJump;
        GameEvents.OnFootstep += PlayFootstep;
        GameEvents.OnSGReload += PlaySGReload;
        GameEvents.OnMGReload += PlayMGReload;
        //GameEvents.OnImpGrowl += PlayImpGrowl;
        GameEvents.OnBatteryFull += PlayBatteryFull;
        GameEvents.OnExorcistHit += PlayExorcistHit;
        GameEvents.OnDemonHit += PlayDemonHit;
        GameEvents.OnDoorOpen += PlayDoorOpen;
        GameEvents.OnDoorOpen += PlayDoorClose;
        GameEvents.OnPlatformSlide += PlayPlatformSlide;
        GameEvents.OnEtherealObjectAppear += PlayEtherealObject;
        GameEvents.OnEtherealObjectDisapear += PlayEtherealDisapear;
        GameEvents.OnBarrelExplosion += PlayExplosion;
        GameEvents.OnPressureplatePress += OnPlayPressurePress;
        GameEvents.OnPressureplateRelease += OnPlayPressureRelease;
    }

    private void OnDisable()
    {
        GameEvents.OnMGShoot -= PlayMGSound;
        GameEvents.OnSGShoot -= PlaySGSound;
        GameEvents.OnDemonJump -= PlayDemonJump;
        GameEvents.OnExorcistJump -= PlayExorcistJump;
        GameEvents.OnFootstep -= PlayFootstep;
        GameEvents.OnSGReload -= PlaySGReload;
        GameEvents.OnMGReload -= PlayMGReload;
        GameEvents.OnBatteryFull -= PlayBatteryFull;
        GameEvents.OnExorcistHit -= PlayExorcistHit;
        GameEvents.OnDemonHit -= PlayDemonHit;
        GameEvents.OnDoorOpen -= PlayDoorOpen;
        GameEvents.OnDoorOpen -= PlayDoorClose;
        GameEvents.OnPlatformSlide -= PlayPlatformSlide;
        GameEvents.OnEtherealObjectAppear -= PlayEtherealObject;
        GameEvents.OnEtherealObjectDisapear -= PlayEtherealDisapear;
        GameEvents.OnBarrelExplosion -= PlayExplosion;
        GameEvents.OnPressureplatePress -= OnPlayPressurePress;
        GameEvents.OnPressureplateRelease -= OnPlayPressureRelease;
    }

    private void Start()
    {
        Exorcist = GameObject.Find("Exorcist(Clone)");
        Demon = GameObject.Find("Demon(Clone)");
        PlayBGTheme();
    }
    public EventReference GetEvent(string eventName)
    {
        return eventName switch
        {
            "DemonBeam" => DemonBeam,
            "ExorcistBeam" => ExorcistBeam,
            "BatteryCharge" => Batterycharge,
            "BatteryDrain" => BatteryDrain,
            _ => default
        };
    }
    #region PlayerSounds
    public void PlayFootstep(Transform source)
    {
        if (source != null)
        {
            RuntimeManager.PlayOneShotAttached(FootstepEvent, source.gameObject);
        }
    }

    private void PlayDemonJump()
    {
        if (Demon != null)
            RuntimeManager.PlayOneShotAttached(DemonJump, Demon);
    }

    private void PlayExorcistJump()
    {
        if (Exorcist != null)
            RuntimeManager.PlayOneShotAttached(ExorcistJump, Exorcist);
    }

    private void PlayExorcistHit()
    {
        if (Exorcist != null)
            RuntimeManager.PlayOneShotAttached(ExorcistHit, Exorcist);
    }

    private void PlayDemonHit()
    {
        if (Demon != null)
            RuntimeManager.PlayOneShotAttached(DemonHit, Demon);
    }

    #endregion
    
    #region GunSounds
    private void PlayMGSound()
    {
        if (Demon != null)
            RuntimeManager.PlayOneShotAttached(MGSound, Demon);    
    }

    private void PlaySGSound()
    {
        if (Exorcist != null)
            RuntimeManager.PlayOneShotAttached(SGSound, Exorcist);
    }

    private void PlayMGReload()
    {
        if (Demon != null)
            RuntimeManager.PlayOneShotAttached(MGReload, Demon);
    }

    private void PlaySGReload()
    {
        if (Exorcist != null)
            RuntimeManager.PlayOneShotAttached(SGReload, Exorcist);
    }
    #endregion

    #region EnemySounds
    /*private void PlayImpGrowl(Transform source)
    {
        if (source != null)
            RuntimeManager.PlayOneShotAttached(ImpGrowl, source.gameObject);
    }*/
    #endregion

    #region PuzzleSounds
    private void PlayBatteryFull(Transform source)
    {
        if (source != null)
            RuntimeManager.PlayOneShotAttached(BatteryFull, source.gameObject);
    }

    private void PlayDoorOpen(Transform source)
    {
        RuntimeManager.PlayOneShotAttached(DoorOpen, source.gameObject);
    }

    private void PlayDoorClose(Transform source)
    {
        RuntimeManager.PlayOneShotAttached(DoorClose, source.gameObject);
    }

    private void PlayPlatformSlide(Transform source)
    {
        RuntimeManager.PlayOneShotAttached(PlatformSlide, source.gameObject);
    }

    private void PlayEtherealObject(Transform source)
    {
        RuntimeManager.PlayOneShotAttached(EtherealObjectAppear, source.gameObject);
    }

    private void PlayEtherealDisapear(Transform source)
    {
        RuntimeManager.PlayOneShotAttached(EtherealObjectDisapear, source.gameObject);
    }

    private void PlayExplosion(Transform source)
    {
        RuntimeManager.PlayOneShotAttached(BarrelExplosion, source.gameObject);
    }

    private void OnPlayPressurePress(Transform source)
    {
        RuntimeManager.PlayOneShotAttached(PressureplatePress, source.gameObject);
    }

    private void OnPlayPressureRelease(Transform source)
    {
        RuntimeManager.PlayOneShotAttached(PressureplateRelease, source.gameObject);
    }

    #endregion

    #region Music

    public void PlayBGTheme()
    {
        if (themeInstance.isValid())
            return;

        themeInstance = RuntimeManager.CreateInstance(BGTheme);
        themeInstance.start();
    }

    public void StopTheme()
    {
        if (!themeInstance.isValid())
            return;

        themeInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        themeInstance.release();
    }


    public void PlayActionMusic()
    {
        if (actionInstance.isValid())
            return;

        actionInstance = RuntimeManager.CreateInstance(ActionMusic);
        actionInstance.start();
        Debug.Log("playing action music");
    }

    public void StopActionMusic()
    {
        if (!actionInstance.isValid())
            return;
        
            actionInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            actionInstance.release();
            Debug.Log("no more action music");        
    }

    public void PlayPuzzleMusic()
    {
        if(puzzleInstance.isValid())
            return;
        puzzleInstance = RuntimeManager.CreateInstance(PuzzleMusic);
        puzzleInstance.start();
    }

    public void StopPuzzleMusic()
    {
        if (!puzzleInstance.isValid()) 
            return;
        
        puzzleInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        puzzleInstance.release();
           
    }

    #endregion
}
