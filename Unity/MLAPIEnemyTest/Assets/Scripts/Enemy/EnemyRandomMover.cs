using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

[Serializable]
public class EnemyRandomMover
{
    [SerializeField] private float moveSpeed = 10;
    [SerializeField] private float rotationSpeed = 2f;
    [SerializeField] private float minMoveNextPointDistance = 5;

    private Transform localTransform;
    private Vector3 enemyPosition;
    private Vector3 moveToPoint;
    private float minMoveNextPointDistanceSqr;

    public void Setup(Transform localTransform)
    {
        this.localTransform = localTransform;

        minMoveNextPointDistanceSqr = minMoveNextPointDistance * minMoveNextPointDistance;

        moveToPoint = EnemyManager.GetRandomSpawnAreaPoint();
    }

    public void Update()
    {
        enemyPosition = localTransform.position;

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

        Quaternion newRotation = Quaternion.Slerp(localTransform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        Vector3 rotationForward = newRotation * Vector3.forward;

        Vector3 newPosition = enemyPosition + (rotationForward * moveSpeed * Time.deltaTime);

        localTransform.SetPositionAndRotation(newPosition, newRotation);
    }

    private EnemyManager EnemyManager => EnemyManager.Instance;
}
