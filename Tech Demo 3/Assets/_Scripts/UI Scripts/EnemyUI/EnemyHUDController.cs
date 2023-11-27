using UnityEngine;

/// <summary>
/// Displays the corresponding enemies profile e.g. (Health, status effects etc.)
/// </summary>
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
