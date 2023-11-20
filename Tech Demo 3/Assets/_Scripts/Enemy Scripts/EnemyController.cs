using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyController : CharacterController
{
    [SerializeField] private EnemyHUDController enemyHUDController;

    public EnemyHUDController GetEnemyHUDController() => enemyHUDController;

    protected override void Start()
    {
        base.Start();
    }

    protected override void InitializeStats()
    {
        base.InitializeStats();

        enemyHUDController.InitializeBars(health);
    }

    public void TakeDamage(float damage)
    {
        if (Random.Range(0, 101) <= ReferenceManager.Instance.GetHitChance())
        {
            Debug.Log("Hit hit!");
            damage *= Random.Range(0.75f, 1.25f) * defenseMultiplier;

            if (Random.Range(0, 101) <= ReferenceManager.Instance.GetCritChance())
            {
                damage *= 2;
                Debug.Log("Crit hit!");
            }

            damagePopupText.text = ((int)damage).ToString();
            StartCoroutine(ActivateDamagePopup(1));

            health -= (int)damage;
            enemyHUDController.SetHealth(health);
        }
        else
            Debug.Log("Missed hit!");
    }

    private IEnumerator ActivateDamagePopup(float duration)
    {
        damagePopupObject.SetActive(true);
        damagePopupAnimator.SetBool("IsFloating", true);
        yield return new WaitForSeconds(duration);
        damagePopupAnimator.SetBool("IsFloating", false);
        damagePopupObject.SetActive(false);
    }
}
