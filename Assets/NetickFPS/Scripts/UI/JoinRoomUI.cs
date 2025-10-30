using UnityEngine;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using System.Collections.Generic;
using UnityEngine.UI;

public class JoinRoomUI : MonoBehaviour
{

    [Header("Content")]
    [SerializeField] private Transform _content;
    [SerializeField] private Transform _lobbyContainer;
    [Header("UI")]
    [SerializeField] private Transform _lobbyElement;
    [SerializeField] private Button _backButton;
    [Header("Lobbies")]
    [SerializeField, Range(2, 100), Tooltip("The interval before getting the lobbies again in seconds")] private float _getLobbiesInterval = 5;
    private float _timer = 0;

    private List<Lobby> _lobbies;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _backButton.onClick.AddListener(CloseMenu);
    }

    private void OnDestroy()
    {
        _backButton.onClick.RemoveListener(CloseMenu);
    }

    public void OpenMenu()
    {
        _content.gameObject.SetActive(true);

        ResetTimer();
        //GetLobbies();
    }

    private void CloseMenu()
    {
        _content.gameObject.SetActive(false);

        ResetTimer();
    }

    private async void GetLobbies()
    {
        try
        {
            Debug.Log("Get Lobbies");
            QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions()
            {
                Filters = new List<QueryFilter>
            {
                new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
            }
            };
            QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync(queryLobbiesOptions);

            //if (_lobbies != null) _lobbies.Clear();
            _lobbies?.Clear();

            _lobbies = queryResponse.Results;

            _timer = _getLobbiesInterval;

            UpdateLobbyUI();
        }
        catch (LobbyServiceException ex)
        {
            Debug.Log(ex);
        }
    }

    private void UpdateLobbyUI()
    {
        foreach (Transform child in _lobbyContainer)
        {
            if (child == _lobbyContainer) continue;

            Destroy(child);
        }

        foreach (Lobby lobby in _lobbies)
        {
            Transform lobbyTransform = Instantiate(_lobbyElement, _lobbyContainer) ;
            lobbyTransform.gameObject.SetActive(true);
            lobbyTransform.GetComponent<LobbyButtonElement>().SetLobby(lobby);
        }
    }

    private void Update()
    {
        bool isOpen = _content.gameObject.activeInHierarchy && _content.gameObject.activeSelf;

        if (isOpen)
        {
            // Get lobbies after the interval
            if (_timer == 0)
            {
                GetLobbies();

                _timer = -1;
            }
            else if (_timer > 0)
            {
                // Countdown.
                _timer -= Time.deltaTime;

                if (_timer <= 0) _timer = 0;
            }
        }
    }

    private void ResetTimer()
    {
        _timer = 0;
    }
}