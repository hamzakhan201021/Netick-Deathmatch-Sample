using Netick.Transport;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LatencyDebug : MonoBehaviour
{
#if UNITY_EDITOR
    //https://revenantx.github.io/LiteNetLib/index.html
    static LatencyDebug instance;

    public bool SimulatePacketLoss;
    [Range(0, 100)]
    [Tooltip("(%)")]
    public int PacketLossChance;

    [Space(10)]

    [Tooltip("make sure these are the same on both client and server")]
    public bool SimulateLatency;
    [Range(0,300)]
    public int SimulationLatencyMs = 0;
    [Range(0, 15)]
    public int SimulationLatencyRange = 0;

    private bool IsServer = false;
    private bool Initialized = false;

    private void Awake()
    {
        instance = this;
        UpdateLatency();
    }

    private void OnValidate()
    {
        UpdateLatency();
    }

    public static void TransportInitialized(bool IsServer)
    {
        if (instance != null)
        {
            instance.Initialized = true;
            instance.IsServer = IsServer;
            instance.UpdateLatency();
        }
    }

    void UpdateLatency()
    {
        if (!Initialized)
            return;

        LNLTransportProviderLatency.transport._netManager.SimulatePacketLoss = SimulatePacketLoss;
        LNLTransportProviderLatency.transport._netManager.SimulationPacketLossChance = PacketLossChance;

        LNLTransportProviderLatency.transport._netManager.SimulateLatency = SimulateLatency;
        int latencyToConnection = SimulationLatencyMs / 2;
        int SimulationMinLatency = latencyToConnection - (SimulationLatencyRange / 2);
        SimulationMinLatency = Mathf.Max(SimulationMinLatency, 0);
        int SimulationMaxLatency = latencyToConnection + (SimulationLatencyRange / 2);
        LNLTransportProviderLatency.transport._netManager.SimulationMinLatency = SimulationMinLatency;
        LNLTransportProviderLatency.transport._netManager.SimulationMaxLatency = SimulationMaxLatency;
    }
#endif
}
