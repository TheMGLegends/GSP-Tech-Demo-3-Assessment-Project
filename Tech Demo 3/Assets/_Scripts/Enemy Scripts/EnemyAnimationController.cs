using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationController : AnimationController
{
    public const string IDLE = "Idle";
    public const string RANGED_ATTACK = "Ranged_Attack";
    public const string DEAD = "Dead";

    private EnemyController enemyController;

    private Vector2 movementInput;
    private GameObject target;

    protected override void Start()
    {
        base.Start();
        enemyController = GetComponent<EnemyController>();
    }

    private void Update()
    {
        
    }
    
    private void TargetSystem(Transform target, Vector2 movementInput)
    {
        // Above Target:
        if (transform.position.y > target.position.y)
            movementInput.y = -1;
    
        // Below Target:
        else if (transform.position.y < target.position.y)
            movementInput.y = 1;
    
        // Left of Target:
        if (transform.position.x < target.position.x)
            movementInput.x = 1;
    
        //Right of Target:
        if (transform.position.x > target.position.x)
            movementInput.x = -1;
    
        SetFacingDirection(movementInput.x);
        animator.SetFloat("MovementY", movementInput.y);
    }
    
    private void SetFacingDirection(float facingDirection)
    {
        if (facingDirection != 0 && facingDirection < 0)
            spriteRenderer.flipX = true;
        else if (facingDirection != 0 && facingDirection > 0)
            spriteRenderer.flipX = false;
    }
}
