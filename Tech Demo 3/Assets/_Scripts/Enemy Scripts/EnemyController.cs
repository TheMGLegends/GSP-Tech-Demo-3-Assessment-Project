using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyController : CharacterController
{
    [SerializeField] private EnemyHUDController enemyHUDController;

    public EnemyHUDController GetEnemyHUDController() => enemyHUDController;

    protected override void Start()
    {
        base.Start();
    }

    protected override void InitializeStats()
    {
        base.InitializeStats();

        enemyHUDController.InitializeBars(health);
    }
}
