using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;


public class PlayerController : MonoBehaviour
{
    [SerializeField] private CharacterStatsSO characterStats;
    [SerializeField] private JoystickController joystickController;
    [SerializeField] private PlayerHUDController playerHUDController;
    [SerializeField] private MeleeAttackController meleeAttackController;
    [SerializeField] private AbilitiesController abilitiesController;

    [SerializeField] private float meleeAttackRadius;
    [SerializeField] private LayerMask touchableMasks;

    // INFO: Character Stats Variables:
    private float health;
    private float mana;
    private float movementSpeed;
    private float defenseMultiplier;
    private float manaRegen;
    private float meleeDamageAmount;
    private float meleeAttackInterval;

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
    private Animator animator;
    private PlayerAnimationController animationController;

    public Vector2 GetMovementInput() => movementInput;
    public GameObject GetTarget() => target;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, meleeAttackRadius);
    }

    private void Awake()
    {
        InitializeStats();
    }

    private void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        meleeAttackRange = GetComponentInChildren<CircleCollider2D>();
        meleeAttackRange.radius = meleeAttackRadius;

        animator = GetComponent<Animator>();
        animationController = GetComponent<PlayerAnimationController>();
    }


    private void Update()
    {
        // TESTING:
        if (Input.GetKeyDown(KeyCode.Space))
        {
            health -= 10;
            mana -= 15;
            Debug.Log(health);
            Debug.Log(mana);
            playerHUDController.SetHealth(health);
            playerHUDController.SetMana(mana);
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
                abilitiesController.ActivateCastingUI(false);
                meleeAttackController.gameObject.SetActive(false);
                target.GetComponent<EnemyController>().GetEnemyHUDController().DisplayProfile(false, null);
                target = null;
            }

            if (meleeAttackController.GetIsMeleeOn())
                meleeAttackController.ToggleButton();
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
            if (!animationController.IsAnimationPlaying(animator, PlayerAnimationController.AnimationStates.Melee_Swing))
                animationController.ChangeAnimationState(PlayerAnimationController.AnimationStates.Melee_Ready);

            MeleeAction();
        }
    }

    private void MeleeAction()
    {
        currentMeleeTime += Time.deltaTime;

        if (currentMeleeTime > meleeAttackInterval)
        {
            currentMeleeTime = 0;
            animationController.ChangeAnimationState(PlayerAnimationController.AnimationStates.Melee_Swing);
            StartCoroutine(MeleeDamageCoroutine(animator.GetCurrentAnimatorStateInfo(0).length / 2));
        }
    }

    private IEnumerator MeleeDamageCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        target.GetComponent<EnemyController>().TakeDamage(meleeDamageAmount);
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
        if (!animationController.IsAnimationPlaying(animator, PlayerAnimationController.AnimationStates.Melee_Swing) && 
            !animationController.IsAnimationPlaying(animator, PlayerAnimationController.AnimationStates.Melee_Ready))
        {
            if (movementInput != Vector2.zero)
                animationController.ChangeAnimationState(PlayerAnimationController.AnimationStates.Walk);
            else
                animationController.ChangeAnimationState(PlayerAnimationController.AnimationStates.Idle);
        }
    }
    
    private void InitializeStats()
    {
        health = characterStats.GetBaseHealth();
        mana = characterStats.GetBaseMana();
        movementSpeed = characterStats.GetBaseMovementSpeed();
        defenseMultiplier = characterStats.GetBaseDefenseMultiplier();
        manaRegen = characterStats.GetBaseManaRegen();
        meleeDamageAmount = characterStats.GetNormalDamage();
        meleeAttackInterval = characterStats.GetNormalAttackSpeed();

        playerHUDController.InitializeBars(health, mana);
    }

}
