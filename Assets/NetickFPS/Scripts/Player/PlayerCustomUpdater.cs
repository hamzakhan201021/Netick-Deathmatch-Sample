using System;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Animations.Rigging;
using UnityEngine.Playables;

public class PlayerCustomUpdater : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] RigBuilder rigBuilder;
    Rig builder;
    [SerializeField] bool autoTickFixed;

    PlayableGraph animGraph;
    bool initialized;

    private float _lastUpdateTime;
    private float _updateDelta;

    void Awake()
    {
        if (!animator) animator = GetComponent<Animator>();
        if (!rigBuilder) rigBuilder = GetComponent<RigBuilder>();

        //rigBuilder.Build();
        //rigBuilder.graph.SetTimeUpdateMode(DirectorUpdateMode.Manual);

        //animator.updateMode = AnimatorUpdateMode.Normal;
        //animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
        //animator.enabled = false; // prevent Unity from auto-updating
        //animator.playableGraph.SetTimeUpdateMode(DirectorUpdateMode.Manual);

        //animGraph = animator.playableGraph;
        //if (animGraph.IsValid())
        //initialized = true;
    }

    public void Tick(float deltaTime)
    {
        //if (!initialized) return;

        var delta = Time.time - _lastUpdateTime;
        animator.Update(delta);

        _updateDelta += delta;
        _lastUpdateTime = Time.time;

        //animator.Update(deltaTime);
        //animator.playableGraph.Evaluate(deltaTime);
        //animGraph.Evaluate(deltaTime);
        //rigBuilder.SyncLayers();

        //rigBuilder.Evaluate(deltaTime);b
    }

    private void Update()
    {
        animator.Update(-_updateDelta);

        _updateDelta = 0;
        _lastUpdateTime = Time.time;
    }

    private void LateUpdate()
    {
        //rigBuilder.SyncLayers();
        //rigBuilder.graph.Evaluate(Time.deltaTime);
    }

    void FixedUpdate()
    {
        if (autoTickFixed)
            Tick(Time.fixedDeltaTime);
    }
}