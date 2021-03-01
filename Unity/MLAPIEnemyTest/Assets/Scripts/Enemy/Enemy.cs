using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;

public class Enemy : NetworkedBehaviour
{
    private void Awake()
    {
        EnemyManager.MoveObjectToGameScene(gameObject);
    }

    private EnemyManager EnemyManager => EnemyManager.Instance;
}
