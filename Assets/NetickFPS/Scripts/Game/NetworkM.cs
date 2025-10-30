using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using System.Threading.Tasks;

using Netick.Unity;
using Netick.Transport;
using Network = Netick.Unity.Network;

public class NetworkM : NetworkEventsListener
{

    public GameObject SandboxPrefab;
    public NetworkTransportProvider UnityTransport;

    private const string KEY_RELAY_JOIN_CODE = "RelayJoinCode";

    private Lobby _joinedLobby;

    public static NetworkM Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private async void Start()
    {
        DontDestroyOnLoad(gameObject);

        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed in " + AuthenticationService.Instance.PlayerId);
        };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    public void StartSinglePlayer()
    {
        var sandbox = Network.StartAsSinglePlayer(SandboxPrefab);

        sandbox.SwitchScene("PlayerScene");
    }

    public async void CreateRoom(string lobbyName, bool isPrivate = false)
    {
        try
        {
            NetickConfig config = Resources.Load<NetickConfig>("NetickConfig");

            int maxPlayers = config.MaxPlayers;

            CreateLobby(lobbyName, maxPlayers, isPrivate);

            Allocation allocation = await AllocateRelay(maxPlayers);

            NetickUnityTransport.SetAllocation(allocation);

            string joinCode = await GetRelayJoinCode(allocation);

            await LobbyService.Instance.UpdateLobbyAsync(_joinedLobby.Id, new UpdateLobbyOptions()
            {
                Data = new System.Collections.Generic.Dictionary<string, DataObject>
                {
                    { KEY_RELAY_JOIN_CODE, new DataObject(DataObject.VisibilityOptions.Member, joinCode)}
                }
            });

            var sandbox = Network.StartAsHost(UnityTransport, allocation.RelayServer.Port, SandboxPrefab);

            sandbox.SwitchScene("PlayerScene");

            Debug.Log("Join Code " + joinCode);
        }
        catch (RelayServiceException ex)
        {
            Debug.Log(ex);
        }
    }

    public async void JoinRoomCode(string joinCode)
    {
        try
        {
            Debug.Log("Joining Relay with code " + joinCode);

            _joinedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(joinCode);

            string joinRelayCode = _joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;

            JoinAllocation ja = await RelayService.Instance.JoinAllocationAsync(joinRelayCode);

            NetickUnityTransport.SetJoinAllocation(ja);

            var sandBox = Network.StartAsClient(UnityTransport, ja.RelayServer.Port, SandboxPrefab);
            sandBox.Connect(ja.RelayServer.Port, ja.RelayServer.IpV4);
        }
        catch (RelayServiceException ex)
        {
            Debug.Log(ex);
        }
    }

    public async void JoinRoomID(string id)
    {
        try
        {
            Debug.Log("Joining Relay with ID " + id);
            
            _joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(id);

            string joinRelayCode = _joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;

            JoinAllocation ja = await RelayService.Instance.JoinAllocationAsync(joinRelayCode);

            NetickUnityTransport.SetJoinAllocation(ja);

            var sandBox = Network.StartAsClient(UnityTransport, ja.RelayServer.Port, SandboxPrefab);
            sandBox.Connect(ja.RelayServer.Port, ja.RelayServer.IpV4);
        }
        catch (RelayServiceException ex)
        {
            Debug.Log(ex);
        }
    }

    private async void CreateLobby(string lobbyName, int maxPlayers, bool isPrivate)
    {
        try
        {
            CreateLobbyOptions options = new CreateLobbyOptions()
            {
                IsPrivate = isPrivate,
            };

            _joinedLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);
        }
        catch (LobbyServiceException ex)
        {
            Debug.Log(ex);
        }
    }

    private async Task QuickJoinLobby()
    {
        try
        {
            await LobbyService.Instance.QuickJoinLobbyAsync();
        }
        catch (LobbyServiceException ex)
        {
            Debug.Log(ex);
        }
    }

    // Unity Gaming Services
    private async Task<Allocation> AllocateRelay(int maxPlayers)
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxPlayers - 1);

            return allocation;
        }
        catch (RelayServiceException ex)
        {
            Debug.Log(ex);

            return default;
        }
    }

    private async Task<string> GetRelayJoinCode(Allocation allocation)
    {
        try
        {
            string relayJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            return relayJoinCode;
        }
        catch (RelayServiceException ex)
        {
            Debug.Log(ex);

            return default;
        }
    }
}