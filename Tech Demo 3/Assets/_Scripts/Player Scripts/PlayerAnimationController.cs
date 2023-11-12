using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

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
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        animator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        Vector2 movementInput = playerMovement.GetMovementInput();

        SetFacingDirection(movementInput.x);

        if (movementInput != Vector2.zero)
        {
            animator.SetFloat("MovementY", movementInput.y);

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
