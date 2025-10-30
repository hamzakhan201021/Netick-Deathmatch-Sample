using Netick.Unity;
using UnityEngine;

public class FirePointRay : NetworkBehaviour
{
    public Transform firePoint;
    public float length = 10f;
    private LineRenderer lr;

    public override void NetworkStart()
    {
        lr = gameObject.AddComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.widthMultiplier = 0.05f;
        lr.positionCount = 2;
        lr.startColor = Color.red;
        lr.endColor = Color.red;
    }

    public override void NetworkFixedUpdate()
    {
        Vector3 start = firePoint.position;
        Vector3 end = start + firePoint.forward * length;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
    }

    //void NetworkUpdate()
    //{
    //    Vector3 start = firePoint.position;
    //    Vector3 end = start + firePoint.forward * length;
    //    lr.SetPosition(0, start);
    //    lr.SetPosition(1, end);
    //}
}
