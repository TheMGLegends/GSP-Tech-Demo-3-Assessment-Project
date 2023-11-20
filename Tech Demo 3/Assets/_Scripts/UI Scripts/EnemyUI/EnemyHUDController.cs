using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHUDController : CharacterHUDController
{
    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void DisplayProfile(bool isTargeting, GameObject target)
    {
        if (isTargeting)
        {
            SetHealth(target.GetComponent<EnemyController>().GetHealth());
            gameObject.SetActive(true);
        }
        else
            gameObject.SetActive(false);
    }
}
