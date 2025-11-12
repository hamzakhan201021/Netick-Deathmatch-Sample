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
    public int InterpolationTickTo;
    public int InterpolationTickFrom;
    public float InterpolationAlpha;
    public int InterpolationTickTo2;
    public int InterpolationTickFrom2;

    public Vector3 PositionHit;
    public Quaternion Rotationhit;
}