using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CooldownController : MonoBehaviour
{
    private float cooldownDuration;
    private TMP_Text cooldownText;

    private void Start()
    {
        cooldownText = GetComponentInChildren<TMP_Text>();
    }

    public void ActivateCooldown( float cooldownDuration )
    {
        this.cooldownDuration = cooldownDuration;

        if (cooldownText == null)
            cooldownText = GetComponentInChildren<TMP_Text>();

        cooldownText.text = cooldownDuration.ToString("F1");

        InvokeRepeating(nameof(Cooldown), 0, Time.deltaTime);
    }

    private void Cooldown()
    {
        cooldownDuration -= Time.deltaTime;
        cooldownText.text = cooldownDuration.ToString("F1");

        if (cooldownDuration <= 0)
        {
            gameObject.SetActive(false);
            CancelInvoke(nameof(Cooldown));
        }
    }
}
