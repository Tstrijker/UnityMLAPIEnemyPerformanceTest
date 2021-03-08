using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyManager : SceneSingleton<EnemyManager>
{
    [SerializeField] private Enemy enemyMLAPINetworkedTransformPrefab = default;
    [SerializeField] private Enemy enemyPredictionMovementPrefab = default;
    [SerializeField] private Bounds spawnAreaSize = default;
    [SerializeField] private MovementPredictionManager movementPredictionManagerPrefab = default;

    public async UniTask LoadEnemiesPool(CancellationToken ct)
    {
        Enemy enemyPrefab = null;

        switch (GameSettingsData.predictionType)
        {
            case MovementPredictionTypes.MLAPINetworkedTransform:
                enemyPrefab = enemyMLAPINetworkedTransformPrefab;
                break;

            case MovementPredictionTypes.Linear:
            case MovementPredictionTypes.CubicHermite:
                if (GameSettingsData.movementPredictionData == MovementPredictionDataTypes.Grouped)
                {
                    MovementPredictionManager movementPredictionManager = Instantiate(movementPredictionManagerPrefab);
                    movementPredictionManager.NetworkedObject.Spawn();
                }

                enemyPrefab = enemyPredictionMovementPrefab;
                break;
        }

        for (int i = 0; i < GameSettingsData.spawnNumberOfEnemies; i++)
        {
            Vector3 spawnPoint = GetRandomSpawnAreaPoint();
            Quaternion rotation = Quaternion.AngleAxis(Random.Range(0, 360), Vector3.up);

            Enemy newEnemy = Instantiate(enemyPrefab, spawnPoint, rotation);

            newEnemy.NetworkedObject.Spawn();

            await UniTask.NextFrame().WithCancellation(ct);
        }
    }

    public Vector3 GetRandomSpawnAreaPoint()
    {
        return spawnAreaSize.GetRandomVector3();
    }

    private GameSettingsData GameSettingsData => GameSettingsManager.Instance.Settings;
}
