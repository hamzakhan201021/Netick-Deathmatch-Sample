using UnityEngine;
using Netick.Unity;

public class PlayerInputProvider : NetworkEventsListener
{
    //Toggle tg;
    //Toggle tg2;
    //float speed = 1;

    //public override void OnStartup(NetworkSandbox sandbox)
    //{
    //    tg = GameObject.FindGameObjectWithTag("Finish").GetComponent<Toggle>();
    //    tg2 = GameObject.FindGameObjectWithTag("Finish2").GetComponent<Toggle>();
    //}

    public override void OnInput(NetworkSandbox sandbox)
    {
        PlayerInput input = sandbox.GetInput<PlayerInput>();

        //input.ShootInput = Input.GetKey(KeyCode.Mouse0);
        //input.ShotInterpData.RemoteInterpFrom = sandbox.RemoteInterpolation.From;
        //input.ShotInterpData.RemoteInterpTo = sandbox.RemoteInterpolation.To;
        //input.ShotInterpData.RemoteInterpAlpha = sandbox.RemoteInterpolation.Alpha;

        // TODO (try something else) set client tick.
        input.ClientTick = sandbox.AuthoritativeTick;

        //if (tg.isOn)
        //{
        //    input.Movement = new Vector2(tg2 ? speed : -speed, 0);
        //}

        sandbox.SetInput(input);
    }
}