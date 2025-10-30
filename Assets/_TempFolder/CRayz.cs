using Netick.Unity;
using UnityEngine;

public class CRayz : NetworkBehaviour
{
    public bool FIND = false;
    public bool Cache = true;
    public Vector3 ForceDir = Vector3.up;
    public float ForceMulti = 5;

    public Rigidbody[] _bodies;

    public override void NetworkStart()
    {
        if (IsServer)
        {
            if (Cache && FIND)
            {
                GetBodies();
            }
        }
    }

    //public override void NetworkUpdate()
    //{
    //    if (IsServer)
    //    {
    //        if (!Cache && FIND)
    //        {
    //            GetBodies();
    //        }

    //        ForceOnButton();
    //    }
    //}
    public override void NetworkFixedUpdate()
    {
        if (IsServer)
        {
            if (!Cache && FIND)
            {
                GetBodies();
            }

            ForceOnButton();
        }
    }

    private void ForceOnButton()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            for (int i = 0; i < _bodies.Length; i++)
            {
                _bodies[i].AddForce(ForceDir * ForceMulti, ForceMode.Impulse);
            }
        }
    }

    private void GetBodies()
    {
        Rigidbody[] foundRB = FindObjectsByType<Rigidbody>(FindObjectsSortMode.None);

        _bodies = foundRB;
    }
}