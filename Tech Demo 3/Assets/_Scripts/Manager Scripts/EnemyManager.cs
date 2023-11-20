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
    private bool sortedEnemies;

    private void Start()
    {
        EnemyController[] enemies = FindObjectsByType<EnemyController>(FindObjectsSortMode.None);

        foreach (EnemyController enemy in enemies)
        {
            AddEnemy(enemy); 
        }
    }

    private void Update()
    {
        if (!ReferenceManager.Instance.playerObject.GetComponent<PlayerController>().GetIsDead() && sortedEnemies)
            sortedEnemies = false;
        else if (ReferenceManager.Instance.playerObject.GetComponent<PlayerController>().GetIsDead() && !sortedEnemies)
        {

            sortedEnemies = true;
            for (int i = 0; i < enemiesList.Count; i++)
            {
                Vector2 enemyPos = new(enemiesList[i].transform.position.x, enemiesList[i].transform.position.y);

                if (enemiesList[i].GetHealth() < enemiesList[i].GetCharacterStats().GetBaseHealth() ||
                    enemyPos != enemiesList[i].GetStartingPosition())
                {
                    ResetEnemy(enemiesList[i]);
                }
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

    private void ResetEnemy(EnemyController enemy)
    {
        enemy.SetHealth(enemy.GetCharacterStats().GetBaseHealth());
        enemy.transform.position = enemy.GetStartingPosition();
    }
}
