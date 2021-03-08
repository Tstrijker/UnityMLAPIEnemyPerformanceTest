using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Serialization.Pooled;
using MLAPI.Messaging;
using System.IO;

public class MovementPredictionCubicHermiteSpline : NetworkedBehaviour
{
    [Header("Settings")]
    [SerializeField] private int sendRatePerSecond = default;
    [SerializeField] private float bufferWaitTime = default;

    [Header("Test features")]
    [SerializeField] private bool dropPackages = default;
    [SerializeField] private int dropEachPackCount = default;

    private float nextSendTime = 0;
    private int dropPackCount = 0;
    private float lastTimeStamp = 0;
    private Vector3 lastVelocityPosition;
    private float lastVeclocityTimeStamp;
    private Queue<MovementData> buffer = new Queue<MovementData>();
    private MovementData? moveFromData = null;
    private MovementData? moveToData = null;

    public override void NetworkStart()
    {
        lastVelocityPosition = transform.position;
        lastVeclocityTimeStamp = Time.time;
    }

    private void Update()
    {
        SendMovementDataUpdate();
        MoveUpdate();
    }

    // Network data handling
    private void SendMovementDataUpdate()
    {
        // Don't send any data if where not the owner of the network object
        if (!IsOwner)
            return;

        // Check if the it time to send data again
        if (Time.time < nextSendTime)
            return;

        nextSendTime = Time.time + (1f / sendRatePerSecond);

        using (PooledBitStream stream = PooledBitStream.Get())
        {
            using (PooledBitWriter writer = PooledBitWriter.Get(stream))
            {
                Vector3 velocity = GetVelocity();

                writer.WriteSinglePacked(NetworkTime);
                writer.WriteVector3Packed(transform.position);
                writer.WriteRotationPacked(transform.rotation);
                writer.WriteVector3Packed(velocity);

                InvokeServerRpcPerformance(ServerReceivedMovementDataRPC, stream);
            }
        }
    }

    private Vector3 GetVelocity()
    {
        float deltaTime = Time.time - lastVeclocityTimeStamp;

        Vector3 velocity = (transform.position - lastVelocityPosition) / deltaTime;

        lastVelocityPosition = transform.position;
        lastVeclocityTimeStamp = Time.time;

        return velocity;
    }

    [ServerRPC]
    private void ServerReceivedMovementDataRPC(ulong clientId, Stream stream)
    {
        if (dropPackages)
        {
            if (dropPackCount >= dropEachPackCount)
            {
                dropPackCount = 0;
                return;
            }
            else
            {
                dropPackCount++;
            }
        }

        InvokeClientRpcOnEveryoneExceptPerformance(ClientReceivedMovementDataRPC, clientId, stream);
    }

    [ClientRPC]
    private void ClientReceivedMovementDataRPC(ulong clientId, Stream stream)
    {
        MovementData movementData = new MovementData();

        using (PooledBitReader reader = PooledBitReader.Get(stream))
        {
            movementData.timeStamp = reader.ReadSinglePacked() + bufferWaitTime;
            movementData.position = reader.ReadVector3Packed();
            movementData.rotation = reader.ReadRotationPacked();
            movementData.velocity = reader.ReadVector3Packed();
        }

        buffer.Enqueue(movementData);
    }

    // Client movement cacalution
    private void MoveUpdate()
    {
        if (IsOwner)
            return;

        if (buffer.Count < 2)
            return;

        if (moveFromData == null)
            moveFromData = buffer.Dequeue();

        if (moveToData == null)
            moveToData = buffer.Dequeue();

        if (NetworkTime < moveFromData.Value.timeStamp)
            return;

        if (NetworkTime > moveToData.Value.timeStamp)
        {
            if (buffer.Count == 0)
                return;

            moveFromData = moveToData;

            moveToData = buffer.Dequeue();
        }

        float deltaMoveTime = moveToData.Value.timeStamp - moveFromData.Value.timeStamp;
        float currentMoveTime = NetworkTime - moveFromData.Value.timeStamp;

        float normalMoveTime = currentMoveTime / deltaMoveTime;

        Vector3 moveFromTangent = moveFromData.Value.velocity * deltaMoveTime;
        Vector3 moveToDataangent = moveToData.Value.velocity * deltaMoveTime;

        Vector3 newPosition = GetHermiteUnitInterval(
            moveFromData.Value.position, 
            moveToData.Value.position,
            moveFromTangent,
            moveToDataangent,
            normalMoveTime);

        Quaternion newRotation = Quaternion.Slerp(moveFromData.Value.rotation, moveToData.Value.rotation, normalMoveTime);

        transform.SetPositionAndRotation(newPosition, newRotation);
    }

    private Vector3 GetHermiteUnitInterval(
        Vector3 aP, 
        Vector3 bP, 
        Vector3 aT, 
        Vector3 bT, 
        float t)
    {
        float t2 = t * t;
        float t3 = t2 * t;

        return (2f * t3 - 3f * t2 + 1f) * aP
            + (t3 - 2f * t2 + t) * aT
            + (-2f * t3 + 3f * t2) * bP
            + (t3 - t2) * bT;
    }

    private float NetworkTime => NetworkingManager.Singleton.NetworkTime;
}
