using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;

public class Enemy : NetworkedBehaviour
{
    [SerializeField] private EnemyPinpPongMover enemyPinpPongMover = default;
    [SerializeField] private EnemyRandomMover enemyRandomMover = default;

    private bool enemyStarted = false;

    private void Awake()
    {
        GameFlowManager.MoveObjectToGameScene(gameObject);
    }

    private IEnumerator Start()
    {
        if (!IsServer)
            yield break;

        yield return EnemyManager.WaitForLoaded();

        switch (GameSettingsData.enemyMoveType)
        {
            case EnemyMoveTypes.PinpPong:
                enemyPinpPongMover.Setup(transform);
                break;

            case EnemyMoveTypes.RandomDirection:
                enemyRandomMover.Setup(transform);
                break;
        }
    
        enemyStarted = true;
    }

    private void Update()
    {
        if (!IsServer)
            return;

        if (!enemyStarted)
            return;

        switch (GameSettingsData.enemyMoveType)
        {
            case EnemyMoveTypes.PinpPong:
                enemyPinpPongMover.Update();
                break;

            case EnemyMoveTypes.RandomDirection:
                enemyRandomMover.Update();
                break;
        }
    }

    private GameSettingsData GameSettingsData => GameSettingsManager.Instance.Settings;
}
