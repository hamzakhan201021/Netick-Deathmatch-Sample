using UnityEngine;
using Netick.Unity;

public class SideToSideMovement : NetworkBehaviour
{
    [SerializeField] private float _speed = 1f;
    [SerializeField] private float _distance = 1f;

    [SerializeField] private bool OnlyInputSource = true;
    [SerializeField] private bool OnlyServer = false;

    public override void NetworkUpdate()
    {
        if (!IsInputSource && OnlyInputSource)
            return;

        if (!IsServer && OnlyServer) return;

        transform.position = new Vector3(
            Mathf.Sin(Time.time * _speed) * _distance,
            transform.position.y,
            transform.position.z
        );
    }
}