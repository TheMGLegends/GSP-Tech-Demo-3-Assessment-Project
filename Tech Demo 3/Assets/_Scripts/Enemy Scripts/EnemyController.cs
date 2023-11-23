using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyController : CharacterBaseController
{
    [SerializeField] private EnemyHUDController enemyHUDController;

    private CircleCollider2D enemyCollider;
    private EnemyAnimationController enemyAnimationController;

    private SpriteRenderer spriteRenderer;
    private bool startingFlip;

    // INFO: Aggro System Component:
    private AggroDetection aggroDetection;

    // INFO: Animation Controller:
    private EnemyAnimationController animationController;

    // INFO: Attacking System:
    private float currentAttackTime;

    public EnemyHUDController GetEnemyHUDController() => enemyHUDController;

    protected override void Start()
    {
        base.Start();
        enemyCollider = GetComponent<CircleCollider2D>();
        enemyAnimationController = GetComponent<EnemyAnimationController>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        aggroDetection = GetComponentInChildren<AggroDetection>();
        animationController = GetComponent<EnemyAnimationController>();

        startingFlip = spriteRenderer.flipX;
    }

    protected override void InitializeStats()
    {
        base.InitializeStats();

        enemyHUDController.InitializeBars(health);
    }

    private void Update()
    {
        if (target != null)
        {
            if (!animationController.IsAnimationPlaying(EnemyAnimationController.RANGED_ATTACK))
                animationController.ChangeAnimationState(EnemyAnimationController.IDLE);

            if (Vector2.Distance(transform.position, target.transform.position) > aggroDetection.GetStoppingDistanceFromTarget())
            {    
                transform.position = Vector2.MoveTowards(transform.position, target.transform.position, movementSpeed * Time.deltaTime);
            }

            if (canAttack)
                AttackAction();
            else
                currentAttackTime = 0;
        }
    }

    private void AttackAction()
    {
        currentAttackTime += Time.deltaTime;

        if (currentAttackTime > normalAttackInterval)
        {
            currentAttackTime = 0;
            animationController.ChangeAnimationState(EnemyAnimationController.RANGED_ATTACK);
            StartCoroutine(AttackCoroutine(animationController.GetAnimator().GetCurrentAnimatorStateInfo(0).length / 2));
        }
    }

    private IEnumerator AttackCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        DamageManager.Instance.Damage(normalDamageAmount, target.GetComponent<PlayerController>(), target.GetComponent<PlayerController>().GetPlayerHUDController(), Color.yellow);
    }

    protected override void DeathAction()
    {
        enemyCollider.enabled = false;
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
        enemyCollider.enabled = true;

        spriteRenderer.flipX = startingFlip;
        enemyAnimationController.GetAnimator().SetFloat("MovementY", 0);

        characterAnimationController.ChangeAnimationState(EnemyAnimationController.IDLE);
    }
}
