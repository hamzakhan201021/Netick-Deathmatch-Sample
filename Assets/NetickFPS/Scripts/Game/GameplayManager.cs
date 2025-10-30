using System.Collections.Generic;
using Netick;
using Netick.Unity;
using UnityEngine;
using UnityEngine.Events;

public class GameplayManager : NetworkEventsListener
{

    public List<NetworkPlayer> NetworkPlayers = new List<NetworkPlayer>();
    public UnityEvent<NetworkPlayer> OnPlayerConnectedCallback;
    public UnityEvent<NetworkPlayer> OnPlayerDisconnectedCallback;
    public static GameplayManager Instance;
    public override void OnStartup(NetworkSandbox sandbox)
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);

            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        // dont update when sandbox is null
        if (Sandbox == null) return;
        //Debug.Log("Players" + Sandbox.ConnectedPlayers.Count);           
    }

    //public override void OnPlayerConnected(NetworkSandbox sandbox, Netick.NetworkPlayer player)
    public override void OnPlayerJoined(NetworkSandbox sandbox, NetworkPlayerId player)
    {
        // Random Spawn Position (to not make them overlap)
        Vector3 spawnPosition = new Vector3();
        spawnPosition.x = Random.Range(-5f, 5f);
        spawnPosition.z = Random.Range(-5f, 5f);
        //Debug.Log("OnPlayerConnected");
        //sandbox.NetworkInstantiate(PlayerPrefab.gameObject, spawnPosition, Quaternion.identity, player);

        NetworkPlayer networkPlayer = sandbox.GetPlayerById(player);

        NetworkPlayers.Add(networkPlayer);

        OnPlayerConnectedCallback.Invoke(networkPlayer);
    }

    //public override void OnPlayerDisconnected(NetworkSandbox sandbox, NetworkPlayer player, TransportDisconnectReason transportDisconnectReason)
    public override void OnPlayerLeft(NetworkSandbox sandbox, NetworkPlayerId player)
    {
        NetworkPlayer networkPlayer = sandbox.GetPlayerById(player);

        NetworkPlayers.Remove(networkPlayer);

        OnPlayerDisconnectedCallback.Invoke(networkPlayer);
    }
}