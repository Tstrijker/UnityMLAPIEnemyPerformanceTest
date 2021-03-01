using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyManager : SceneSingleton<EnemyManager>
{
    [SerializeField] private Enemy enemyMLAPIPredictionMovementPrefab = default;
    [SerializeField] private Enemy enemySinglePredictionMovementPrefab2 = default;
    [SerializeField] private Enemy enemyGroupedPredictionMovementPrefab3 = default;
    [SerializeField] private int poolSize = 100;
    [SerializeField] private Bounds spawnAreaSize = default;
    [SerializeField] private MovementPredictionManager movementPredictionManagerPrefab = default;

    public async UniTask LoadEnemiesPool(MovementPredictionTypes movementPredictionType, CancellationToken ct)
    {
        Enemy enemyPrefab = null;

        switch (movementPredictionType)
        {
            case MovementPredictionTypes.MLAPIPredictionMovement:
                enemyPrefab = enemyMLAPIPredictionMovementPrefab;
                break;

            case MovementPredictionTypes.SinglePredictionMovement:
                enemyPrefab = enemySinglePredictionMovementPrefab2;
                break;

            case MovementPredictionTypes.GroupedPredictionMovement:
                MovementPredictionManager movementPredictionManager = Instantiate(movementPredictionManagerPrefab);
                movementPredictionManager.NetworkedObject.Spawn();

                enemyPrefab = enemyGroupedPredictionMovementPrefab3;
                break;
        }

        for (int i = 0; i < poolSize; i++)
        {
            Vector3 spawnPoint = GetRandomSpawnAreaPoint();
            Quaternion rotation = Quaternion.AngleAxis(Random.Range(0, 360), Vector3.up);

            Enemy newEnemy = Instantiate<Enemy>(enemyPrefab, spawnPoint, rotation);

            newEnemy.NetworkedObject.Spawn();

            await UniTask.NextFrame().WithCancellation(ct);
        }
    }

    public Vector3 GetRandomSpawnAreaPoint()
    {
        return spawnAreaSize.GetRandomVector3();
    }
}
