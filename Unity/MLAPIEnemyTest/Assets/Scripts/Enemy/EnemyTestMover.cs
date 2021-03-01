using MLAPI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTestMover : NetworkedBehaviour
{
    [SerializeField] private float moveSpeed = 10;
    [SerializeField] private Vector3 startPosition = new Vector3(80, 0, 0);
    [SerializeField] private Vector3 endPosition = new Vector3(-80, 0, 0);
    [SerializeField] private float turnAroundDistance = 1;

    private Vector3 targetPosition;
    private float turnAroundDistanceSqr;
    private bool movingToEndPoint;

    private void Awake()
    {
        if (IsServer)
            transform.position = startPosition;

        turnAroundDistanceSqr = turnAroundDistance * turnAroundDistance;

        targetPosition = endPosition;
        movingToEndPoint = true;
        transform.LookAt(targetPosition);
    }

    private void Update()
    {
        if (!IsServer)
            return;

        MoveUpdate();
    }

    private void MoveUpdate()
    {
        Vector3 position = transform.position;

        Vector3 newPosition = Vector3.MoveTowards(position, targetPosition, moveSpeed * Time.deltaTime);

        transform.position = newPosition;

        if ((targetPosition - newPosition).sqrMagnitude < turnAroundDistanceSqr)
        {
            if (movingToEndPoint)
            {
                targetPosition = startPosition;
                movingToEndPoint = false;
            }
            else
            {
                targetPosition = endPosition;
                movingToEndPoint = true;
            }

            transform.LookAt(targetPosition);
        }
    }
}
