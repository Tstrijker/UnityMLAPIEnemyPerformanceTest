using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EnemyPinpPongMover
{
    [SerializeField] private float moveSpeed = 10;
    [SerializeField] private Vector3 startPosition = new Vector3(80, 0, 0);
    [SerializeField] private Vector3 endPosition = new Vector3(-80, 0, 0);
    [SerializeField] private float turnAroundDistance = 1;

    private Transform localTransform;
    private Vector3 targetPosition;
    private float turnAroundDistanceSqr;
    private bool movingToEndPoint;

    public void Setup(Transform localTransform)
    {
        this.localTransform = localTransform;
        
        localTransform.position = startPosition;

        turnAroundDistanceSqr = turnAroundDistance * turnAroundDistance;

        targetPosition = endPosition;
        movingToEndPoint = true;

        localTransform.LookAt(targetPosition);
    }

    public void Update()
    {
        MoveUpdate();
    }

    private void MoveUpdate()
    {
        Vector3 position = localTransform.position;

        Vector3 newPosition = Vector3.MoveTowards(position, targetPosition, moveSpeed * Time.deltaTime);

        localTransform.position = newPosition;

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

            localTransform.LookAt(targetPosition);
        }
    }
}
