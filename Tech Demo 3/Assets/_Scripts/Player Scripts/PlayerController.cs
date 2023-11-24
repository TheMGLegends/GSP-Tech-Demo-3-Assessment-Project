using System;
using System.Collections;
using System.Linq.Expressions;
using Unity.Android.Types;
using UnityEngine;
using UnityEngine.EventSystems;


public class PlayerController : CharacterBaseController
{
    // INFO: HUD References:
    [SerializeField] private JoystickController joystickController;
    [SerializeField] private PlayerHUDController playerHUDController;
    [SerializeField] private MeleeAttackController meleeAttackController;
    [SerializeField] private AbilitiesController abilitiesController;

    // INFO: Melee Attack References:
    [SerializeField] private float meleeAttackRadius;
    [SerializeField] private LayerMask touchableMasks;

    // INFO: Player Stats Variables:
    private float mana;
    private float manaRegen;

    // INFO: Movement Variables/Components:
    private Rigidbody2D rb2D;
    private BoxCollider2D playerCollider;
    private Vector2 movementInput;

    // INFO: Meleeing System Variables/Components:
    private CircleCollider2D meleeAttackRange;
    private float currentMeleeTime;

    // INFO: Animation Controller:
    private PlayerAnimationController animationController;

    // INFO: Casting Particles:

    public Vector2 GetMovementInput() => movementInput;
    public PlayerHUDController GetPlayerHUDController() => playerHUDController;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, meleeAttackRadius);
    }

    protected override void Start()
    {
        base.Start();

        rb2D = GetComponent<Rigidbody2D>(); 
        playerCollider = GetComponent<BoxCollider2D>();
        meleeAttackRange = GetComponentInChildren<CircleCollider2D>();
        meleeAttackRange.radius = meleeAttackRadius;

        animationController = GetComponent<PlayerAnimationController>();
    }

    protected override void InitializeStats()
    {
        base.InitializeStats();
        mana = characterStats.GetBaseMana();
        manaRegen = characterStats.GetBaseManaRegen();

        playerHUDController.InitializeBars(health, mana);
    }

    private void GetInputAxis()
    {
        movementInput = joystickController.GetJoystickInput();

#if UNITY_EDITOR
        movementInput.x = Input.GetAxisRaw("Horizontal");
        movementInput.y = Input.GetAxisRaw("Vertical");

        if (joystickController.GetJoystickInput() != Vector2.zero)
            movementInput = joystickController.GetJoystickInput();
#endif
    }

    private void AnimateCharacter()
    {
        if (!animationController.IsAnimationPlaying(PlayerAnimationController.MELEE_SWING) &&
            !animationController.IsAnimationPlaying(PlayerAnimationController.MELEE_READY) &&
            !animationController.IsAnimationPlaying(PlayerAnimationController.CAST))
        {
            if (movementInput != Vector2.zero)
                animationController.ChangeAnimationState(PlayerAnimationController.WALK);
            else
                animationController.ChangeAnimationState(PlayerAnimationController.IDLE);
        }

        if (abilitiesController.GetIsCasting() &&
            !animationController.IsAnimationPlaying(PlayerAnimationController.MELEE_SWING))
            animationController.ChangeAnimationState(PlayerAnimationController.CAST);
    }

    private void Update()
    {
        //Testing CODE:
        if (Input.GetKeyDown(KeyCode.Space))
        {
            DamageManager.Instance.Damage(1000, this, playerHUDController, Color.yellow);
        }


        GetInputAxis();
        AnimateCharacter();
        TargetAuthentication();
        MeleeAuthentication();
    }


    private void FixedUpdate()
    {
        movementInput.Normalize();
        rb2D.velocity = new Vector2(movementInput.x * movementSpeed, movementInput.y * movementSpeed);
    }

    private void TargetAuthentication()
    {
        if (Input.touchCount > 0)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                if (Input.touches[i].phase == TouchPhase.Began)
                {
                    if (EventSystem.current.IsPointerOverGameObject(Input.touches[i].fingerId)) continue;

                    Vector3 touchPosition = Camera.main.ScreenToWorldPoint(Input.touches[i].position);
                    TargetAction(touchPosition);
                }
            }
        }

#if UNITY_EDITOR
        else if (Input.GetMouseButtonDown(0))
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                Vector3 touchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                TargetAction(touchPosition);
            }
        }
#endif
    }

    private void TargetAction(Vector3 touchPosition)
    {
        RaycastHit2D hit = Physics2D.Raycast(touchPosition, Vector2.zero, Mathf.Infinity, touchableMasks);

        if (hit != false && hit.collider != null)
        {
            if (hit.collider.gameObject.CompareTag("Enemy"))
            {
                target = hit.collider.gameObject;
                target.GetComponent<EnemyController>().GetEnemyHUDController().DisplayProfile(true, target);
                meleeAttackController.gameObject.SetActive(true);
                abilitiesController.ActivateCastingUI(true);
            }
            else if ((hit.collider.gameObject.CompareTag("Enemy") && target != null) || 
                     (hit.collider.gameObject.CompareTag("Ground") && target != null))
            {
                DisableUI();
            }

            if (meleeAttackController.GetIsMeleeOn())
                meleeAttackController.ToggleButton();
        }
    }

    private void DisableUI()
    {
        abilitiesController.ActivateCastingUI(false);
        meleeAttackController.gameObject.SetActive(false);

        if (target != null)
        {
            target.GetComponent<EnemyController>().GetEnemyHUDController().DisplayProfile(false, null);
            target = null;
        }
    }

    private void MeleeAuthentication()
    {
        if (target != null && Vector2.Distance(transform.position, target.transform.position) < meleeAttackRadius)
        {
            if (!canAttack)
            {
                canAttack = true;
                meleeAttackController.IsOutsideRangeImageActive(false);
            }
        }
        else
        {
            if (currentMeleeTime != 0)
                currentMeleeTime = 0;

            if (canAttack)
            {
                canAttack = false;
                meleeAttackController.IsOutsideRangeImageActive(true);
            }
        }

        if (canAttack && meleeAttackController.GetIsMeleeOn())
        {
            if (!animationController.IsAnimationPlaying(PlayerAnimationController.MELEE_SWING) &&
                !animationController.IsAnimationPlaying(PlayerAnimationController.CAST))
                animationController.ChangeAnimationState(PlayerAnimationController.MELEE_READY);

            if (!target.GetComponent<EnemyController>().GetIsDead())
                MeleeAction();
            else
                Invoke(nameof(DisableUI), 2);
        }
    }

    private void MeleeAction()
    {
        currentMeleeTime += Time.deltaTime;

        if (currentMeleeTime > normalAttackInterval)
        {
            currentMeleeTime = 0;
            animationController.ChangeAnimationState(PlayerAnimationController.MELEE_SWING);
            StartCoroutine(MeleeDamageCoroutine(animationController.GetAnimator().GetCurrentAnimatorStateInfo(0).length / 2));
        }
    }

    private IEnumerator MeleeDamageCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        //if (target != null)
        //    DamageManager.Instance.Damage(normalDamageAmount, target.GetComponent<EnemyController>(), target.GetComponent<EnemyController>().GetEnemyHUDController(), Color.red);

        //Testing Code:
        if (target != null)
          DamageManager.Instance.Damage(1000, target.GetComponent<EnemyController>(), target.GetComponent<EnemyController>().GetEnemyHUDController(), Color.red);
    }

    protected override void DeathAction()
    {
        playerCollider.enabled = false;
        DisableUI();
        characterAnimationController.ChangeAnimationState(PlayerAnimationController.DEAD);
        rb2D.velocity = Vector2.zero;
        characterAnimationController.GetAnimator().SetBool("IsDead", isDead);
        enabled = false;

        Invoke(nameof(AfterDeath), (characterAnimationController.GetAnimator().GetCurrentAnimatorStateInfo(0).length / 2) + 3f);
    }

    protected override void AfterDeath()
    {
        base.AfterDeath();
        EnemyManager.Instance.ResetEnemies();
        enabled = true;
        transform.position = startingPosition;
        InitializeStats();
        playerCollider.enabled = true;

        characterAnimationController.GetAnimator().SetBool("IsDead", isDead);
        characterAnimationController.GetAnimator().SetFloat("MovementY", -1);
        animationController.SetFacingDirection(1);
        characterAnimationController.ChangeAnimationState(PlayerAnimationController.IDLE);
    }
}
