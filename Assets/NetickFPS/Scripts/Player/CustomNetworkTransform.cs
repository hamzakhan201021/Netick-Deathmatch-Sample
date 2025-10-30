using Netick;
using Netick.Unity;
using UnityEngine;

[ExecuteAfter(typeof(NetworkTransform))]
public class CustomNetworkTransform : NetworkBehaviour
{
    [Networked][Smooth(false)] public Vector3 Position { get; set; }
    [Networked][Smooth(false)] public Quaternion Rotation { get; set; }
    //public Transform RenderTransform;

    public override void NetcodeIntoGameEngine()
    {
        transform.position = Position;
        transform.rotation = Rotation;
    }

    public override void GameEngineIntoNetcode()
    {
        Position = transform.position;
        Rotation = transform.rotation;
    }

    //public override void NetworkRender()
    //{
    //    RenderTransform.transform.position = Position;
    //    RenderTransform.transform.rotation = Rotation;
    //}
}