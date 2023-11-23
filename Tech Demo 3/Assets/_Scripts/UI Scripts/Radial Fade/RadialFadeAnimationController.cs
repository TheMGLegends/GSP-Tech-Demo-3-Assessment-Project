using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadialFadeAnimationController : MonoBehaviour
{
    private Animator animator;
    private GameObject childObject;

    private void Start()
    {
        animator = GetComponent<Animator>();
        childObject = transform.GetChild(0).gameObject;
        childObject.SetActive(false);
    }

    private void Update()
    {
        if (ReferenceManager.Instance.playerObject.GetComponent<PlayerController>().GetIsDead() && !animator.GetBool("isZoomIn"))
        {
            childObject.SetActive(true);
            animator.SetBool("isZoomIn", true);
        }
        else if (!ReferenceManager.Instance.playerObject.GetComponent<PlayerController>().GetIsDead() && animator.GetBool("isZoomIn"))
        {
            animator.SetBool("isZoomIn", false);
        }
    }

    public void DisableRadialFadeObject()
    {
        childObject.SetActive(false);
    }
}
