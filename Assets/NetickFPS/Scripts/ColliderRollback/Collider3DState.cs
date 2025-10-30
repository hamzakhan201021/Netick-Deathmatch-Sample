using UnityEngine;

public struct Collider3DState
{
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;
    public int Tick;

    public Collider3DState(Collider collider, int tick)
    {
        var trs = collider.transform;
        position = trs.position;
        rotation = trs.rotation;
        scale = trs.localScale;
        Tick = tick;
    }

    public Collider3DState Interpolate(Collider3DState stateB, float tickFraction)
    {
        return new Collider3DState
        {
            position = Vector3.Lerp(position, stateB.position, tickFraction),
            rotation = Quaternion.Slerp(rotation, stateB.rotation, tickFraction),
            scale = Vector3.Lerp(scale, stateB.scale, tickFraction)
        };
    }
}