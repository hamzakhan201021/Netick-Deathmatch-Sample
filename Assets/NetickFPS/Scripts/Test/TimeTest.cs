using Netick.Unity;
using Netick;
using UnityEngine;
using static DoubleSmoothedNet;

public class TimeTest : NetworkBehaviour
{
    [Networked(1, Relevancy.Everyone, 0.00001f), Smooth] public float TimeOffset { get; set; }
    public bool DebugToConsole = true;

    public float DurationOfAnimation = 0;

    private float AnimationTime;

    public override void NetworkStart()
    {
        if (!IsProxy)
        {
            // AnimationTime = Sandbox.LocalInterpolation.Time;
        }
    }

    public override void NetworkUpdate()
    {
        InputNew input = Sandbox.GetInput<InputNew>();
        // Debug.Log(Input.GetKey(KeyCode.Space));

        input.Duration = DurationOfAnimation;
        Sandbox.SetInput(input);
    }

    public override void NetworkFixedUpdate()
    {
        // We are giving input or are the owner(server)
        // then we can change the networked variable
        if (!IsProxy && FetchInput(out InputNew input))
        {
            // AnimationTime += Sandbox.FixedDeltaTime;
            // Debug.Log(Sandbox.LocalInterpolation.Time);
            float deltaBlended = Sandbox.FixedDeltaTime / (float)input.Duration;
            TimeOffset += deltaBlended - Sandbox.FixedDeltaTime;
        }
    }

    public override void NetworkRender()
    {
        if (DebugToConsole)
        {
            Debug.Log($"Remote interpolation time {Sandbox.RemoteInterpolation.Time}");
            Debug.Log($"Time Offset {TimeOffset}");

            AnimationTime = TimeOffset + Sandbox.RemoteInterpolation.Time;
            Debug.Log($"Animation Time {AnimationTime}");
            Debug.Log($"Difference {Mathf.Abs(Sandbox.RemoteInterpolation.Time - AnimationTime)}");
        }
    }
}
