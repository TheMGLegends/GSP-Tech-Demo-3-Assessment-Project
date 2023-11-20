using System;
using System.Collections;
using System.Linq.Expressions;
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
    private Vector2 movementInput;

    // INFO: Targetting System Variables/Components:
    private GameObject target;

    // INFO: Meleeing System Variables/Components:
    private CircleCollider2D meleeAttackRange;
    private bool canAttack;
    private float currentMeleeTime;

    // INFO: Animation Controller:
    private PlayerAnimationController animationController;

    public Vector2 GetMovementInput() => movementInput;
    public GameObject GetTarget() => target;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, meleeAttackRadius);
    }

    protected override void Start()
    {
        base.Start();
        damagePopupText.color = Color.yellow;

        rb2D = GetComponent<Rigidbody2D>();
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
            !animationController.IsAnimationPlaying(PlayerAnimationController.MELEE_READY))
        {
            if (movementInput != Vector2.zero)
                animationController.ChangeAnimationState(PlayerAnimationController.WALK);
            else
                animationController.ChangeAnimationState(PlayerAnimationController.IDLE);
        }
    }

    private void Update()
    {
        //Testing CODE:
        if (Input.GetKeyDown(KeyCode.Space))
        {
            DamageManager.Instance.Damage(1000, this, playerHUDController);
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
            if (hit.collider.CompareTag("Enemy"))
            {
                target = hit.collider.gameObject;
                target.GetComponent<EnemyController>().GetEnemyHUDController().DisplayProfile(true, target);
                meleeAttackController.gameObject.SetActive(true);
                abilitiesController.ActivateCastingUI(true);
            }
            else if ((hit.collider.CompareTag("Enemy") && target != null) || (hit.collider.CompareTag("Ground") && target != null))
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
            if (!animationController.IsAnimationPlaying(PlayerAnimationController.MELEE_SWING))
                animationController.ChangeAnimationState(PlayerAnimationController.MELEE_READY);

            if (!target.GetComponent<EnemyController>().GetIsDead())
                MeleeAction();
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
        DamageManager.Instance.Damage(meleeDamageAmount, target.GetComponent<EnemyController>(), target.GetComponent<EnemyController>().GetEnemyHUDController());
    }

    protected override void DeathAction()
    {
        base.DeathAction();
        DisableUI();
        characterAnimationController.ChangeAnimationState(PlayerAnimationController.DEAD);

        rb2D.velocity = Vector2.zero;
        enabled = false;

        Invoke(nameof(AfterDeath), (characterAnimationController.GetAnimator().GetCurrentAnimatorStateInfo(0).length / 2) + 3f);
    }

    protected override void AfterDeath()
    {
        enabled = true;
        transform.position = startingPosition;
        InitializeStats();
        characterCollider.enabled = true;
        movementInput = Vector2.zero;
        if (health > 0)
        {
            animationController.SetFacingDirection(movementInput.x);
            characterAnimationController.GetAnimator().SetFloat("MovementY", movementInput.y);
            characterAnimationController.ChangeAnimationState(PlayerAnimationController.IDLE);
        }
    }
}
