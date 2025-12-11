using HalalStudio.NetickLagCompensation;
using Netick;
using Netick.Unity;
using UnityEngine;

namespace LagCompTest
{
    public class LagCompPlayer : NetworkBehaviour
    {

        [SerializeField] private Transform _origin;
        [SerializeField] private float _maxDistance;

        private bool DidHit;
        private Vector3 HitPosition;
        private Quaternion HitRotation;
        private float InterpAlpha;
        private float InterpTo;
        // [SerializeField] private ;

        public struct LCPlayerInput : INetworkInput
        {
            public bool Shoot;
            public bool Right;
            public bool Left;
            public int RemoteInterpTo;
            public float RemoteInterpAlpha;
        }

        public override void NetworkUpdate()
        {
            if (!IsInputSource || !Sandbox.InputEnabled) return;

            // {
            LCPlayerInput input = Sandbox.GetInput<LCPlayerInput>();
            input.RemoteInterpTo = Sandbox.RemoteInterpolation.To;
            input.RemoteInterpAlpha = Sandbox.RemoteInterpolation.Alpha;
            input.Shoot |= Input.GetKeyDown(KeyCode.Space);
            input.Left = Input.GetKey(KeyCode.LeftArrow);
            input.Right = Input.GetKey(KeyCode.RightArrow);
            Sandbox.SetInput(input);

            if (input.Shoot)
            {
                /// check using col cast
                if (ColliderCastSystem.ColliderCastTransform(_origin.position, _origin.forward, _maxDistance,
                out ColliderCastHit hit, out HitColliderCollection collection, out int index))
                {
                    Debug.Log("I should get my HIT");

                    DidHit = true;

                    InterpTo = input.RemoteInterpTo;
                    InterpAlpha = input.RemoteInterpAlpha;

                    HitPosition = collection.GetHitColliderAtIndex(index).transform.position;
                    HitRotation = collection.GetHitColliderAtIndex(index).transform.rotation;
                }
            }
            // }
        }

        public override void NetworkFixedUpdate()
        {
            if (FetchInput(out LCPlayerInput input))
            {
                // Movement
                if (input.Left)
                {
                    transform.position += new Vector3(-0.1f, 0, 0);
                }
                else if (input.Right)
                {
                    transform.position += new Vector3(0.1f, 0, 0);
                }

                if (input.Shoot && !Sandbox.IsResimulating)
                {
                    // Do lag comp or test etc

                    if (IsServer)
                    {
                        LagCompensationManager s = Sandbox.GetComponent<LagCompensationManager>();

                        TickInterpolation interpData = new TickInterpolation(input.RemoteInterpTo, input.RemoteInterpAlpha);

                        Ray ray = new Ray(_origin.position, _origin.forward);

                        if (s.RaycastLC(ray, InputSource, -1, interpData, out LCHitInfo hitInfo, _maxDistance, null))
                        {
                            // We probably got a hit so we can now try to do somethin.
                            Debug.Log("Found hit");
                        }
                    }
                    else
                    {
                        if (DidHit)
                        {
                            DidHit = false;

                            Debug.Log("Sending rpc to server");

                            Debug.Log("Interp values");
                            Debug.Log(input.RemoteInterpTo);
                            Debug.Log(InterpTo);
                            Debug.Log(input.RemoteInterpAlpha);
                            Debug.Log(InterpAlpha);

                            /// Send debug data
                            LagCompensationManager LCManager = Sandbox.GetComponent<LagCompensationManager>();
                            LCManager.SendClientHitObjectDataRpc(HitPosition, HitRotation, false, input.RemoteInterpTo);
                        }
                    }
                    // else
                    // {
                    //     LagCompensationManager LCManager = Sandbox.GetComponent<LagCompensationManager>();
                    //     LCManager.SendClientHitObjectDataRpc();
                    // }
                }
            }
        }
    }
}