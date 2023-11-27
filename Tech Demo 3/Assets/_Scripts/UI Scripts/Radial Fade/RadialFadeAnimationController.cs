using UnityEngine;

/// <summary>
/// Controls the radial fade effect (Enlarging and decrementing transparent circle)
/// </summary>
public class RadialFadeAnimationController : MonoBehaviour
{
    private PlayerController playerController;

    private Animator animator;
    private GameObject childObject;

    private void Start()
    {
        playerController = ReferenceManager.Instance.playerObject.GetComponent<PlayerController>();

        animator = GetComponent<Animator>();
        childObject = transform.GetChild(0).gameObject;
        childObject.SetActive(false);
    }

    private void Update()
    {
        if (playerController.GetIsDead() && !animator.GetBool("isZoomIn"))
        {
            childObject.SetActive(true);
            animator.SetBool("isZoomIn", true);
        }
        else if (!playerController.GetIsDead() && animator.GetBool("isZoomIn"))
        {
            animator.SetBool("isZoomIn", false);
        }
    }

    public void DisableRadialFadeObject()
    {
        childObject.SetActive(false);
    }
}
