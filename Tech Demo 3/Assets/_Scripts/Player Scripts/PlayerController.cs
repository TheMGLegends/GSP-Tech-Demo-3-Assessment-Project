using UnityEngine;
using UnityEngine.EventSystems;


public class PlayerController : MonoBehaviour
{
    [SerializeField] private CharacterStatsSO characterStats;
    [SerializeField] private JoystickController joystickController;
    [SerializeField] private PlayerHUDController playerHUDController;
    [SerializeField] private MeleeAttackController meleeAttackController;

    [SerializeField] private float meleeAttackRadius;
    [SerializeField] private LayerMask touchableMasks;

    // INFO: Character Stats Variables:
    private float health;
    private float mana;
    private float movementSpeed;
    private float defenseMultiplier;
    private float manaRegen;
    private float damageAmount;
    private float meleeAttackSpeed;

    // INFO: Movement Variables/Components:
    private Rigidbody2D rb2D;
    private Vector2 movementInput;

    // INFO: Targetting System Variables/Components:
    private GameObject target;

    // INFO: Meleeing System Variables/Components:
    private CircleCollider2D meleeAttackRange;
    private bool canAttack;

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
        // TESTING:

        GetInputAxis();
        TargetAction();
        MeleeAction();
    }

    private void FixedUpdate()
    {
        movementInput.Normalize();
        rb2D.velocity = new Vector2(movementInput.x * movementSpeed, movementInput.y * movementSpeed);
    }

    private void TargetAction()
    {
        //if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
        //{
        //    if (EventSystem.current.IsPointerOverGameObject(Input.touches[0].fingerId)) return;
        //
        //    Vector3 touchPosition = Camera.main.ScreenToWorldPoint(Input.touches[0].position);
        //    LockOn(touchPosition);
        //}

        if (Input.touchCount > 0)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                if (Input.touches[i].phase == TouchPhase.Began)
                {
                    if (EventSystem.current.IsPointerOverGameObject(Input.touches[i].fingerId)) continue;

                    Vector3 touchPosition = Camera.main.ScreenToWorldPoint(Input.touches[i].position);
                    LockOn(touchPosition);
                }
            }
        }

#if UNITY_EDITOR
        else if (Input.GetMouseButtonDown(0))
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                Vector3 touchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                LockOn(touchPosition);
            }
        }
#endif
    }

    private void LockOn(Vector3 touchPosition)
    {
        RaycastHit2D hit = Physics2D.Raycast(touchPosition, Vector2.zero, Mathf.Infinity, touchableMasks);

        if (hit != false && hit.collider != null)
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                target = hit.collider.gameObject;
                target.GetComponent<EnemyController>().GetEnemyHUDController().DisplayProfile(true);
                meleeAttackController.gameObject.SetActive(true);
            }
            else if ((hit.collider.CompareTag("Enemy") && target != null) || (hit.collider.CompareTag("Ground") && target != null))
            {
                meleeAttackController.gameObject.SetActive(false);
                target.GetComponent<EnemyController>().GetEnemyHUDController().DisplayProfile(false);
                target = null;
            }

            if (meleeAttackController.GetIsMeleeOn())
                meleeAttackController.ToggleButton();
        }
    }

    private void MeleeAction()
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
            if (canAttack)
            {
                canAttack = false;
                meleeAttackController.IsOutsideRangeImageActive(true);
            }
        }

        if (canAttack && meleeAttackController.GetIsMeleeOn())
            Debug.Log("Attacking");
    }

    private void GetInputAxis()
    {
#if UNITY_EDITOR
        movementInput.x = Input.GetAxisRaw("Horizontal");
        movementInput.y = Input.GetAxisRaw("Vertical");

        if (joystickController.GetJoystickInput() != Vector2.zero)
            movementInput = joystickController.GetJoystickInput();
#endif
            movementInput = joystickController.GetJoystickInput();
    }

    private void InitializeStats()
    {
        health = characterStats.GetBaseHealth();
        mana = characterStats.GetBaseMana();
        movementSpeed = characterStats.GetBaseMovementSpeed();
        defenseMultiplier = characterStats.GetBaseDefenseMultiplier();
        manaRegen = characterStats.GetBaseManaRegen();
        damageAmount = characterStats.GetNormalDamage();
        meleeAttackSpeed = characterStats.GetNormalAttackSpeed();

        playerHUDController.InitializeBars(health, mana);
    }

}
