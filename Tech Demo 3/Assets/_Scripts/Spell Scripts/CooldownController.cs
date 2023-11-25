using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CooldownController : MonoBehaviour
{
    private bool isSpellOnCooldown;
    private float cooldownDuration;
    private TMP_Text cooldownText;

    private void Start()
    {
        cooldownText = GetComponentInChildren<TMP_Text>();
    }

    private void Update()
    {
        if (isSpellOnCooldown)
        {
            cooldownDuration -= Time.deltaTime;
            cooldownText.text = cooldownDuration.ToString("F1");

            if (cooldownDuration <= 0)
            {
                isSpellOnCooldown = false;
                gameObject.SetActive(false);
            }
        }
    }

    public void ActivateCooldown( float cooldownDuration )
    {
        isSpellOnCooldown = true;
        this.cooldownDuration = cooldownDuration;

        if (cooldownText == null)
            cooldownText = GetComponentInChildren<TMP_Text>();

        cooldownText.text = cooldownDuration.ToString("F1");
    }
}
