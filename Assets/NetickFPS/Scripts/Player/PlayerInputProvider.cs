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

    private PlayerShootingController _pSC;

    public override void OnInput(NetworkSandbox sandbox)
    {
        if (_pSC == null) _pSC = GetComponent<PlayerShootingController>();

        PlayerInput input = sandbox.GetInput<PlayerInput>();

        //input.ShootInput = Input.GetKey(KeyCode.Mouse0);
        //input.ShotInterpData.RemoteInterpFrom = sandbox.RemoteInterpolation.From;
        //input.ShotInterpData.RemoteInterpTo = sandbox.RemoteInterpolation.To;
        //input.ShotInterpData.RemoteInterpAlpha = sandbox.RemoteInterpolation.Alpha;

        // TODO (try something else) set client tick.

        input.HitPosition = _pSC.HitPosition;
        input.HitRotation = _pSC.HitRotation;

        input.ClientTick = Sandbox.AuthoritativeTick;
        input.InterpolationTickTo = Sandbox.RemoteInterpolation.To;
        input.InterpolationTickFrom = Sandbox.RemoteInterpolation.From;
        input.InterpolationTickTo2 = sandbox.RemoteInterpolation.To;
        input.InterpolationTickFrom2 = sandbox.RemoteInterpolation.From;
        // input.InterpolationTickFrom = 


        //if (tg.isOn)
        //{
        //    input.Movement = new Vector2(tg2 ? speed : -speed, 0);
        //}

        sandbox.SetInput(input);
    }
}