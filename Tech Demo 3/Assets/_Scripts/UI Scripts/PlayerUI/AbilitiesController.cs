using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilitiesController : MonoBehaviour
{
    [SerializeField] private GameObject castingBar;

    private void Start()
    {
        ActivateCastingUI(false);
    }

    public void ActivateCastingUI(bool activate)
    {
        if (activate)
        {
            castingBar.SetActive(true);
            gameObject.SetActive(true);
        }
        else
        {
            castingBar.SetActive(false);
            gameObject.SetActive(false);
        }
    }
}
