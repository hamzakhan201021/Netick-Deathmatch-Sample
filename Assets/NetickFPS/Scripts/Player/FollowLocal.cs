using UnityEngine;

public class FollowLocal : MonoBehaviour
{

    public Transform Target;


    // Update is called once per frame
    void Update()
    {
        transform.position = Target.position;
        transform.rotation = Target.rotation;
    }
}
