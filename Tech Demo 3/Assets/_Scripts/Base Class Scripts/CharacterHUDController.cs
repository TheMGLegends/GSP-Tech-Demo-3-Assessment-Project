using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Base class hud controller for all characters (player and enemy)
/// </summary>
public class CharacterHUDController : MonoBehaviour
{
    [SerializeField] protected Slider healthSlider;
    [SerializeField] protected TMP_Text healthText;

    [SerializeField] protected Slider manaSlider;
    [SerializeField] protected TMP_Text manaText;

    protected float maxHealth;
    protected float maxMana;

    public float GetMaxHealth() => maxHealth;
    public float GetMaxMana() => maxMana;

    public virtual void InitializeBars(float health)
    {
        maxHealth = health;

        healthSlider.maxValue = maxHealth;
        healthSlider.value = healthSlider.maxValue;

        healthText.text = healthSlider.value + "/" + healthSlider.maxValue;
    }

    public virtual void InitializeBars(float health, float mana)
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

    public virtual void SetHealth(float newHealth)
    {
        healthSlider.value = newHealth;
        healthText.text = healthSlider.value + "/" + maxHealth;
    }

    public virtual void SetMana(float newMana)
    {
    }
}
