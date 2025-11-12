using UnityEngine;
using Netick;
using Netick.Unity;
using UnityEngine.Animations.Rigging;
using UnityEngine.Animations;
using UnityEngine.Playables;
using System.Collections.Generic;
using System;
using System.Linq;

// [DefaultExecutionOrder(1000)]
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

    // TODO use new system for blending.
    [System.Serializable]
    public class BlendClip
    {
        public AnimationClip Clip;
        public Vector2 Position;
    }

    #region Not Working XD

    // [Header("Animation References")]
    // [SerializeField] private Animator _targetAnimator;
    // [SerializeField] private RigBuilder _rigBuilder;

    // public List<BlendClip> BlendClips = new();

    // [Range(-1f, 1f)] public float MoveX;
    // [Range(-1f, 1f)] public float MoveY;
    // [Range(0.0f, 5f)] public float Speed = 1f;
    // [Tooltip("Keeping this on at all times is recommended, disabling this might result in odd animation")]
    // [SerializeField] private bool _normalizeWeights = true;
    // [SerializeField] private bool _enableAnimFootIK = false;
    // [SerializeField] private bool _enableIKPass = false;

    // [SerializeField] private Transform _boneTR;
    // [SerializeField] private Transform _boneTar;

    // [SerializeField] private AvatarMask UpperBodyMask;
    // [SerializeField] private AnimationClip UpperBodyClip;

    // // [Header("Rig Builder")]
    // // [SerializeField] private RigBuilder _rigBuilder;

    // PlayableGraph graph;
    // AnimationMixerPlayable baseMixer;
    // AnimationClipPlayable[] baseClipPlayables;
    // AnimationClip[] clips;
    // PolarGradientBandInterpolator interpolator;

    // AnimationMixerPlayable upperMixer;
    // AnimationClipPlayable upperClipPlayable;
    // AnimationLayerMixerPlayable layerMixer;

    // double manualTime;
    // public bool TakeControl = false;
    // public bool Reverse = false;
    // public float networkTime;

    // public override void NetworkStart()
    // {
    //     // Create the graph
    //     // CreateGraph();
    //     CreateGraphNew();
    // }

    // private void CreateGraphNew()
    // {

    // }

    // [Obsolete]
    // private void CreateGraph()
    // {
    //     if (_targetAnimator == null || BlendClips == null || BlendClips.Count == 0)
    //     {
    //         Debug.LogWarning("Failed to create graph., Double click to see reason in editor, hehe");

    //         return;
    //     }

    //     int count = BlendClips.Count;
    //     clips = new AnimationClip[count];
    //     float[][] samplePoints = new float[count][];

    //     for (int i = 0; i < count; i++)
    //     {
    //         clips[i] = BlendClips[i].Clip;
    //         samplePoints[i] = new float[] { BlendClips[i].Position.x, BlendClips[i].Position.y };
    //     }

    //     interpolator = new PolarGradientBandInterpolator(samplePoints);

    //     // var animator = GetComponent<Animator>();

    //     _rigBuilder?.Build();

    //     // Use the rig builders graph if it isn't null otherwise create a new graph.
    //     // graph = _rigBuilder ? _rigBuilder.graph : PlayableGraph.Create();
    //     graph = _rigBuilder?.graph ?? PlayableGraph.Create();

    //     // TODO remove this 
    //     // graph = PlayableGraph.Create();

    //     graph.SetTimeUpdateMode(DirectorUpdateMode.Manual);

    //     var output = AnimationPlayableOutput.Create(graph, "Animation", _targetAnimator);

    //     // Base mixer
    //     baseMixer = AnimationMixerPlayable.Create(graph, count);
    //     baseClipPlayables = new AnimationClipPlayable[count];

    //     for (int i = 0; i < count; i++)
    //     {
    //         var c = clips[i] != null ? clips[i] : new AnimationClip();
    //         baseClipPlayables[i] = AnimationClipPlayable.Create(graph, c);
    //         baseClipPlayables[i].SetSpeed(1.0);
    //         baseClipPlayables[i].SetTime(0.0);
    //         graph.Connect(baseClipPlayables[i], 0, baseMixer, i);
    //         baseMixer.SetInputWeight(i, i == 0 ? 1f : 0f);
    //     }

    //     // Upper-body layer
    //     upperMixer = AnimationMixerPlayable.Create(graph, 1);
    //     if (UpperBodyClip != null)
    //     {
    //         upperClipPlayable = AnimationClipPlayable.Create(graph, UpperBodyClip);
    //         upperClipPlayable.SetApplyFootIK(false);
    //         upperClipPlayable.SetApplyPlayableIK(true);
    //         graph.Connect(upperClipPlayable, 0, upperMixer, 0);
    //         upperMixer.SetInputWeight(0, 1f);
    //     }

    //     // Layer mixer
    //     layerMixer = AnimationLayerMixerPlayable.Create(graph, 2);
    //     layerMixer.ConnectInput(0, baseMixer, 0);       // Base layer
    //     layerMixer.SetInputWeight(0, 1f);
    //     layerMixer.ConnectInput(1, upperMixer, 0);      // Upper-body layer
    //     layerMixer.SetInputWeight(1, 1f);

    //     if (UpperBodyMask != null)
    //         layerMixer.SetLayerMaskFromAvatarMask(1, UpperBodyMask);

    //     output.SetSourcePlayable(layerMixer);
    //     graph.Play();
    // }

    // public override void NetworkFixedUpdate()
    // {
    //     if (!Object.IsProxy)
    //         SetAnimationNew(Sandbox.LocalInterpolation.Time);
    // }

    // public override void NetworkRender()
    // {
    //     // SetAnimation(Object.IsProxy ? Sandbox.RemoteInterpolation.Time : Sandbox.LocalInterpolation.Time);
    //     SetAnimationNew(Object.IsProxy ? Sandbox.RemoteInterpolation.Time : Sandbox.LocalInterpolation.Time);
    // }

    // [Obsolete]
    // private void SetAnimation(double aTime)
    // {
    //     if (!graph.IsValid()) return;

    //     MoveX = Mathf.Clamp(MoveX, -1f, 1f);
    //     MoveY = Mathf.Clamp(MoveY, -1f, 1f);

    //     float[] input = new float[] { MoveX, MoveY };
    //     float[] weights = interpolator.Interpolate(input, _normalizeWeights);

    //     if (weights == null || weights.Length != clips.Length)
    //     {
    //         weights = new float[clips.Length];
    //         weights[0] = 1f;
    //     }

    //     // Base clips
    //     for (int i = 0; i < baseClipPlayables.Length; i++)
    //     {
    //         var clip = clips[i];
    //         double length = (clip != null && clip.length > 0.0001) ? clip.length : 1.0;
    //         double t = (aTime % length);
    //         baseClipPlayables[i].SetTime(t);
    //         baseClipPlayables[i].SetApplyFootIK(_enableAnimFootIK);
    //         baseClipPlayables[i].SetApplyPlayableIK(_enableIKPass);
    //         baseMixer.SetInputWeight(i, weights[i]);
    //     }

    //     // Upper layer clip
    //     if (upperClipPlayable.IsValid())
    //     {
    //         double upperLength = UpperBodyClip.length;
    //         upperClipPlayable.SetTime(aTime % upperLength);
    //         upperClipPlayable.SetApplyFootIK(false);
    //         upperClipPlayable.SetApplyPlayableIK(false);
    //         upperMixer.SetInputWeight(0, 1f);
    //     }

    //     graph.Evaluate();
    // }

    // private void SetAnimationNew(double networkTime)
    // {

    // }

    // public override void NetworkDestroy()
    // {
    //     if (graph.IsValid()) graph.Destroy();
    // }

    // void OnDestroy()
    // {
    //     if (graph.IsValid()) graph.Destroy();
    // }

    // void OnDisable()
    // {
    //     if (graph.IsValid()) graph.Destroy();
    // }

    #endregion




    // TODO try to create our own graph
    // [Header("Animation References")]
    // [SerializeField] private Animator _targetAnimator;
    // [SerializeField] private RigBuilder _rigBuilder;

    // public List<BlendClip> BlendClips = new();

    // [Range(-1f, 1f)] public float MoveX;
    // [Range(-1f, 1f)] public float MoveY;
    // [Range(0f, 5f)] public float Speed = 1f;

    // [SerializeField] private bool _enableAnimFootIK = true;
    // [SerializeField] private bool _enableIKPass = false;
    // [SerializeField] private bool _normalizeWeights = true;

    // private PlayableGraph graph;
    // private AnimationMixerPlayable mixer;
    // private AnimationClipPlayable[] clipPlayables;
    // private AnimationClip[] clips;
    // private PolarGradientBandInterpolator interpolator;

    // private double manualTime;
    // private double lastNetworkTime;
    // private double effectivePhaseSpeed;

    // public bool TakeControl = false;
    // public bool Reverse = false;
    // public float networkTime;
    [Header("Graph Management")]
    [SerializeField] private RigBuilder _rigBuilder;
    [SerializeField] private Animator _targetAnimator;
    // [SerializeField] private CreateGraphMode GraphCreationMode;
    // [SerializeField] private string GraphName = "CustomAnimationGraph";
    [SerializeField] private bool InvokeSyncLayers = true;

    public List<BlendClip> BlendClips = new();
    public AnimationClip UpperBodyClip;
    public AvatarMask UpperBodyMask;

    // [Range(-1f, 1f)] public float MoveX;
    // [Range(-1f, 1f)] public float MoveY;
    [Networked, Smooth(false)] public float MoveX { get; set; }
    [Networked, Smooth(false)] public float MoveY { get; set; }

    [Networked, Smooth(false)] public Vector2 Movement { get; set; }
    public float LerpSpeed = 10;
    public float Speed = 1f;

    [SerializeField] private bool _animStateFootIK = true;
    [SerializeField] private bool _animIKPass = false;

    PlayableGraph graph;
    AnimationMixerPlayable mixer;
    AnimationClipPlayable[] clipPlayables;
    AnimationClip[] clips;
    PolarGradientBandInterpolator interpolator;
    double manualTime;
    public bool TakeControl = false;
    public bool ContinueUpdatingEvenWhenTakeControl = false;
    public bool Reverse = false;
    // public double networkTime;

    double lastNetworkTime = 0.0f;
    double effectivePhaseSpeed = 0.0;

    public Action OnSetValues;

    [Networked, Smooth] public float AnimTime { get; set; }

    void Start()
    {
        InitializeGraph();
        // CreateGraph();
        CreateGraphSimple();
        SetupSystem();
    }

    private void InitializeGraph()
    {
        // Init the rig builder and use it's graph.
        _rigBuilder.Build();
        graph = _rigBuilder.graph;
        graph.SetTimeUpdateMode(DirectorUpdateMode.Manual);
    }

    // private void CreateGraph()
    // {
    //     if (_targetAnimator == null || BlendClips == null || BlendClips.Count == 0)
    //     {
    //         Debug.LogError("TickAnimator: Assign Animator and BlendClips before starting!");
    //         return;
    //     }

    //     int count = BlendClips.Count;
    //     clips = new AnimationClip[count];
    //     float[][] samplePoints = new float[count][];

    //     for (int i = 0; i < count; i++)
    //     {
    //         clips[i] = BlendClips[i].Clip;
    //         samplePoints[i] = new float[] { BlendClips[i].Position.x, BlendClips[i].Position.y };
    //     }

    //     interpolator = new PolarGradientBandInterpolator(samplePoints);

    //     var output = AnimationPlayableOutput.Create(graph, "Animation", _targetAnimator);

    //     mixer = AnimationMixerPlayable.Create(graph, count);
    //     clipPlayables = new AnimationClipPlayable[count];

    //     for (int i = 0; i < count; i++)
    //     {
    //         var c = clips[i] != null ? clips[i] : new AnimationClip();
    //         clipPlayables[i] = AnimationClipPlayable.Create(graph, c);
    //         clipPlayables[i].SetSpeed(1.0);
    //         clipPlayables[i].SetTime(0.0);
    //         graph.Connect(clipPlayables[i], 0, mixer, i);
    //         mixer.SetInputWeight(i, i == 0 ? 1f : 0f);
    //     }

    //     output.SetSourcePlayable(mixer);
    //     graph.Play();
    // }

    private void CreateGraphSimple()
    {
        if (_targetAnimator == null || BlendClips == null || BlendClips.Count == 0)
        {
            Debug.LogError("TickAnimator: Assign Animator and BlendClips before starting!");
            return;
        }

        // Layer mixer (2 layers)
        var layerMixer = AnimationLayerMixerPlayable.Create(graph, 2);

        // Upper body clip
        var upperBodyClipPlayable = AnimationClipPlayable.Create(graph, UpperBodyClip);
        upperBodyClipPlayable.SetApplyPlayableIK(_animIKPass);

        // Blend clip mixer
        mixer = AnimationMixerPlayable.Create(graph, BlendClips.Count);


        /// ----TODO--- Create sample points using blend clips
        clips = new AnimationClip[BlendClips.Count];
        float[][] samplePoints = new float[BlendClips.Count][];

        for (int i = 0; i < BlendClips.Count; i++)
        {
            clips[i] = BlendClips[i].Clip;
            samplePoints[i] = new float[] { BlendClips[i].Position.x, BlendClips[i].Position.y };
        }

        // Create the interpolator for this blend tree (mixer) using sample points TODO
        interpolator = new PolarGradientBandInterpolator(samplePoints);

        // Initalize playable clips list
        clipPlayables = new AnimationClipPlayable[BlendClips.Count];

        // Setup mixer with clip playables
        for (int i = 0; i < BlendClips.Count; i++)
        {
            // Create and setup this clip as a clip playable
            clipPlayables[i] = AnimationClipPlayable.Create(graph, BlendClips[i].Clip);
            clipPlayables[i].SetSpeed(1);
            clipPlayables[i].SetTime(0);
            clipPlayables[i].SetApplyPlayableIK(_animIKPass);

            // Connect it to the mixer and set the input weight
            graph.Connect(clipPlayables[i], 0, mixer, i);
            mixer.SetInputWeight(i, i == 0 ? 1f : 0f);
        }
        // var baseClipPlayable = AnimationClipPlayable.Create(graph, BlendClips[1].Clip);
        // baseClipPlayable.SetSpeed(1);
        // baseClipPlayable.SetTime(0);

        // graph.Connect(baseClipPlayable, 0, mixer, 0);
        // mixer.SetInputWeight(0, 1);

        // Connect layers correctly
        graph.Connect(mixer, 0, layerMixer, 0);
        graph.Connect(upperBodyClipPlayable, 0, layerMixer, 1);

        // Apply avatar mask to layer 1
        layerMixer.SetLayerMaskFromAvatarMask(1, UpperBodyMask);

        layerMixer.SetInputWeight(0, 1f);
        layerMixer.SetInputWeight(1, 1f);

        var output = AnimationPlayableOutput.Create(graph, "Animation", _targetAnimator);
        output.SetSourcePlayable(layerMixer);

        graph.Play();
    }
    private void CreateGraph()
    {
        if (_targetAnimator == null || BlendClips == null || BlendClips.Count == 0)
        {
            Debug.LogError("TickAnimator: Assign Animator and BlendClips before starting!");
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

        // Base locomotion mixer
        mixer = AnimationMixerPlayable.Create(graph, count);
        clipPlayables = new AnimationClipPlayable[count];

        for (int i = 0; i < count; i++)
        {
            var c = clips[i] != null ? clips[i] : new AnimationClip();
            clipPlayables[i] = AnimationClipPlayable.Create(graph, c);
            clipPlayables[i].SetSpeed(1.0);
            clipPlayables[i].SetTime(0.0);
            graph.Connect(clipPlayables[i], 0, mixer, i);
            mixer.SetInputWeight(i, i == 0 ? 1f : 0f);
        }

        // Layer mixer (2 layers)
        var layerMixer = AnimationLayerMixerPlayable.Create(graph, 2);

        // Upper body clip
        var upperBodyClipPlayable = AnimationClipPlayable.Create(graph, UpperBodyClip);
        graph.Connect(mixer, 0, layerMixer, 0);
        graph.Connect(upperBodyClipPlayable, 0, layerMixer, 1);

        // Apply avatar mask to layer 1
        layerMixer.SetLayerMaskFromAvatarMask(1, UpperBodyMask);

        layerMixer.SetInputWeight(0, 1f);
        layerMixer.SetInputWeight(1, 1f);

        var output = AnimationPlayableOutput.Create(graph, "Animation", _targetAnimator);
        output.SetSourcePlayable(layerMixer);

        graph.Play();
    }

    private void SetupSystem()
    {
        manualTime = 0.0;
        // lastNetworkTime = networkTime;
        effectivePhaseSpeed = 0.0;
    }

    public override void NetworkFixedUpdate()
    {
        if (!Object.IsProxy)
        {
            OnSetValues.Invoke();

            Movement = Vector2.Lerp(Movement, new Vector2(MoveX, MoveY), LerpSpeed * Sandbox.FixedDeltaTime);

            AnimTime += Sandbox.FixedDeltaTime;

            // SetAnimation(Sandbox.LocalInterpolation.Time);
            // Animate(MoveX, MoveY, Sandbox.LocalInterpolation.Time);

            // Animate(MoveX, MoveY, AnimTime);
        }

        Animate(Movement, AnimTime);
    }

    public override void NetworkRender()
    {
        // SetAnimation(Object.IsProxy ? Sandbox.RemoteInterpolation.Time : Sandbox.LocalInterpolation.Time);
        // Animate(MoveX, MoveY, Object.IsProxy ? Sandbox.RemoteInterpolation.Time : Sandbox.LocalInterpolation.Time);

        // Animate(MoveX, MoveY, AnimTime);
    }

    private void Animate(Vector2 movement, float time)
    {
        if (!graph.IsValid()) return;

        // Set mixer time.
        mixer.SetPropagateSetTime(true);
        mixer.SetTime(time);

        movement = Vector2.ClampMagnitude(movement, 1);

        // moveX = Mathf.Clamp(moveX, -1f, 1f);
        // moveY = Mathf.Clamp(moveY, -1f, 1f);

        float[] input = new float[] { movement.x, movement.y };
        float[] weights = interpolator.Interpolate(input, true);

        for (int i = 0; i < clipPlayables.Length; i++)
        {
            mixer.SetInputWeight(i, weights[i]);
        }



        if (InvokeSyncLayers) _rigBuilder.SyncLayers();
        graph.Evaluate();
    }

    private void SetAnimation(double currentNetworkTime)
    {
        if (!graph.IsValid()) return;

        // if (!TakeControl) networkTime += Time.deltaTime;

        float cMoveX = Mathf.Clamp(MoveX, -1f, 1f);
        float cMoveY = Mathf.Clamp(MoveY, -1f, 1f);

        float[] input = new float[] { cMoveX, cMoveY };
        float[] weights = interpolator.Interpolate(input, true);

        if (weights == null || weights.Length != clips.Length)
        {
            weights = new float[clips.Length];
            weights[0] = 1f;
        }

        // Compute weighted effective duration of the blend
        double weightedDuration = 0.0;
        double totalWeight = 0.0;
        for (int i = 0; i < clips.Length; i++)
        {
            if (clips[i] == null || clips[i].length < 0.0001) continue;
            weightedDuration += clips[i].length * weights[i];
            totalWeight += weights[i];
        }
        if (totalWeight > 0.0001)
            weightedDuration /= totalWeight;
        else
            weightedDuration = 1.0;

        // Advance manualTime based on network time delta and weighted duration
        double deltaTime = currentNetworkTime - lastNetworkTime;
        lastNetworkTime = currentNetworkTime;

        manualTime += (deltaTime / weightedDuration) * Speed;
        manualTime %= 1.0;

        // Apply manualTime to each clip
        for (int i = 0; i < clipPlayables.Length; i++)
        {
            var clip = clips[i];
            if (clip == null || clip.length < 0.0001) continue;

            double t = manualTime * clip.length;
            clipPlayables[i].SetTime(t);
            clipPlayables[i].SetApplyFootIK(_animStateFootIK);
            clipPlayables[i].SetApplyPlayableIK(_animIKPass);

            mixer.SetInputWeight(i, weights[i]);
        }

        if (InvokeSyncLayers) _rigBuilder.SyncLayers();
        graph.Evaluate(0f);
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