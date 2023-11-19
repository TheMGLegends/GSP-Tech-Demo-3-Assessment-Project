using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    public static AnimationManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    public void TargetSystem(Transform target, Vector2 movementInput, Animator animator, SpriteRenderer spriteRenderer)
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

        SetFacingDirection(movementInput.x, spriteRenderer);
        animator.SetFloat("MovementY", movementInput.y);
    }

    public void SetFacingDirection(float facingDirection, SpriteRenderer spriteRenderer)
    {
        if (facingDirection != 0 && facingDirection < 0)
            spriteRenderer.flipX = true;
        else if (facingDirection != 0 && facingDirection > 0)
            spriteRenderer.flipX = false;
    }

    public enum PlayerAnimStates
	{
        Idle,
        Walk,
        Melee_Ready,
        Melee_Swing,
        Cast,
        Dead
    }

    public List<PlayerAnimStates> playerAnimStatesList = new();
    [SerializeField] private List<string> playerAnimStringsList = new();
    [SerializeField] private Dictionary<PlayerAnimStates, string> playerAnimLib = new();



    public enum EnemyAnimStates
    {
        Idle,
        Ranged_Attack,
        Dead
    }

    public List<EnemyAnimStates> enemyAnimStatesList = new();
    [SerializeField] private List<string> enemyAnimStringsList = new();
    [SerializeField] private Dictionary<EnemyAnimStates, string> enemyAnimDictionary = new();
}
