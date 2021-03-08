using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettingsData
{
    public EnemyMoveTypes enemyMoveType = EnemyMoveTypes.RandomDirection;
    public MovementPredictionTypes predictionType = MovementPredictionTypes.Linear;
    public MovementPredictionDataTypes movementPredictionData = MovementPredictionDataTypes.Local;
    public int sendRatePerSecond = 20;
    public float bufferWaitTime = 0.5f;
    public int spawnNumberOfEnemies = 100;
    public bool simulateDropingPackages = false;
    public int dropEachSetNumberPackage = 10;
}
