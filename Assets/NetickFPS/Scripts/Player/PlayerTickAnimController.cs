using UnityEngine;
using Netick;
using Netick.Unity;
using UnityEngine.Animations.Rigging;
using UnityEngine.Animations;
using UnityEngine.Playables;
using System.Collections.Generic;

public class PlayerTickAnimController : NetworkBehaviour
{
    //    [SerializeField] private Animator _animator;
    //    [SerializeField] private RigBuilder _rigBuilder;

    //    [Networked] public float AnimTime { get; set; }
    //    [Networked] public float Forward { get; set; } = 1;

    //    public override void NetworkAwake()
    //    {

    //    }

    //    public override void NetworkStart()
    //    {
    //        _rigBuilder.Build();
    //        _rigBuilder.graph.SetTimeUpdateMode(DirectorUpdateMode.Manual);
    //    }

    //    public override void NetworkFixedUpdate()
    //    {
    //        _animator.Update(Sandbox.FixedDeltaTime);
    //        _animator.playableGraph.Evaluate(Sandbox.FixedDeltaTime);


    //        AnimTime += Sandbox.FixedDeltaTime;
    //        _animator.SetFloat("MOTION", AnimTime);
    //    }

    //    private void LateUpdate()
    //    {
    //        _rigBuilder.Evaluate(Sandbox.FixedDeltaTime);
    //        _rigBuilder.graph.Evaluate(Sandbox.FixedDeltaTime);
    //    }

    //    public override void NetcodeIntoGameEngine()
    //    {
    //        //_animator.playbackTime = AnimTime;
    //        _animator.SetFloat("MOTION", AnimTime);
    //        _animator.SetFloat("MoveZ", Forward);
    //    }

    //    public override void GameEngineIntoNetcode()
    //    {
    //        //AnimTime = _animator.playbackTime;
    //        AnimTime = _animator.GetFloat("MOTION");
    //        Forward = _animator.GetFloat("MoveZ");
    //    }
    #region ADI
    // [SerializeField] private Animator _animator;
    // // [SerializeField] private RigBuilder _rigBuilder;

    // [Networked] public float AnimTime { get; set; }
    // [Networked] public float Forward { get; set; } = 1;

    // public bool EnableComponent = true;
    // public bool SetForward = false;
    // public bool UpdateAnimator = false;

    // //private float _lastUpdateTime;
    // //private float _updateDelta;

    // public override void NetworkFixedUpdate()
    // {
    //     if (!EnableComponent) return;

    //     // Update animator forward
    //     var delta = Sandbox.FixedDeltaTime;

    //     if (UpdateAnimator) _animator.Update(delta);

    //     //_rigBuilder.Evaluate(delta);


    //     //_updateDelta += delta;
    //     //_lastUpdateTime = Sandbox.NetworkTime;

    //     AnimTime += Sandbox.FixedDeltaTime;


    //     // Set the accurate time of the animator playback but in realtime idk how
    //     _animator.SetFloat("MOTION", AnimTime);
    //     if (SetForward) _animator.SetFloat("MoveZ", Forward);
    // }

    // //public override void NetworkUpdate()
    // //{
    // //    _animator.Update(-_updateDelta);

    // //    _updateDelta = 0;
    // //    _lastUpdateTime = Time.time;
    // //}

    // public override void NetcodeIntoGameEngine()
    // {
    //     if (!EnableComponent) return;

    //     _animator.SetFloat("MOTION", AnimTime);

    //     if (SetForward) _animator.SetFloat("MoveZ", Forward);
    // }

    // public override void GameEngineIntoNetcode()
    // {
    //     if (!EnableComponent) return;

    //     AnimTime = _animator.GetFloat("MOTION");
    //     if (SetForward) Forward = _animator.GetFloat("MoveZ");
    // }
    #endregion

    #region Non Layered (Only blend tree)
    //     [System.Serializable]
    //     public class BlendClip
    //     {
    //         public AnimationClip Clip;
    //         public Vector2 Position;
    //     }

    //     public List<BlendClip> BlendClips = new();

    //     [Range(-1f, 1f)] public float MoveX;
    //     [Range(-1f, 1f)] public float MoveY;
    //     [Range(0.0f, 5f)] public float Speed = 1f;
    //     [Tooltip("Keeping this on at all times is recommended, disabling this might result in odd animation")]
    //     [SerializeField] private bool _normalizeWeights = true;
    //     [SerializeField] private bool _enableAnimFootIK = false;
    //     [SerializeField] private bool _enableIKPass = false;

    //     PlayableGraph graph;
    //     AnimationMixerPlayable mixer;
    //     AnimationClipPlayable[] clipPlayables;
    //     AnimationClip[] clips;
    //     PolarGradientBandInterpolator interpolator;
    //     // double normalizedTime;

    //     public override void NetworkStart()
    //     {
    //         CreateGraph();
    //     }

    //     private void CreateGraph()
    //     {
    //         if (BlendClips == null || BlendClips.Count == 0)
    //         {
    //             Debug.LogError("Please assign some BlendClips before playing!");
    //             return;
    //         }

    //         int count = BlendClips.Count;
    //         clips = new AnimationClip[count];
    //         float[][] samplePoints = new float[count][];

    //         for (int i = 0; i < count; i++)
    //         {
    //             clips[i] = BlendClips[i].Clip;
    //             samplePoints[i] = new float[] { BlendClips[i].Position.x, BlendClips[i].Position.y };
    //         }

    //         interpolator = new PolarGradientBandInterpolator(samplePoints);

    //         var animator = GetComponent<Animator>();

    //         graph = PlayableGraph.Create();
    //         graph.SetTimeUpdateMode(DirectorUpdateMode.Manual);

    //         var output = AnimationPlayableOutput.Create(graph, "Animation", animator);

    //         mixer = AnimationMixerPlayable.Create(graph, count);
    //         output.SetSourcePlayable(mixer);

    //         clipPlayables = new AnimationClipPlayable[count];

    //         for (int i = 0; i < count; i++)
    //         {
    //             var c = clips[i] != null ? clips[i] : new AnimationClip();
    //             clipPlayables[i] = AnimationClipPlayable.Create(graph, c);
    //             // clipPlayables[i].SetApplyFootIK(_enableAnimFootIK);
    //             clipPlayables[i].SetSpeed(1.0);
    //             clipPlayables[i].SetTime(0.0);
    //             graph.Connect(clipPlayables[i], 0, mixer, i);
    //             mixer.SetInputWeight(i, i == 0 ? 1f : 0f);
    //         }

    //         graph.Play();
    //         // normalizedTime = 0.0;
    //     }


    //     public override void NetworkFixedUpdate()
    //     {
    //         if (!Object.IsProxy)
    //         {
    //             SetAnimation(Sandbox.LocalInterpolation.Time);
    //         }
    //     }

    //     public override void NetworkRender()
    //     {
    //         SetAnimation(Object.IsProxy ? Sandbox.RemoteInterpolation.Time : Sandbox.LocalInterpolation.Time);
    //     }

    //     private void SetAnimation(double aTime)
    //     {
    //         // TODO use time and update animation thingy
    //         if (!graph.IsValid()) return;

    //         // double delta = Time.deltaTime;
    //         // normalizedTime += aTime;
    //         // normalizedTime %= 1.0;

    //         // timeTrue += Time.deltaTime;
    //         // timeTrue %= 1.0;

    //         graph.Evaluate((float)aTime);

    //         // mixer.SetTime(timeTrue);

    //         // PlayableExtensions.SetTime(graph.GetRootPlayable(0), timeTrue);
    //         // PlayableExtensions.SetSpeed(graph.GetRootPlayable(0), Speed);

    //         MoveX = Mathf.Clamp(MoveX, -1f, 1f);
    //         MoveY = Mathf.Clamp(MoveY, -1f, 1f);

    //         float[] input = new float[] { MoveX, MoveY };
    //         float[] weights = interpolator.Interpolate(input, true);

    //         if (weights == null || weights.Length != clips.Length)
    //         {
    //             weights = new float[clips.Length];
    //             weights[0] = 1f;
    //         }

    //         for (int i = 0; i < clipPlayables.Length; i++)
    //         {
    //             var clip = clips[i];
    //             double length = (clip != null && clip.length > 0.0001) ? clip.length : 1.0;

    //             aTime %= 1.0;

    //             double t = aTime * length;
    //             clipPlayables[i].SetTime(t);
    //             clipPlayables[i].SetApplyFootIK(_enableAnimFootIK);
    //             clipPlayables[i].SetApplyPlayableIK(_enableIKPass);
    //             mixer.SetInputWeight(i, weights[i]);
    //         }
    //     }

    //     public override void NetworkDestroy()
    //     {
    //         if (graph.IsValid()) graph.Destroy();
    //     }

    //     void OnDestroy()
    //     {
    // if (graph.IsValid()) graph.Destroy();
    //     }

    //     void OnDisable()
    //     {
    //         if (graph.IsValid()) graph.Destroy();
    //     }
    #endregion


    [System.Serializable]
    public class BlendClip
    {
        public AnimationClip Clip;
        public Vector2 Position;
    }

    [Header("Animation References")]
    [SerializeField] private Animator _targetAnimator;
    [SerializeField] private RigBuilder _rigBuilder;

    public List<BlendClip> BlendClips = new();

    [Range(-1f, 1f)] public float MoveX;
    [Range(-1f, 1f)] public float MoveY;
    [Range(0.0f, 5f)] public float Speed = 1f;
    [Tooltip("Keeping this on at all times is recommended, disabling this might result in odd animation")]
    [SerializeField] private bool _normalizeWeights = true;
    [SerializeField] private bool _enableAnimFootIK = false;
    [SerializeField] private bool _enableIKPass = false;

    [SerializeField] private Transform _boneTR;
    [SerializeField] private Transform _boneTar;

    [SerializeField] private AvatarMask UpperBodyMask;
    [SerializeField] private AnimationClip UpperBodyClip;

    // [Header("Rig Builder")]
    // [SerializeField] private RigBuilder _rigBuilder;

    PlayableGraph graph;
    AnimationMixerPlayable baseMixer;
    AnimationClipPlayable[] baseClipPlayables;
    AnimationClip[] clips;
    PolarGradientBandInterpolator interpolator;

    AnimationMixerPlayable upperMixer;
    AnimationClipPlayable upperClipPlayable;
    AnimationLayerMixerPlayable layerMixer;

    public override void NetworkStart()
    {
        // Create the graph
        CreateGraph();
    }

    private void CreateGraph()
    {
        if (_targetAnimator == null || BlendClips == null || BlendClips.Count == 0)
        {
            Debug.LogWarning("Failed to create graph., Double click to see reason in editor, hehe");
            
            return;
        }

        int count = BlendClips.Count;
        clips = new AnimationClip[count];
        float[][] samplePoints = new float[count][];

        for (int i = 0; i < count; i++)
        {
            clips[i] = BlendClips[i].Clip;
            samplePoints[i] = new float[] { BlendClips[i].Position.x, BlendClips[i].Position.y };
        }

        interpolator = new PolarGradientBandInterpolator(samplePoints);

        // var animator = GetComponent<Animator>();

        _rigBuilder?.Build();

        // Use the rig builders graph if it isn't null otherwise create a new graph.
        // graph = _rigBuilder ? _rigBuilder.graph : PlayableGraph.Create();
        graph = _rigBuilder?.graph ?? PlayableGraph.Create();

        // TODO remove this 
        // graph = PlayableGraph.Create();

        graph.SetTimeUpdateMode(DirectorUpdateMode.Manual);

        var output = AnimationPlayableOutput.Create(graph, "Animation", _targetAnimator);

        // Base mixer
        baseMixer = AnimationMixerPlayable.Create(graph, count);
        baseClipPlayables = new AnimationClipPlayable[count];

        for (int i = 0; i < count; i++)
        {
            var c = clips[i] != null ? clips[i] : new AnimationClip();
            baseClipPlayables[i] = AnimationClipPlayable.Create(graph, c);
            baseClipPlayables[i].SetSpeed(1.0);
            baseClipPlayables[i].SetTime(0.0);
            graph.Connect(baseClipPlayables[i], 0, baseMixer, i);
            baseMixer.SetInputWeight(i, i == 0 ? 1f : 0f);
        }

        // Upper-body layer
        upperMixer = AnimationMixerPlayable.Create(graph, 1);
        if (UpperBodyClip != null)
        {
            upperClipPlayable = AnimationClipPlayable.Create(graph, UpperBodyClip);
            upperClipPlayable.SetApplyFootIK(false);
            upperClipPlayable.SetApplyPlayableIK(true);
            graph.Connect(upperClipPlayable, 0, upperMixer, 0);
            upperMixer.SetInputWeight(0, 1f);
        }

        // Layer mixer
        layerMixer = AnimationLayerMixerPlayable.Create(graph, 2);
        layerMixer.ConnectInput(0, baseMixer, 0);       // Base layer
        layerMixer.SetInputWeight(0, 1f);
        layerMixer.ConnectInput(1, upperMixer, 0);      // Upper-body layer
        layerMixer.SetInputWeight(1, 1f);

        if (UpperBodyMask != null)
            layerMixer.SetLayerMaskFromAvatarMask(1, UpperBodyMask);

        output.SetSourcePlayable(layerMixer);
        graph.Play();
    }

    public override void NetworkFixedUpdate()
    {
        if (!Object.IsProxy)
            SetAnimation(Sandbox.LocalInterpolation.Time);
    }

    public override void NetworkRender()
    {
        SetAnimation(Object.IsProxy ? Sandbox.RemoteInterpolation.Time : Sandbox.LocalInterpolation.Time);
    }

    private void SetAnimation(double aTime)
    {
        if (!graph.IsValid()) return;

        MoveX = Mathf.Clamp(MoveX, -1f, 1f);
        MoveY = Mathf.Clamp(MoveY, -1f, 1f);

        float[] input = new float[] { MoveX, MoveY };
        float[] weights = interpolator.Interpolate(input, _normalizeWeights);

        if (weights == null || weights.Length != clips.Length)
        {
            weights = new float[clips.Length];
            weights[0] = 1f;
        }

        // Base clips
        for (int i = 0; i < baseClipPlayables.Length; i++)
        {
            var clip = clips[i];
            double length = (clip != null && clip.length > 0.0001) ? clip.length : 1.0;
            double t = (aTime % length);
            baseClipPlayables[i].SetTime(t);
            baseClipPlayables[i].SetApplyFootIK(_enableAnimFootIK);
            baseClipPlayables[i].SetApplyPlayableIK(_enableIKPass);
            baseMixer.SetInputWeight(i, weights[i]);
        }

        // Upper layer clip
        if (upperClipPlayable.IsValid())
        {
            double upperLength = UpperBodyClip.length;
            upperClipPlayable.SetTime(aTime % upperLength);
            upperClipPlayable.SetApplyFootIK(false);
            upperClipPlayable.SetApplyPlayableIK(false);
            upperMixer.SetInputWeight(0, 1f);
        }

        graph.Evaluate();
    }

    public override void NetworkDestroy()
    {
        if (graph.IsValid()) graph.Destroy();
    }

    void OnDestroy()
    {
        if (graph.IsValid()) graph.Destroy();
    }

    void OnDisable()
    {
        if (graph.IsValid()) graph.Destroy();
    }
}