using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Serialization.Pooled;
using MLAPI.Messaging;
using System.IO;
using MLAPI.NetworkedVar;

public class MovementPrediction : NetworkedBehaviour
{
    private float nextSendTime = 0;
    private int dropPackCount = 0;
    private Queue<MovementData> buffer = new Queue<MovementData>();
    private MovementData? moveFromData = null;
    private MovementData? moveToData = null;

    private NetworkedVar<byte> movementNetworkId = new NetworkedVar<byte>();

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
        if (GameSettingsData.movementPredictionData == MovementPredictionDataTypes.Local)
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

        nextSendTime = Time.time + (1f / GameSettingsData.sendRatePerSecond);

        using (PooledBitStream stream = PooledBitStream.Get())
        {
            using (PooledBitWriter writer = PooledBitWriter.Get(stream))
            {
                writer.WriteSinglePacked(NetworkTime);
                writer.WriteVector3Packed(transform.position);
                writer.WriteRotationPacked(transform.rotation);

                InvokeServerRpcPerformance(ServerReceivedMovementDataRPC, stream);
            }
        }
    }
    
    [ServerRPC]
    private void ServerReceivedMovementDataRPC(ulong clientId, Stream stream)
    {
        if (GameSettingsData.simulateDropingPackages)
        {
            if (dropPackCount >= GameSettingsData.dropEachSetNumberPackage)
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
            movementData.timeStamp = reader.ReadSinglePacked() + GameSettingsData.bufferWaitTime;
            movementData.position = reader.ReadVector3Packed();
            movementData.rotation = reader.ReadRotationPacked();
        }

        buffer.Enqueue(movementData);
    }

    public void AddMovementDataToBuffer(float timeStamp, Vector3 position, Quaternion rotation)
    {
        MovementData movementData = new MovementData();

        movementData.timeStamp = timeStamp + GameSettingsData.bufferWaitTime;
        movementData.position = position;
        movementData.rotation = rotation;

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

        Vector3 newPosition = Vector3.Lerp(moveFromData.Value.position, moveToData.Value.position, normalMoveTime);

        if (GameSettingsData.predictionType == MovementPredictionTypes.Linear)
        {
            newPosition = Vector3.Lerp(
                moveFromData.Value.position, 
                moveToData.Value.position, 
                normalMoveTime);
        }
        else if (GameSettingsData.predictionType == MovementPredictionTypes.CubicHermite)
        {
            Vector3 moveFromTangent = moveFromData.Value.velocity * deltaMoveTime;
            Vector3 moveToDataangent = moveToData.Value.velocity * deltaMoveTime;

            newPosition = GetHermiteUnitInterval(
            moveFromData.Value.position,
            moveToData.Value.position,
            moveFromTangent,
            moveToDataangent,
            normalMoveTime);
        }

        Quaternion newRotation = Quaternion.Lerp(moveFromData.Value.rotation, moveToData.Value.rotation, normalMoveTime);
        
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
    private GameSettingsData GameSettingsData => GameSettingsManager.Instance.Settings;
    private MovementPredictionManager MovementPredictionManager => MovementPredictionManager.Instance;
}
