using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Serialization.Pooled;
using MLAPI.Messaging;
using System.IO;

public class MovementPredictionManager : NetworkedBehaviour
{
    [Header("Settings")]
    [SerializeField] private int sendRatePerSecond = default;

    [Header("Test features")]
    [SerializeField] private bool dropPackages = default;
    [SerializeField] private int dropEachPackCount = default;

    private float nextSendTime = 0;
    private int dropPackCount = 0;
    private byte movementNetworkIdCounter = 0;

    private Dictionary<byte, MovementPrediction> handlers = new Dictionary<byte, MovementPrediction>();

    public static MovementPredictionManager Instance { get; private set; }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (Instance == null)
            Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    private void Update()
    {
        SendMovementDataUpdate();
    }

    public byte AddHandlerOnServer(MovementPrediction predictionHandler)
    {
        byte movementNetworkId = movementNetworkIdCounter;

        movementNetworkIdCounter++;

        handlers.Add(movementNetworkId, predictionHandler);

        return movementNetworkId;
    }

    public void AddHandlerOnClient(byte movementNetworkId , MovementPrediction predictionHandler)
    {
        handlers.Add(movementNetworkId, predictionHandler);
    }

    public void RemoveHandler(byte movementNetworkId)
    {
        handlers.Remove(movementNetworkId);
    }

    // Network data handling
    private void SendMovementDataUpdate()
    {
        // Don't send any data if where not the owner of the network object
        if (!IsOwner)
            return;

        if (handlers.Count == 0)
            return;

        // Check if the it time to send data again
        if (Time.time < nextSendTime)
            return;

        nextSendTime = Time.time + (1f / sendRatePerSecond);

        using (PooledBitStream stream = PooledBitStream.Get())
        {
            using (PooledBitWriter writer = PooledBitWriter.Get(stream))
            {
                foreach (var handler in handlers)
                {
                    writer.WriteByte(handler.Key);
                    writer.WriteSinglePacked(NetworkTime);
                    writer.WriteVector3Packed(handler.Value.transform.position);
                    writer.WriteRotationPacked(handler.Value.transform.rotation);
                }

                InvokeServerRpcPerformance(ServerReceivedMovementDataRPC, stream);
            }
        }
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
        using (PooledBitReader reader = PooledBitReader.Get(stream))
        {
            for (int i = 0; i < handlers.Count; i++)
            {
                byte movementNetworkId = (byte)reader.ReadByte();
                float timeStamp = reader.ReadSinglePacked();
                Vector3 position = reader.ReadVector3Packed();
                Quaternion rotation = reader.ReadRotationPacked();

                if (handlers.ContainsKey(movementNetworkId))
                    handlers[movementNetworkId].AddMovementDataToBuffer(timeStamp, position, rotation);
            }
        }
    }

    private float NetworkTime => NetworkingManager.Singleton.NetworkTime;
}
