using UnityEngine;
using UnityEngine.UI;

public class MainLobbyUI : MonoBehaviour
{

    [Header("Buttons")]
    [SerializeField] private Button _createRoomButton;
    [SerializeField] private Button _joinRoomButton;
    [Header("Menus")]
    [SerializeField] private CreateRoomUI _createRoomUI;
    [SerializeField] private JoinRoomUI _joinRoomUI;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _createRoomButton.onClick.AddListener(_createRoomUI.OpenMenu);
        _joinRoomButton.onClick.AddListener(_joinRoomUI.OpenMenu);
    }

    private void OnDestroy()
    {
        _createRoomButton.onClick.RemoveListener(_createRoomUI.OpenMenu);
        _joinRoomButton.onClick.RemoveListener(_joinRoomUI.OpenMenu);
    }
}