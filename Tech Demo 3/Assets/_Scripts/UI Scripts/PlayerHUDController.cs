using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUDController : MonoBehaviour
{
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Slider manaSlider;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private TMP_Text manaText;

    private float maxHealth;
    private float maxMana;

    public void InitializeBars(float health, float mana)
    {
        maxHealth = health;
        maxMana = mana;

        healthSlider.maxValue = maxHealth;
        healthSlider.value = healthSlider.maxValue;

        manaSlider.maxValue = maxMana;
        manaSlider.value = manaSlider.maxValue;

        healthText.text = healthSlider.value + "/" + healthSlider.maxValue;
        manaText.text = manaSlider.value + "/" + manaSlider.maxValue;
    }

    public void SetHealth(float newHealth)
    {
        healthSlider.value = newHealth;
        healthText.text = healthSlider.value + "/" + maxHealth;
    }

    public void SetMana(float newMana)
    {
        manaSlider.value = newMana;
        manaText.text = manaSlider.value + "/" + maxMana;
    }
}
