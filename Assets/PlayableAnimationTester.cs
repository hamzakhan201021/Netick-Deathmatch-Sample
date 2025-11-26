using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations;
using UnityEngine.Animations.Rigging;
using System;
using static PlayerTickAnimController;

public class PlayableAnimationTester : MonoBehaviour
{
    [Header("Graph Management")]
    [SerializeField] private RigBuilder rigBuilder;
    [SerializeField] private Animator targetAnimator;
    [SerializeField] private bool invokeSyncLayers;

    [Header("Locomotion Clips")]
    public BlendClip[] WalkClips;
    public BlendClip[] RunClips;
    public BlendClip[] CrouchClips;
    public AnimationClip AirborneClip;

    [Header("Upper Body")]
    public AnimationClip UpperBodyClip;
    public AvatarMask UpperBodyMask;

    [Header("Input Keys")]
    public KeyCode Forward = KeyCode.W;
    public KeyCode Back = KeyCode.S;
    public KeyCode Left = KeyCode.A;
    public KeyCode Right = KeyCode.D;
    public KeyCode RunKey = KeyCode.LeftShift;
    public KeyCode CrouchKey = KeyCode.LeftControl;
    public KeyCode GroundedToggle = KeyCode.Space;

    [Header("Manual Time Controls")]
    public bool UseManualInterpolationTime = false;
    public bool UseManualTimeOffset = false;

    public float ManualInterpolationTime = 0f;
    public float ManualTimeOffset = 0f;

    [Header("Internal Values (debug)")]
    public float MoveX;
    public float MoveY;
    public float StateValue;          // 0 = crouch, 0.5 = walk, 1 = run, 2 = sprint or extended state
    public float GroundedValue = 1f;  // 1 grounded, 0 airborne

    private float interpolationTime;
    private float finalTime;

    private PlayableGraph graph;
    private AnimationMixerPlayable locomotionMixer;
    private AnimationMixerPlayable groundedMixer;
    private AnimationMixerPlayable walkMixer;
    private AnimationMixerPlayable runMixer;
    private AnimationMixerPlayable crouchMixer;

    private AnimationClipPlayable[] walkPlayables;
    private AnimationClipPlayable[] runPlayables;
    private AnimationClipPlayable[] crouchPlayables;
    private AnimationClipPlayable airbornePlayable;
    private AnimationClipPlayable upperBodyPlayable;

    private PolarGradientBandInterpolator walkInterp;
    private PolarGradientBandInterpolator runInterp;
    private PolarGradientBandInterpolator crouchInterp;

    private LinearBandInterpolator locomotionInterp = new(new float[] { 0f, 0.5f, 1f, 2f });
    private LinearBandInterpolator groundedInterp = new(new float[] { 0f, 1f });

    void Start()
    {
        rigBuilder.Build();
        graph = rigBuilder.graph;
        graph.SetTimeUpdateMode(DirectorUpdateMode.Manual);

        CreateGraph();
        graph.Play();
    }

    private void CreateGraph()
    {
        (walkMixer, walkPlayables, walkInterp) = CreateTree(WalkClips);
        (runMixer, runPlayables, runInterp) = CreateTree(RunClips);
        (crouchMixer, crouchPlayables, crouchInterp) = CreateTree(CrouchClips);

        locomotionMixer = AnimationMixerPlayable.Create(graph, 3);
        graph.Connect(crouchMixer, 0, locomotionMixer, 0);
        graph.Connect(walkMixer, 0, locomotionMixer, 1);
        graph.Connect(runMixer, 0, locomotionMixer, 2);

        airbornePlayable = AnimationClipPlayable.Create(graph, AirborneClip);

        groundedMixer = AnimationMixerPlayable.Create(graph, 2);
        graph.Connect(locomotionMixer, 0, groundedMixer, 0);
        graph.Connect(airbornePlayable, 0, groundedMixer, 1);

        var layerMixer = AnimationLayerMixerPlayable.Create(graph, 2);
        graph.Connect(groundedMixer, 0, layerMixer, 0);

        upperBodyPlayable = AnimationClipPlayable.Create(graph, UpperBodyClip);
        graph.Connect(upperBodyPlayable, 0, layerMixer, 1);
        layerMixer.SetLayerMaskFromAvatarMask(1, UpperBodyMask);
        layerMixer.SetInputWeight(0, 1f);
        layerMixer.SetInputWeight(1, 1f);

        var output = AnimationPlayableOutput.Create(graph, "Anim", targetAnimator);
        output.SetSourcePlayable(layerMixer);
    }

    private (AnimationMixerPlayable, AnimationClipPlayable[], PolarGradientBandInterpolator)
        CreateTree(BlendClip[] clips)
    {
        var mixer = AnimationMixerPlayable.Create(graph, clips.Length);
        var playables = new AnimationClipPlayable[clips.Length];

        float[][] sample = new float[clips.Length][];

        for (int i = 0; i < clips.Length; i++)
        {
            playables[i] = AnimationClipPlayable.Create(graph, clips[i].Clip);
            playables[i].SetApplyFootIK(true);
            playables[i].SetApplyPlayableIK(false);

            graph.Connect(playables[i], 0, mixer, i);
            mixer.SetInputWeight(i, i == 0 ? 1 : 0);

            sample[i] = new float[] { clips[i].Position.x, clips[i].Position.y };
        }

        return (mixer, playables, new PolarGradientBandInterpolator(sample));
    }

    void Update()
    {
        HandleInput();
        ComputeInterpolationTime();
        ApplyAnimation();
    }

    private void HandleInput()
    {
        MoveX = 0;
        MoveY = 0;

        if (Input.GetKey(Forward)) MoveY = 1;
        if (Input.GetKey(Back)) MoveY = -1;
        if (Input.GetKey(Left)) MoveX = -1;
        if (Input.GetKey(Right)) MoveX = 1;

        if (Input.GetKey(RunKey)) StateValue = 1f;
        else if (Input.GetKey(CrouchKey)) StateValue = 0f;
        else StateValue = 0.5f;

        if (Input.GetKeyDown(GroundedToggle))
            GroundedValue = GroundedValue > 0 ? 0 : 1;
    }

    private void ComputeInterpolationTime()
    {
        if (UseManualInterpolationTime)
            interpolationTime = ManualInterpolationTime;
        else
            interpolationTime += Time.deltaTime; // free-running

        if (UseManualTimeOffset)
            finalTime = interpolationTime + ManualTimeOffset;
        else
            finalTime = interpolationTime;
    }

    private void ApplyAnimation()
    {
        float[] locWeights = locomotionInterp.Interpolate(StateValue);
        locomotionMixer.SetInputWeight(0, locWeights[0]);
        locomotionMixer.SetInputWeight(1, locWeights[1]);
        locomotionMixer.SetInputWeight(2, locWeights[2]);

        float groundedW = groundedInterp.Interpolate(GroundedValue)[0];
        groundedMixer.SetInputWeight(0, groundedW);
        groundedMixer.SetInputWeight(1, 1f - groundedW);

        ApplyTree(crouchMixer, crouchPlayables, crouchInterp);
        ApplyTree(walkMixer, walkPlayables, walkInterp);
        ApplyTree(runMixer, runPlayables, runInterp);

        airbornePlayable.SetTime(finalTime * AirborneClip.length);
        upperBodyPlayable.SetTime(finalTime * UpperBodyClip.length);

        if (invokeSyncLayers) rigBuilder.SyncLayers();
        graph.Evaluate();
    }

    private void ApplyTree(AnimationMixerPlayable mixer, AnimationClipPlayable[] clips,
        PolarGradientBandInterpolator interp)
    {
        float[] w = interp.Interpolate(new float[] { MoveX, MoveY }, true);

        for (int i = 0; i < clips.Length; i++)
        {
            clips[i].SetTime(finalTime * clips[i].GetAnimationClip().length);
            mixer.SetInputWeight(i, w[i]);
        }
    }

    void OnDestroy()
    {
        if (graph.IsValid()) graph.Destroy();
    }
}
