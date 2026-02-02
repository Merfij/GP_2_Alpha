using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DemonAbilities : MonoBehaviour
{
     
    public Minigun gun;
    public DemonLaser laser;
    private bool isHoldingShoot = false;
    private bool isHoldingLaser = false;
    public Image image;
    public bool isLaserActive => isHoldingLaser;

    void OnShoot(InputValue button)
    {
        if (button.isPressed)
        {
            isHoldingShoot = true;
        }

        if (!button.isPressed)
        {
            isHoldingShoot = false;
        }
    }

    void OnLaser(InputValue button)
    {
        if (button.isPressed)
        {
            isHoldingLaser = true;
        }
        else
        {
            isHoldingLaser = false;
        }
    }


    void OnReload()
    {
        gun.TryReload();
    }

    private void Update()
    {
        if (isHoldingShoot && !isHoldingLaser)
        {
            gun.Shoot();            
        }

        if (isHoldingLaser)
        {
            laser.ActivateLaser();
        }
        else
        {
            laser.DeactivateLaser();
        }
        SetAmmo(gun.currentAmmo);
    }

    public void SetAmmo(float currentAmmo)
    {
        image.fillAmount = currentAmmo / gun.magSize;
    }
}
