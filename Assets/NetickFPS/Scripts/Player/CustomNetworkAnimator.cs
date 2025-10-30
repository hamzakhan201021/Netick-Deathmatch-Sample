using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations;
using Netick;
using Netick.Unity;

public enum AnimationState
{
    Idle,
    WalkForward,
    WalkBackward
}

public class CustomNetworkAnimator : NetworkBehaviour
{
    public AnimationClip IdleClip;
    public AnimationClip WalkForwardClip;
    public AnimationClip WalkBackwardClip;

    [Networked, Smooth(false)]
    public AnimationState State { get; set; }

    [Networked, Smooth(false)]
    public double StateTime { get; set; }

    PlayableGraph graph;
    AnimationMixerPlayable mixer;
    AnimationClipPlayable idle, walkF, walkB;

    void Start()
    {
        var animator = GetComponent<Animator>();
        animator.runtimeAnimatorController = null;

        graph = PlayableGraph.Create("NetworkPlayableAnimator");
        var output = AnimationPlayableOutput.Create(graph, "Animation", animator);

        mixer = AnimationMixerPlayable.Create(graph, 3);
        idle = AnimationClipPlayable.Create(graph, IdleClip);
        walkF = AnimationClipPlayable.Create(graph, WalkForwardClip);
        walkB = AnimationClipPlayable.Create(graph, WalkBackwardClip);

        graph.Connect(idle, 0, mixer, 0);
        graph.Connect(walkF, 0, mixer, 1);
        graph.Connect(walkB, 0, mixer, 2);

        output.SetSourcePlayable(mixer);

        // freeze all playables — manual sampling
        idle.SetSpeed(0);
        walkF.SetSpeed(0);
        walkB.SetSpeed(0);
        graph.Play();
    }

    public override void NetworkFixedUpdate()
    {
        if (FetchInput(out PlayerInput input))
        {
            if (input.Movement.y > 0.1f)
                State = AnimationState.WalkForward;
            else if (input.Movement.y < -0.1f)
                State = AnimationState.WalkBackward;
            else
                State = AnimationState.Idle;
        }

        if (!Object.IsProxy)
            StateTime += Sandbox.FixedDeltaTime;
    }

    public override void NetworkRender()
    {
        //var interpolator = FindInterpolator(nameof(State));
        //bool didGetData = interpolator.GetInterpolationData<AnimationState>(InterpolationSource.Auto, out var from, out var to, out float alpha);

        //AnimationState interpolatedValue = default;

        //if (didGetData)
        //{
        //    var interpState = InterpolateState(State); // use Netick's interpolator API if you want to smooth transitions


        //    interpolatedValue = AnimationState.Interpolate(from, to, alpha);
        //}
        //else
        //{
        //    interpolatedValue = State;
        //}

        double interpTime = Object.IsProxy
                ? Sandbox.RemoteInterpolation.Time
                : Sandbox.LocalInterpolation.Time;

        var interpState = State;

        Animate(interpState, interpTime);
    }

    void Animate(AnimationState state, double time)
    {
        // Reset weights
        mixer.SetInputWeight(0, 0);
        mixer.SetInputWeight(1, 0);
        mixer.SetInputWeight(2, 0);

        // Pick active playable
        AnimationClipPlayable activePlayable = state switch
        {
            AnimationState.WalkForward => walkF,
            AnimationState.WalkBackward => walkB,
            _ => idle,
        };

        // Set correct clip time based on tick interpolation
        double t = StateTime + time;
        double dur = activePlayable.GetAnimationClip().length;
        activePlayable.SetTime(t % dur);

        // Activate correct weight
        int index = (state == AnimationState.Idle) ? 0 :
                    (state == AnimationState.WalkForward) ? 1 : 2;
        mixer.SetInputWeight(index, 1f);

        // Evaluate pose
        graph.Evaluate(0);
    }

    void OnDestroy()
    {
        if (graph.IsValid()) graph.Destroy();
    }
}