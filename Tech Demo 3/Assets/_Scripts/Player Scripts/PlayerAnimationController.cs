using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class PlayerAnimationController : MonoBehaviour
{
    const string IDLE = "Idle";
    const string WALK = "Walk";
    const string DEAD = "Dead";
    const string CAST = "Cast";
    const string MELEE_READY = "Melee_Ready";
    const string MELEE_SWING = "Melee_Swing";

    private Animator animator;
    private string currentState;

    private PlayerMovement playerMovement;
    private PlayerActions playerActions;
    private SpriteRenderer spriteRenderer;

    private Vector2 movementInput;
    private GameObject target;
    private bool hasTarget;

    private void Start()
    {
        animator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        playerActions = GetComponent<PlayerActions>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        movementInput = playerMovement.GetMovementInput();

        if (!playerActions.IsTargeting())
        {
            if (hasTarget)
            {
                hasTarget = false;
            }

            if (movementInput != Vector2.zero)
            {
                SetFacingDirection(movementInput.x);
                animator.SetFloat("MovementY", movementInput.y);
            }
        }
        else
        {
            if (!hasTarget)
            {
                hasTarget = true;
                target = playerActions.GetTarget();
            }

            if (target != null)
            {
                TargetSystem(target.transform, movementInput);
            }
        }

        if (movementInput != Vector2.zero)
        {
            ChangeAnimationState(WALK);
        }
        else
        {
            ChangeAnimationState(IDLE);
        }
    }

    private void SetFacingDirection(float facingDirection)
    {
        if (facingDirection != 0 && facingDirection < 0)
        {
            spriteRenderer.flipX = true;
        }
        else if (facingDirection != 0 && facingDirection > 0)
        {
            spriteRenderer.flipX = false;
        }
    }

    private void TargetSystem(Transform target, Vector2 movementInput)
    {
        // Above Enemy:
        if (transform.position.y > target.position.y)
        {
            movementInput.y = -1;
        }
        // Below Enemy:
        else if (transform.position.y < target.position.y)
        {
            movementInput.y = 1;
        }

        // Left of Enemy:
        if (transform.position.x < target.position.x)
        {
            movementInput.x = 1;
        }
        //Right of Enemy:
        if (transform.position.x > target.position.x)
        {
            movementInput.x = -1;
        }

        SetFacingDirection(movementInput.x);
        animator.SetFloat("MovementY", movementInput.y);
    }

    private void ChangeAnimationState(string newState)
    {
        if (currentState == newState)
        {
            return;
        }
        else
        {
            animator.Play(newState);
            currentState = newState;
        }
    }

    private bool IsAnimationPlaying(string state)
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName(state)
            && animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
