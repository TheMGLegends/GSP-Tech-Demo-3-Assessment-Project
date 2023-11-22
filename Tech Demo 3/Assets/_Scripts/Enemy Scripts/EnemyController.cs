using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyController : CharacterBaseController
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

    protected override void DeathAction()
    {
        base.DeathAction();
        EnemyManager.Instance.RemoveEnemy(this);
        characterAnimationController.ChangeAnimationState(EnemyAnimationController.DEAD);
        Invoke(nameof(AfterDeath), characterAnimationController.GetAnimator().GetCurrentAnimatorStateInfo(0).length / 2);
    }

    protected override void AfterDeath()
    {
        enemyHUDController.enabled = false;

        characterAnimationController.GetAnimator().enabled = false;
        characterAnimationController.enabled = false;

        enabled = false;
        GetComponent<CharacterBaseController>().enabled = false;
    }

    public void AfterPlayerDeath()
    {
        transform.position = startingPosition;
        target = null;
        health = enemyHUDController.GetMaxHealth();
        enemyHUDController.SetHealth(health);
        characterAnimationController.ChangeAnimationState(EnemyAnimationController.IDLE);
    }
}
