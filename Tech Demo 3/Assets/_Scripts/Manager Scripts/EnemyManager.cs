using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages all the enemies present in the scene by putting them in an enemies list
/// if they die they get removed from this list
/// </summary>
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

    private readonly List<EnemyController> enemiesList = new();

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
            enemiesList[i].AfterPlayerDeath();
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
