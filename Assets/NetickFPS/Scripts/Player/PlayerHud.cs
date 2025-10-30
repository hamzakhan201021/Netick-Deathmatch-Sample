using Netick.Unity;
using UnityEngine;

public class PlayerHud : NetworkBehaviour
{
    [SerializeField] private GameObject _playerHud;

    public override void NetworkStart()
    {
        _playerHud.SetActive(IsInputSource);
    }
}
