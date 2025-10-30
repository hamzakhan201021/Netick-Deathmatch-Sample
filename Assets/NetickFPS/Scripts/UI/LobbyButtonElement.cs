using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LobbyButtonElement : MonoBehaviour
{

    [Header("UI")]
    [SerializeField] private Button _joinButton;
    [SerializeField] private TMP_Text _lobbyName;

    private Lobby _lobby;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _joinButton.onClick.AddListener(OnJoinButtonClicked);
    }

    private void OnJoinButtonClicked()
    {
        if (_lobby != null)
        {
            NetworkM.Instance.JoinRoomID(_lobby.Id);
        }
    }

    public void SetLobby(Lobby lobby)
    {
        _lobby = lobby;
        _lobbyName.text = lobby.Name;
    }

    private void OnDestroy()
    {
        _joinButton.onClick.RemoveListener(OnJoinButtonClicked);
    }
}