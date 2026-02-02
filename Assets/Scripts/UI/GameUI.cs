using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [SerializeField] Image damageScreen, ammoBar, healthbar;
    [SerializeField] PlayerHealth player;
    [SerializeField] TMP_Text healthNr;
    ExorcistAbilities exorcist;
    Shotgun shotgun;
    Minigun minigun;
    IEnumerator corutine;
    float health, maxhealth;

    IEnumerator DamageTime(float time)
    {
        yield return new WaitForSeconds(time);
        damageScreen.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        player.onPlayerDamage += DisplayDamage;
    }

    private void OnDisable()
    {
        player.onPlayerDamage -= DisplayDamage;
    }

    private void Start()
    {
        minigun = FindFirstObjectByType<Minigun>();
        shotgun = FindFirstObjectByType<Shotgun>();
        
        health = FindFirstObjectByType<SharedHealth>().currentHealth;
        maxhealth = FindFirstObjectByType<SharedHealth>().getMaxHealth();
    }

    private void DisplayDamage()
    {
        damageScreen.gameObject.SetActive(true);
        corutine = DamageTime(0.1f);
        StartCoroutine(corutine);
    }

    private void FixedUpdate()
    {
        health = FindFirstObjectByType<SharedHealth>().currentHealth;
        healthNr.text = health.ToString();
        healthbar.fillAmount = health / maxhealth;
    }

}
