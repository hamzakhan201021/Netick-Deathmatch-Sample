using Netick;
using Netick.Unity;
using UnityEngine;

public class DoubleSmoothedNet : NetworkBehaviour
{
    public struct InputNew : INetworkInput
    {
        public NetworkBool AddValue;
        public float Duration;
    }

    public bool TestingEnabled = false;

    [Networked, Smooth] public double ThisDoubleSmoothed { get; set; }
    [Networked, Smooth] public float ThisFloatSmoothed { get; set; }

    // public double SetValue = 0;
    public override void NetworkUpdate()
    {
        if (!TestingEnabled) return;

        InputNew input = Sandbox.GetInput<InputNew>();
        // Debug.Log(Input.GetKey(KeyCode.Space));

        input.AddValue = Input.GetKey(KeyCode.Space);
        Sandbox.SetInput(input);
    }
    public override void NetworkFixedUpdate()
    {
        if (!TestingEnabled) return;

        if (FetchInput(out InputNew input))
        {
            Debug.Log($"Network fixed update {input.AddValue}");

            if (input.AddValue)
            {
                ThisDoubleSmoothed += 1;
                ThisFloatSmoothed += 1;
            }
        }
    }

    public override void NetworkRender()
    {
        if (!TestingEnabled) return;

        Debug.Log($"Value of double {ThisDoubleSmoothed}");
        Debug.Log($"Value of float {ThisFloatSmoothed}");
    }
}