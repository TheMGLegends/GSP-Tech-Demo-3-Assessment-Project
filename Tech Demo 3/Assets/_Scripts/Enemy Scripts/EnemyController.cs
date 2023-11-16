using System.Collections;
using System.Collections.Generic;
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

    public EnemyHUDController GetEnemyHUDController() => enemyHUDController;

    private void Awake()
    {
        InitializeStats();
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
        health -= damage;
        enemyHUDController.SetHealth(health);
    }
}
