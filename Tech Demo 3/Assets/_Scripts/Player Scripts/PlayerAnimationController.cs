using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : CharacterAnimationController
{
    public const string IDLE = "Idle";
    public const string WALK = "Walk";
    public const string MELEE_READY = "Melee_Ready";
    public const string MELEE_SWING = "Melee_Swing";
    public const string CAST = "Cast";
    public const string DEAD = "Dead";

    private PlayerController playerController;

    private Vector2 movementInput;
    private GameObject target;

    protected override void Start()
    {
        base.Start();
        playerController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        movementInput = playerController.GetMovementInput();

        if (target != playerController.GetTarget())
        {
            target = playerController.GetTarget();
        }

        if (target == null)
        {
            if (movementInput != Vector2.zero)
            {
                SetFacingDirection(movementInput.x);
                animator.SetFloat("MovementY", movementInput.y);
            }
        }
        else
            TargetSystem(target.transform, movementInput);
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

    public void SetFacingDirection(float facingDirection)
    {
        if (facingDirection != 0 && facingDirection < 0) 
            spriteRenderer.flipX = true;
        else if (facingDirection != 0 && facingDirection > 0) 
            spriteRenderer.flipX = false;
    }
}
