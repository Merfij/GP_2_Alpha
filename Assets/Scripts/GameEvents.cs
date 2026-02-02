using System;
using UnityEngine;

public static class GameEvents 
{ 
    [Header("Player")]
    public static Action<Transform> OnFootstep;
    public static Action OnDemonJump;
    public static Action OnExorcistJump;
    public static Action OnExorcistHit;
    public static Action OnDemonHit;

    [Header("Guns")]
    public static Action OnMGShoot;
    public static Action OnMGReload;
    public static Action OnSGShoot;
    public static Action OnSGReload;
    public static Action OnHittingGround;
    public static Action OnHittingMetal;
    public static Action OnHittingFlesh;

    [Header("Beams")]
    public static Action OnEnemyPulledIn;
    public static Action OnLightStun;
    public static Action OnLightActivating;
    public static Action OnDemonBeam;
    public static Action OnExorcistBeam;

    [Header("Enemy")]
    public static Action<Transform> OnImpGrowl;
    public static Action OnImpDeath;
    public static Action OnImpHurt;
    public static Action OnImpLightHiss;
    public static Action OnShiftSound;

    [Header("UI")]
    public static Action OnHover;
    public static Action OnClick;
    public static Action OnGameStart;

    [Header("Puzzle")]
    public static Action OnBatterycharge;
    public static Action OnBatteryDrain;
    public static Action<Transform> OnBatteryFull;
    public static Action<Transform> OnDoorOpen;
    public static Action<Transform> OnDoorClose;
    public static Action<Transform> OnPlatformSlide;
    public static Action<Transform> OnEtherealObjectAppear;
    public static Action<Transform> OnEtherealObjectDisapear;
    public static Action<Transform> OnBarrelExplosion;
    public static Action<Transform> OnPressureplatePress;
    public static Action<Transform> OnPressureplateRelease;

}
