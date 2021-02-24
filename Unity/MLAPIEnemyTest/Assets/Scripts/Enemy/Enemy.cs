using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.LagCompensation;
using MLAPI.NetworkedVar;

public class Enemy : NetworkedBehaviour
{
    [SerializeField] private EnemyMover enemyMover = default;

    private void Awake()
    {
        if (IsServer)
            enemyMover.Setup(transform);

        EnemyManager.MoveObjectToGameScene(gameObject);
    }

    private void Update()
    {
        if (IsServer)
            ServerUpdate();
    }

    private void ServerUpdate()
    {
        enemyMover.Update();
    }

    private EnemyManager EnemyManager => EnemyManager.Instance;
}
