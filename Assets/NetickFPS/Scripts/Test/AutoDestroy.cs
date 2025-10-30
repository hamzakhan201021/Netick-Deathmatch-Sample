using Netick;
using Netick.Unity;
using UnityEngine;

public class AutoDestroy : NetworkBehaviour
{
    private float timeLeft;
    private bool active;

    public void Begin(float duration)
    {
        if (!Sandbox.IsServer) return;
        timeLeft = duration;
        active = true;
    }

    public override void NetworkFixedUpdate()
    {
        if (!active || !Sandbox.IsServer)
            return;

        timeLeft -= Sandbox.FixedDeltaTime;

        if (timeLeft <= 0f)
        {
            active = false;
            Sandbox.Destroy(Object);
        }
    }
}