using MLAPI;
using MLAPI.NetworkedVar;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementPredictionHandler : NetworkedBehaviour
{
    [SerializeField] private float bufferWaitTime = default;

    private NetworkedVar<byte> movementNetworkId = new NetworkedVar<byte>();
    private Queue<MovementData> buffer = new Queue<MovementData>();
    private MovementData? moveFromData = null;
    private MovementData? moveToData = null;

    private IEnumerator Start()
    {
        while (MovementPredictionManager == null)
            yield return null;

        if (NetworkedObject.IsOwner)
            movementNetworkId.Value = MovementPredictionManager.AddHandlerOnServer(this);
        else
            MovementPredictionManager.AddHandlerOnClient(movementNetworkId.Value, this);
    }

    private void OnDestroy()
    {
        if (MovementPredictionManager != null)
            MovementPredictionManager.RemoveHandler(movementNetworkId.Value);
    }

    private void Update()
    {
        MoveUpdate();
    }

    // Client movement cacalution
    public void AddMovementDataToBuffer(Vector3 position, Quaternion rotation)
    {
        MovementData movementData = new MovementData();

        movementData.timeStamp = Time.time + bufferWaitTime;
        movementData.position = position;
        movementData.rotation = rotation;

        buffer.Enqueue(movementData);
    }

    private void MoveUpdate()
    {
        if (NetworkedObject.IsOwner)
            return;

        if (buffer.Count < 2)
            return;

        if (moveFromData == null)
            moveFromData = buffer.Dequeue();

        if (moveToData == null)
            moveToData = buffer.Dequeue();

        if (Time.time < moveFromData.Value.timeStamp)
            return;

        if (Time.time > moveToData.Value.timeStamp)
        {
            if (buffer.Count == 0)
                return;

            moveFromData = moveToData;

            moveToData = buffer.Dequeue();
        }

        float deltaMoveTime = moveToData.Value.timeStamp - moveFromData.Value.timeStamp;
        float currentMoveTime = Time.time - moveFromData.Value.timeStamp;

        float normalMoveTime = currentMoveTime / deltaMoveTime;

        Vector3 newPosition = Vector3.Slerp(moveFromData.Value.position, moveToData.Value.position, normalMoveTime);
        Quaternion newRotation = Quaternion.Slerp(moveFromData.Value.rotation, moveToData.Value.rotation, normalMoveTime);

        transform.SetPositionAndRotation(newPosition, newRotation);
    }

    private MovementPredictionManager MovementPredictionManager => MovementPredictionManager.Instance;
}
