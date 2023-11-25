using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyController : CharacterBaseController
{
    [SerializeField] private List<Transform> waypoints;
    private int waypointIndex = 0;

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
    public override CharacterHUDController GetCharacterHUDController() => enemyHUDController;

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
        if (target != null && !isDead)
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
        else
        {
            MoveBetweenWaypoints();
        }
    }

    private void MoveBetweenWaypoints()
    {
        Vector2 direction = (waypoints[waypointIndex].position - transform.position).normalized;

        animationController.SetMovementDirection(direction);

        // INFO: Moves from its current position towards a position held at waypointIndex in the wayPoints list
        transform.position = Vector2.MoveTowards(transform.position, waypoints[waypointIndex].position, movementSpeed * Time.deltaTime);

        // INFO: Given that the distance between itself and the destination object is less than some amount
        if (Mathf.Abs((transform.position - waypoints[waypointIndex].position).magnitude) < 0.1f)
        {
            // INFO: Given that the index is equal to the size of the list we know we've reached our last waypoint
            if (waypointIndex == waypoints.Count - 1)
            {
                // INFO: Hence we reset it to 0 to move to the very first platform, creating a looping effect
                waypointIndex = 0;
            }
            else
            {
                // INFO: Otherwise the index will be increased so that the platform can move to the next waypoint in the list
                waypointIndex++;
            }
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
        waypointIndex = 0;
        health = enemyHUDController.GetMaxHealth();
        enemyHUDController.SetHealth(health);
        enemyCollider.enabled = true;

        spriteRenderer.flipX = startingFlip;
        enemyAnimationController.GetAnimator().SetFloat("MovementY", 0);

        characterAnimationController.ChangeAnimationState(EnemyAnimationController.IDLE);
    }
}
