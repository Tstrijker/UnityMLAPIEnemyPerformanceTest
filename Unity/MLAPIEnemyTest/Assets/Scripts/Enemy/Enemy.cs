using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;

public class Enemy : NetworkedBehaviour
{
    private void Awake()
    {
        GameFlowManager.MoveObjectToGameScene(gameObject);
    }
}
