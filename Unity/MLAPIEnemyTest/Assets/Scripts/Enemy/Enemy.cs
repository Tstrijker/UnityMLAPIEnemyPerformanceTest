using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;

public class Enemy : NetworkedBehaviour
{
    private EnemyMover enemyMover;

    private void Awake()
    {
        enemyMover = new EnemyMover();
    }


}
