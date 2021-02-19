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
    }

    private IEnumerator Start()
    {
        yield return EnemyManager.WaitForLoaded();

        EnemyManager.MoveObjectToGameScene(gameObject);
    }

    private void Update()
    {
        if (IsServer)
            ServerUpdate();
        else
            ClientUpdate();
    }

    private void ServerUpdate()
    {
        enemyMover.Update();

        
    }

    private void ClientUpdate()
    {
        
    }

    private EnemyManager EnemyManager => EnemyManager.Instance;
}
