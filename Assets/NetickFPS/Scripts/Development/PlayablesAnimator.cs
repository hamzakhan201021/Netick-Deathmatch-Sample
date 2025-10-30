using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

[RequireComponent(typeof(Animator))]
public class PlayablesAnimator : MonoBehaviour
{
    public AnimationClip IdleClip;
    public AnimationClip WalkForwardClip;
    public AnimationClip WalkBackwardClip;

    [Range(-1, 1)]
    public float weight;

    PlayableGraph playableGraph;

    AnimationMixerPlayable mixerPlayable;

    //AnimationMixerPlayable _moveMixerPlayable;

    void Start()
    {
        // Creates the graph, the mixer and binds them to the Animator.
        playableGraph = PlayableGraph.Create();

        var playableOutput = AnimationPlayableOutput.Create(playableGraph, "Animation", GetComponent<Animator>());

        mixerPlayable = AnimationMixerPlayable.Create(playableGraph, 3);
        playableOutput.SetSourcePlayable(mixerPlayable);

        // Creates AnimationClipPlayable and connects them to the mixer.
        var clipPlayable0 = AnimationClipPlayable.Create(playableGraph, IdleClip);
        var clipPlayable1 = AnimationClipPlayable.Create(playableGraph, WalkForwardClip);
        var clipPlayable2 = AnimationClipPlayable.Create(playableGraph, WalkBackwardClip);

        playableGraph.Connect(clipPlayable0, 0, mixerPlayable, 0);
        playableGraph.Connect(clipPlayable1, 0, mixerPlayable, 1);
        playableGraph.Connect(clipPlayable2, 0, mixerPlayable, 2);

        // Plays the Graph.
        //playableGraph.Play();
    }

    void Update()
    {
        playableGraph.Evaluate(Time.deltaTime);

        // Clamp weight.
        weight = Mathf.Clamp(weight, -1, 1);

        // Calculate weights
        float idleWeight = 1f - Mathf.Abs(weight);
        float forwardWeight = Mathf.Max(0f, weight);
        float backwardWeight = Mathf.Abs(Mathf.Max(0f, -weight));

        // calc and set weights.
        mixerPlayable.SetInputWeight(0, idleWeight);
        mixerPlayable.SetInputWeight(1, forwardWeight);
        mixerPlayable.SetInputWeight(2, backwardWeight);
    }

    void OnDisable()
    {
        // Destroys all Playables and Outputs created by the graph.
        playableGraph.Destroy();
    }

    //public RuntimeAnimatorController Controller; // your .controller with 2D blend tree

    //PlayableGraph graph;
    //AnimatorControllerPlayable controllerPlayable;

    //public float MoveX;
    //public float MoveZ;

    //void Start()
    //{
    //    var animator = GetComponent<Animator>();
    //    animator.runtimeAnimatorController = null; // stop Animator from auto-updating

    //    graph = PlayableGraph.Create();

    //    var output = AnimationPlayableOutput.Create(graph, "Animation", animator);

    //    controllerPlayable = AnimatorControllerPlayable.Create(graph, Controller);
    //    output.SetSourcePlayable(controllerPlayable);

    //    graph.Play();
    //    graph.Stop();
    //}

    //void FixedUpdate()
    //{
    //    controllerPlayable.SetFloat("MoveX", MoveX);
    //    controllerPlayable.SetFloat("MoveZ", MoveZ);

    //    //controllerPlayable.SetTime();
    //    //controllerPlayable.GetTime();

    //    // manual evaluation step
    //    graph.Evaluate(Time.fixedDeltaTime);
    //}

    //void OnDisable()
    //{
    //    graph.Destroy();
    //}
}