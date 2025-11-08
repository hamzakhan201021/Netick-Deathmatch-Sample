using UnityEngine;

public class PlayablesAnimationIK : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnAnimatorIK(int layerIndex)
    {
        Debug.Log("On animator IK.. layer index is : " + layerIndex);
    }
}
