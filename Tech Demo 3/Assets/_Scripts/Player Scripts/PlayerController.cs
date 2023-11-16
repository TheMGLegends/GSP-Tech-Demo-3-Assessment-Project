using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class PlayerController : MonoBehaviour
{
    [SerializeField] private CharacterStatsSO characterStats;
    [SerializeField] private JoystickController joystickController;
    [SerializeField] private PlayerHUDController playerHUDController;

    // INFO: Character Stats Variables:
    private float health;
    private float mana;
    private float movementSpeed;
    private float defenseMultiplier;
    private float manaRegen;
    private float damageAmount;
    private float meleeAttackSpeed;

    // INFO: Movement Variables/Components
    private Rigidbody2D rb2D;
    private Vector2 movementInput;

    // INFO: Targetting System Variables/Components
    private GameObject target;
    private bool isTargeting;

    public Vector2 GetMovementInput() => movementInput;
    public GameObject GetTarget() => target;

    private void Awake()
    {
        InitializeStats();
    }

    private void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
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
    }

    private void FixedUpdate()
    {
        movementInput.Normalize();
        rb2D.velocity = new Vector2(movementInput.x * movementSpeed, movementInput.y * movementSpeed);
    }

    private void TargetAction()
    {
        if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
        {
            if (EventSystem.current.IsPointerOverGameObject(Input.touches[0].fingerId)) return;

            Vector3 touchPosition = Camera.main.ScreenToWorldPoint(Input.touches[0].position);
            LockOn(touchPosition);
        }

#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;

            Vector3 touchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            LockOn(touchPosition);
        }
#endif
    }

    private void LockOn(Vector3 touchPosition)
    {
        RaycastHit2D hit = Physics2D.Raycast(touchPosition, Vector2.zero);

        if (hit != false && hit.collider != null)
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                target = hit.collider.gameObject;
                isTargeting = true;
            }
            else if ((hit.collider.CompareTag("Enemy") && isTargeting) || hit.collider.CompareTag("Ground"))
            {
                target = null;
                isTargeting = false;
            }
        }
    }

    private void GetInputAxis()
    {
#if UNITY_EDITOR
        movementInput.x = Input.GetAxisRaw("Horizontal");
        movementInput.y = Input.GetAxisRaw("Vertical");
#endif
        
        if (joystickController.GetJoystickInput() != Vector2.zero)
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
