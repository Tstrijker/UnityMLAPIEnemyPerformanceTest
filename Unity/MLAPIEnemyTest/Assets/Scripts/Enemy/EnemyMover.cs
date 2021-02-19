using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EnemyMover
{
    [SerializeField] private float moveSpeed = 100;
    [SerializeField] private float rotationSpeed = 0.5f;
    [SerializeField] private float minMoveNextPointDistance = 5; 

    private Transform transform;
    private Vector3 enemyPosition;
    private Vector3 moveToPoint;
    private float minMoveNextPointDistanceSqr;

    public void Setup(Transform transform)
    {
        this.transform = transform;

        minMoveNextPointDistanceSqr = minMoveNextPointDistance * minMoveNextPointDistance;
        moveToPoint = EnemyManager.GetRandomSpawnAreaPoint();
    }

    public void Update()
    {
        enemyPosition = transform.position;

        RandomMoveToPointUpdate();
        MoveToPoint();
    }

    private void RandomMoveToPointUpdate()
    {
        if (minMoveNextPointDistanceSqr < (moveToPoint - enemyPosition).sqrMagnitude)
            return;

        moveToPoint = EnemyManager.GetRandomSpawnAreaPoint();
    }

    private void MoveToPoint()
    {
        Vector3 targetDirection = moveToPoint - enemyPosition;
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

        Quaternion newRotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        Vector3 rotationForward = newRotation * Vector3.forward;

        Vector3 newPosition = enemyPosition + (rotationForward * moveSpeed * Time.deltaTime);

        transform.SetPositionAndRotation(newPosition, newRotation);
    }

    private EnemyManager EnemyManager => EnemyManager.Instance;
}
