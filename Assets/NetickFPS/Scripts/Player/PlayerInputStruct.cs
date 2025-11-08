using Netick;
using UnityEngine;

public struct PlayerInput : INetworkInput
{
    public Vector2 Movement;
    public Vector2 YawPitch;
    public Vector2 MouseInput;

    public bool CrouchInput;
    public bool Sprinting;
    public bool JumpInput;

    // Shooting stuff
    public bool ShootInput;
    public bool ReloadInput;

    public Vector3 HitPosition;
    public Quaternion HitRotation;
    public int ClientTick;
}