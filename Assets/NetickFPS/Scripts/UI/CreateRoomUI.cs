using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CreateRoomUI : MonoBehaviour
{

    [Header("Content")]
    [SerializeField] private GameObject _content;
    [Header("UI")]
    [SerializeField] private TMP_InputField _lobbyNameInputField;
    [SerializeField] private Button _createLobbyButton;
    [SerializeField] private Button _backButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _lobbyNameInputField.onValueChanged.AddListener(OnLobbyNameChanged);
        _createLobbyButton.onClick.AddListener(OnCreateLobbyButtonClicked);
        _backButton.onClick.AddListener(CloseMenu);

        OnLobbyNameChanged(_lobbyNameInputField.text);
    }

    private void OnDestroy()
    {
        _lobbyNameInputField.onValueChanged.RemoveListener(OnLobbyNameChanged);
        _createLobbyButton.onClick.RemoveListener(OnCreateLobbyButtonClicked);
        _backButton.onClick.RemoveListener(CloseMenu);
    }

    private void OnCreateLobbyButtonClicked()
    {
        if (IsValidName(_lobbyNameInputField.text))
        {
            NetworkM.Instance.CreateRoom(_lobbyNameInputField.text);
            //NetworkM.Instance.StartSinglePlayer();
        }
    }

    private void OnLobbyNameChanged(string value)
    {
        if (IsValidName(value))
        {
            ShowButton(true);
        }
        else
        {
            ShowButton(false);
        }
    }

    private bool IsValidName(string input)
    {
        return !string.IsNullOrWhiteSpace(input) && input.Trim().Length > 0;
    }

    private void ShowButton(bool show)
    {
        _createLobbyButton.gameObject.SetActive(show);
    }

    public void OpenMenu()
    {
        _content.SetActive(true);
    }

    private void CloseMenu()
    {
        _content.SetActive(false);
    }
}