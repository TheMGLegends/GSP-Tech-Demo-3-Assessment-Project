using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    private List<EnemyController> enemiesList = new();

    private void Start()
    {
        EnemyController[] enemies = FindObjectsByType<EnemyController>(FindObjectsSortMode.None);

        foreach (EnemyController enemy in enemies)
        {
            AddEnemy(enemy); 
        }
    }

    public void ResetEnemies()
    {
        for (int i = 0; i < enemiesList.Count; i++)
        {
            Vector2 enemyPos = new(enemiesList[i].transform.position.x, enemiesList[i].transform.position.y);

            if (enemiesList[i].GetHealth() < enemiesList[i].GetCharacterStats().GetBaseHealth() ||
                enemyPos != enemiesList[i].GetStartingPosition())
            {
                enemiesList[i].AfterPlayerDeath();
            }
        }
    }

    private void AddEnemy(EnemyController enemy)
    {
        enemiesList.Add(enemy);
    }

    public void RemoveEnemy(EnemyController enemy)
    {
        enemiesList.Remove(enemy);
    }
}
