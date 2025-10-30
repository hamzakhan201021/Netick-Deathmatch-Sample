using Netick;
using Netick.Unity;
using UnityEngine;

public class PlayerSpawning : NetworkBehaviour
{

    public NetworkObject PlayerPrefab;

    public override void NetworkStart()
    {
        if (IsServer)
        {
            CreateJoinedPlayers();

            GameplayManager.Instance.OnPlayerConnectedCallback.AddListener(CreateNewPlayer);
        }
    }

    private void CreateJoinedPlayers()
    {
        Debug.Log("CreateJoinedPlayers");
        //if (GameplayManager.Instance.NetworkPlayers.Count > 0)
        //{
        //Debug.Log("For loop");
        //for (int i = 0; i < GameplayManager.Instance.NetwShow potential fixesorkPlayers.Count; i++)
        //{
        //    CreateNewPlayer(GameplayManager.Instance.NetworkPlayers[i]);
        //}
        //}

        for (int i = 0; i < Sandbox.Players.Count; i++)
        {
            CreateNewPlayer(Sandbox.GetPlayerById(Sandbox.Players[i]));
        }
    }

    private void CreateNewPlayer(NetworkPlayer player)
    {
        Vector3 spawnPosition = new Vector3();
        spawnPosition.x = Random.Range(-5f, 5f);
        spawnPosition.z = Random.Range(-5f, 5f);

        //sandbox.NetworkInstantiate(PlayerPrefab.gameObject, spawnPosition, Quaternion.identity, player);
        Sandbox.NetworkInstantiate(PlayerPrefab.gameObject, spawnPosition, Quaternion.identity, player);
    }
}