using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyManager : SceneSingleton<EnemyManager>
{
    private const string ENEMY_SCENE_NAME = "Enemies";

    [SerializeField] private Enemy enemyMLAPIPredictionMovementPrefab = default;
    [SerializeField] private Enemy enemySinglePredictionMovementPrefab2 = default;
    [SerializeField] private Enemy enemyGroupedPredictionMovementPrefab3 = default;
    [SerializeField] private int poolSize = 100;
    [SerializeField] private Bounds spawnAreaSize = default;
    [SerializeField] private MovementPredictionManager movementPredictionManagerPrefab = default;

    private static Scene? enemyScene;

    protected override void OnDestroy()
    {
        UnloadEnemyPool();

        base.OnDestroy();
    }

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

    public static void MoveObjectToGameScene(GameObject gameObject)
    {
        if (enemyScene == null)
            enemyScene = SceneManager.CreateScene(ENEMY_SCENE_NAME);

        SceneManager.MoveGameObjectToScene(gameObject, enemyScene.Value);
    }

    public Vector3 GetRandomSpawnAreaPoint()
    {
        return spawnAreaSize.GetRandomVector3();
    }

    private void UnloadEnemyPool()
    {
        SceneManager.UnloadSceneAsync(enemyScene.Value);
    }
}
