using Netick;
using UnityEngine;

public struct PlayerCharacterInput : INetworkInput
{
    public Vector3 Movement;
    public bool Jump;
    public bool RandomizeColor;
}