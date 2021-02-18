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

    [SerializeField] private Enemy enemyPrefab = default;
    [SerializeField] private int poolSize = 100;
    [SerializeField] private Bounds spawnAreaSize = default;

    private Scene enemyScene;

    protected override void Awake()
    {
        base.Awake();

        enemyScene = SceneManager.CreateScene(ENEMY_SCENE_NAME);
    }

    protected override void OnDestroy()
    {
        UnloadEnemyPool();

        base.OnDestroy();
    }

    public async UniTask LoadEnemiesPool(CancellationToken ct)
    {
        for (int i = 0; i < poolSize; i++)
        {
            Vector3 spawnPoint = spawnAreaSize.GetRandomVector3();
            Quaternion rotation = Quaternion.AngleAxis(Random.Range(0, 360), Vector3.up);

            Enemy newEnemy = Instantiate<Enemy>(enemyPrefab, spawnPoint, rotation);

            newEnemy.NetworkedObject.Spawn();

            await UniTask.NextFrame().WithCancellation(ct);
        }
    }

    public void MoveObjectToGameScene(GameObject gameObject)
    {
        SceneManager.MoveGameObjectToScene(gameObject, enemyScene);
    }

    private void UnloadEnemyPool()
    {
        SceneManager.UnloadSceneAsync(enemyScene);
    }
}
