using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private CharacterStatsSO characterStats;
    [SerializeField] private EnemyHUDController enemyHUDController;

    // INFO: Character Stats Variables:
    private float health;
    private float movementSpeed;
    private float defenseMultiplier;
    private float damageAmount;
    private float meleeAttackSpeed;

    // INFO: Damage Popup Variables/Components:
    private GameObject damagePopupObject;
    private Animator DPanimator;
    private TextMesh damageText;
    private float damagePopupYOffset = 1.5f;

    public EnemyHUDController GetEnemyHUDController() => enemyHUDController;
    public float GetHealth() => health;

    private void Awake()
    {
        InitializeStats();
    }

    private void Start()
    {
        damagePopupObject = Instantiate(ReferenceManager.Instance.damagePopupPrefab,
                                  new Vector2(transform.position.x, transform.position.y + damagePopupYOffset),
                                  Quaternion.identity);
        damagePopupObject.SetActive(false);

        damageText = damagePopupObject.transform.GetChild(0).GetComponent<TextMesh>();
        DPanimator = damagePopupObject.transform.GetChild(0).GetComponent<Animator>();
    }

    private void InitializeStats()
    {
        health = characterStats.GetBaseHealth();
        movementSpeed = characterStats.GetBaseMovementSpeed();
        defenseMultiplier = characterStats.GetBaseDefenseMultiplier();
        damageAmount = characterStats.GetNormalDamage();
        meleeAttackSpeed = characterStats.GetNormalAttackSpeed();

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

            damageText.text = ((int)damage).ToString();
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
        DPanimator.SetBool("IsFloating", true);
        yield return new WaitForSeconds(duration);
        DPanimator.SetBool("IsFloating", false);
        damagePopupObject.SetActive(false);
    }
}
