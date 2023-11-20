using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageManager : MonoBehaviour
{
    public static DamageManager Instance { get; private set; }

    private const int hitChance = 80;
    private const int critChance = 20;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    public void Damage(float damage, CharacterController targetController, CharacterHUDController targetHUDController)
    {
        if (Random.Range(0, 101) <= hitChance)
        {
            Debug.Log("Hit hit!");
            damage *= Random.Range(0.75f, 1.25f) * targetController.GetDefenseMultiplier();

            if (Random.Range(0, 101) <= critChance)
            {
                damage *= 2;
                Debug.Log("Crit hit!");
            }

            targetController.GetDamagePopupText().text = ((int)damage).ToString();
            StartCoroutine(ActivateDamagePopup(1, targetController));

            targetController.ReduceHealth((int)damage);
            targetHUDController.SetHealth(targetController.GetHealth());
        }
        else
            Debug.Log("Missed hit!");
    }

    private IEnumerator ActivateDamagePopup(float duration, CharacterController targetController)
    {
        GameObject damagePopupObject = targetController.GetDamagePopupObject();
        Animator targetAnimator = targetController.GetDamagePopupAnimator();


        damagePopupObject.SetActive(true);
        targetAnimator.SetBool("IsFloating", true);
        yield return new WaitForSeconds(duration);
        targetAnimator.SetBool("IsFloating", false);
        damagePopupObject.SetActive(false);
    }
}
