using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHUDController : MonoBehaviour
{
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TMP_Text healthText;

    private float maxHealth;

    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void InitializeBars(float health)
    {
        maxHealth = health;

        healthSlider.maxValue = maxHealth;
        healthSlider.value = healthSlider.maxValue;

        healthText.text = healthSlider.value + "/" + healthSlider.maxValue;
    }

    public void SetHealth(float newHealth)
    {
        healthSlider.value = newHealth;
        healthText.text = healthSlider.value + "/" + maxHealth;
    }

    public void DisplayProfile(bool isTargeting, GameObject target)
    {
        if (isTargeting)
        {
            SetHealth(target.GetComponent<EnemyController>().GetHealth());
            gameObject.SetActive(true);
        }
        else
            gameObject.SetActive(false);
    }
}
