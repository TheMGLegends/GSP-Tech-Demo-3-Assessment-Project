using System.Collections;
using System.Collections.Generic;
using Unity.Android.Types;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyController : CharacterBaseController
{
    [SerializeField] private List<Transform> waypoints;
    private int waypointIndex = 0;

    [SerializeField] private EnemyHUDController enemyHUDController;
    [SerializeField] private AbilitySO toxicSpitAbility;

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

    private bool isEnraged;
    private int numberOfAttacks;

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
            numberOfAttacks++;

        if (currentAttackTime > normalAttackInterval && numberOfAttacks < 3)
        {
            currentAttackTime = 0;
            animationController.ChangeAnimationState(EnemyAnimationController.RANGED_ATTACK);
            StartCoroutine(AttackCoroutine(animationController.GetAnimator().GetCurrentAnimatorStateInfo(0).length / 2));
        }
        else if (currentAttackTime > normalAttackInterval && numberOfAttacks >= 3)
        {
            currentAttackTime = 0;
            numberOfAttacks = 0;
            animationController.ChangeAnimationState(EnemyAnimationController.RANGED_ATTACK);
            StartCoroutine(ToxicSpitCoroutine(animationController.GetAnimator().GetCurrentAnimatorStateInfo(0).length / 2));
        }
    }

    private IEnumerator AttackCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        string returnValue = DamageManager.Instance.Damage(normalDamageAmount, target.GetComponent<PlayerController>(), target.GetComponent<PlayerController>().GetPlayerHUDController(), Color.yellow);

        if (returnValue != AttackResultStrings.hasMissed)
        {
            if (Random.Range(0, 101) <= 50)
            {
                target.GetComponent<CharacterBaseController>().ReduceHealth(target.GetComponent<PlayerController>().GetPoisonDamage());
                target.GetComponent<CharacterBaseController>().GetCharacterHUDController().SetHealth(target.GetComponent<CharacterBaseController>().GetHealth());
            }
        }
    }

    private IEnumerator ToxicSpitCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        GameObject GO = Instantiate(ReferenceManager.Instance.spellPrefab, transform.position, Quaternion.identity);
        GO.GetComponent<SpellController>().GatherInfo(gameObject, ReferenceManager.Instance.playerObject, toxicSpitAbility);
        GO.GetComponent<SpriteRenderer>().sprite = toxicSpitAbility.GetAbilitySprite();
        GO.GetComponent<SpriteRenderer>().color = Color.green;
    }

    protected override void DeathAction()
    {
        GetComponent<StatusEffectController>().RemoveAllEffects();
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

    public override void ReduceHealth(float damage)
    {
        base.ReduceHealth(damage);

        if (!isEnraged && health < characterStats.GetBaseHealth() * 0.2)
        {
            spriteRenderer.color = Color.red;
            normalDamageAmount *= 2;
            isEnraged = true;
        }
    }
}
