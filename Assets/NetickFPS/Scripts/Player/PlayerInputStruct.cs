using Netick;
using UnityEngine;

public struct PlayerInput : INetworkInput
{
    public Vector2 Movement;
    public Vector2 YawPitch;

    public bool Sprinting;
    public bool JumpInput;

    public bool ShootInput;

    // Shooting stuff

    public int ClientTick;

    public Vector3 Position;
    public Quaternion Rotation;
}