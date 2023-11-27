using UnityEngine;

/// <summary>
/// Character base class controller that holds generic functionality that both player
/// and enemy require such as health stats, movement speed, dead state etc.
/// </summary>
public class CharacterBaseController : MonoBehaviour
{
    [SerializeField] protected CharacterStatsSO characterStats;

    protected float health;
    protected float movementSpeed;
    protected float defenseMultiplier;
    protected float normalDamageAmount;
    protected float normalAttackInterval;

    protected bool canAttack;

    protected Vector3 startingPosition;
    protected bool isDead;

    protected CharacterAnimationController characterAnimationController;
    protected CharacterHUDController characterHUDController;
    protected StatusEffectController statusEffectController;

    protected GameObject target;

    public float GetHealth() => health;
    public float GetMovementSpeed() => movementSpeed;
    public float GetDefenseMultiplier() => defenseMultiplier;
    public bool GetIsDead() => isDead;
    public CharacterAnimationController GetCharacterAnimationController() => characterAnimationController;
    public virtual CharacterHUDController GetCharacterHUDController() => characterHUDController;
    public GameObject GetTarget() => target;
    public Vector2 GetStartingPosition() => startingPosition;
    public CharacterStatsSO GetCharacterStats() => characterStats;
    public void SetTarget(GameObject target) { this.target = target; }
    public void SetCanAttack(bool canAttack) { this.canAttack = canAttack; }
    public void SetMovementSpeed(float movementSpeed) {  this.movementSpeed = movementSpeed; }
    public void SetDefenseMultiplier(float defenseMultiplier) { this.defenseMultiplier = defenseMultiplier; }

    private void Awake()
    {
        InitializeStats();
    }

    protected virtual void Start()
    {
        startingPosition = transform.position;
        characterAnimationController = GetComponent<CharacterAnimationController>();
        statusEffectController = GetComponent<StatusEffectController>();
    }

    protected virtual void InitializeStats()
    {
        health = characterStats.GetBaseHealth();
        movementSpeed = characterStats.GetBaseMovementSpeed();
        defenseMultiplier = characterStats.GetBaseDefenseMultiplier();
        normalDamageAmount = characterStats.GetNormalDamage();
        normalAttackInterval = characterStats.GetNormalAttackSpeed();
    }

    public virtual void ReduceHealth(float damage)
    {
        health -= damage;

        if (health <= 0)
        {
            isDead = true;
            DeathAction();
        }
    }

    public virtual void ReduceMana(float manaCost)
    {
    }

    protected virtual void DeathAction()
    {
    }

    protected virtual void AfterDeath()
    {
        isDead = false;
    }
}
