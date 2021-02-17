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

    [SerializeField] private int poolSize = 100;
    [SerializeField] private Enemy enemyPrefab = default;

    private Scene enemyScene;

    protected override void OnDestroy()
    {
        UnloadEnemyPool();

        base.OnDestroy();
    }

    public async UniTask LoadEnemiesPool(CancellationToken ct)
    {
        enemyScene = SceneManager.CreateScene(ENEMY_SCENE_NAME);

        for (int i = 0; i < poolSize; i++)
        {
            Enemy newEnemy = Instantiate<Enemy>(enemyPrefab);

            newEnemy.NetworkedObject.Spawn();

            SceneManager.MoveGameObjectToScene(newEnemy.gameObject, enemyScene);

            await UniTask.NextFrame().WithCancellation(ct);
        }
    }

    private void UnloadEnemyPool()
    {
        SceneManager.UnloadSceneAsync(enemyScene);
    }
}
