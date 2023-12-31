using UnityEngine;

/// <summary>
/// Base Class Character Animation Controller, holds the generic functionality and methods that
/// both player and enemy woud use
/// </summary>
public class CharacterAnimationController : MonoBehaviour
{
    protected Animator animator;
    protected SpriteRenderer spriteRenderer;
    protected string currentState;

    public Animator GetAnimator() => animator;

    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void ChangeAnimationState(string newState)
    {
        if (currentState == newState) return;

        animator.Play(newState);
        currentState = newState;
    }

    public bool IsAnimationPlaying(string state)
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName(state)
            && animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
            return true;
        else
            return false;
    }
}
