using System.Collections.Generic;
using UnityEngine;

public class PlayableAnimatorValueGroupControl : MonoBehaviour
{

    public List<PlayablesAnimator> playablesAnimators;
    [Header("Values")]
    [SerializeField, Range(-1, 1)] private float MoveY = 0;
    [SerializeField, Range(-1, 1)] private float MoveX = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < playablesAnimators.Count; i++)
        {
            playablesAnimators[i].MoveY = MoveY;
            playablesAnimators[i].MoveX = MoveX;
        }
    }
}
