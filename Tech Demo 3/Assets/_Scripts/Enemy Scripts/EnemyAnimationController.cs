using UnityEngine;

/// <summary>
/// Holds animation controller functionality exclusive to the enemy e.g. (Enemy animation states)
/// </summary>
public class EnemyAnimationController : CharacterAnimationController
{
    public const string IDLE = "Idle";
    public const string RANGED_ATTACK = "Ranged_Attack";
    public const string DEAD = "Dead";

    private EnemyController enemyController;

    private Vector2 movementDirection;
    private GameObject target;

    public void SetMovementDirection(Vector2 movementDirection) {  this.movementDirection = movementDirection; }

    protected override void Start()
    {
        base.Start();
        enemyController = GetComponent<EnemyController>();
    }

    private void Update()
    {
        if (target != enemyController.GetTarget())
        {
            target = enemyController.GetTarget();
        }

        if (target == null)
        {
            SetFacingDirection(movementDirection.x);
            animator.SetFloat("MovementY", movementDirection.y);
        }
        else
            TargetSystem(target.transform);
    }
    
    private void TargetSystem(Transform target)
    {
        // Above Target:
        if (transform.position.y > target.position.y)
            movementDirection.y = -1;
    
        // Below Target:
        else if (transform.position.y < target.position.y)
            movementDirection.y = 1;
    
        // Left of Target:
        if (transform.position.x < target.position.x)
            movementDirection.x = 1;
    
        //Right of Target:
        if (transform.position.x > target.position.x)
            movementDirection.x = -1;
    
        SetFacingDirection(movementDirection.x);
        animator.SetFloat("MovementY", movementDirection.y);
    }
    
    public void SetFacingDirection(float facingDirection)
    {
        if (facingDirection != 0 && facingDirection > 0)
            spriteRenderer.flipX = true;
        else if (facingDirection != 0 && facingDirection < 0)
            spriteRenderer.flipX = false;
    }
}
