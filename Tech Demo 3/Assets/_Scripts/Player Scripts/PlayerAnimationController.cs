using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    public enum AnimationStates
    {
        Idle,
        Walk,
        Melee_Ready,
        Melee_Swing,
        Cast,
        Dead
    }

    public List<AnimationStates> animationStatesList = new();
    [SerializeField] private List<string> animationStringsList = new();
    [SerializeField] private Dictionary<AnimationStates, string> animationDictionary = new();

    public void ChangeAnimationState(AnimationStates newState)
    {
        if (currentState == newState) return;

        if (animationDictionary.ContainsKey(newState))
        {
            animator.Play(animationDictionary[newState]);
            currentState = newState;
        }
    }

    public bool IsAnimationPlaying(Animator animator, AnimationStates state)
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName(animationDictionary[state])
            && animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private Animator animator;
    private AnimationStates currentState;

    private PlayerController playerController;
    private SpriteRenderer spriteRenderer;

    private Vector2 movementInput;
    private GameObject target;

    private void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        playerController = GetComponent<PlayerController>();

        for (int i = 0; i < animationStatesList.Count; i++)
        {
            animationDictionary.Add(animationStatesList[i], animationStringsList[i]);
        }
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
        {
            TargetSystem(target.transform, movementInput);
        }

        if (movementInput != Vector2.zero)
            ChangeAnimationState(AnimationStates.Walk);
        else
            ChangeAnimationState(AnimationStates.Idle);
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
