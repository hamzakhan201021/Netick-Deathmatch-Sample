using UnityEngine;
using Netick;
using Netick.Unity;
using UnityEngine.Animations.Rigging;
using UnityEngine.Animations;
using UnityEngine.Playables;
using System.Collections.Generic;
using System;
using System.Linq;
using Unity.VisualScripting;


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






    // Below original with only one blend tree
    // [Header("Graph Management")]
    // [SerializeField] private RigBuilder _rigBuilder;
    // [SerializeField] private Animator _targetAnimator;
    // // [SerializeField] private CreateGraphMode GraphCreationMode;
    // // [SerializeField] private string GraphName = "CustomAnimationGraph";
    // [SerializeField] private bool InvokeSyncLayers = true;

    // [Header("Locomotion")]
    // public List<BlendClip> WalkBlendClips = new();
    // public List<BlendClip> RunBlendClips = new();
    // public List<BlendClip> CrouchBlendClips = new();
    // [Header("Upper Body")]
    // public AnimationClip UpperBodyClip;
    // public AvatarMask UpperBodyMask;

    // // [Range(-1f, 1f)] public float MoveX;
    // // [Range(-1f, 1f)] public float MoveY;

    // public float LerpSpeed = 10;
    // public float Speed = 1f;
    // [SerializeField] private bool _animStateFootIK = true;
    // [SerializeField] private bool _animIKPass = false;

    // // Networked properties
    // [Networked, Smooth(false)] public float MoveX { get; set; }
    // [Networked, Smooth(false)] public float MoveY { get; set; }
    // [Networked, Smooth(false)] public float StateValue { get; set; }
    // [Networked, Smooth(false)] public Vector2 Movement { get; set; }

    // private PlayableGraph graph;
    // private AnimationMixerPlayable mixer;
    // private AnimationClipPlayable[] clipPlayables;
    // private AnimationClip[] clips;
    // private PolarGradientBandInterpolator interpolator;
    // private double manualTime;

    // [Header("Debugging (might not work XD)")]
    // public bool TakeControl = false;
    // public bool ContinueUpdatingEvenWhenTakeControl = false;
    // public bool Reverse = false;
    // // public double networkTime;

    // private double lastNetworkTime = 0.0f;
    // private double effectivePhaseSpeed = 0.0;

    // [Tooltip("Listen to this to set values")]
    // public Action OnSetValues;

    // [Networked, Smooth] public float AnimTime { get; set; }

    // void Start()
    // {
    //     InitializeGraph();
    //     // CreateGraphOld();
    //     CreateGraphSimple();
    //     SetupSystem();
    // }

    // private void InitializeGraph()
    // {
    //     // Init the rig builder and use it's graph.
    //     _rigBuilder.Build();
    //     graph = _rigBuilder.graph;
    //     graph.SetTimeUpdateMode(DirectorUpdateMode.Manual);
    // }

    // private void CreateGraphSimple()
    // {
    //     if (_targetAnimator == null || WalkBlendClips == null || WalkBlendClips.Count == 0)
    //     {
    //         Debug.LogError("TickAnimator: Assign Animator and BlendClips before starting!");
    //         return;
    //     }

    //     // Layer mixer (2 layers)
    //     var layerMixer = AnimationLayerMixerPlayable.Create(graph, 2);

    //     // Upper body clip
    //     var upperBodyClipPlayable = AnimationClipPlayable.Create(graph, UpperBodyClip);
    //     upperBodyClipPlayable.SetApplyPlayableIK(_animIKPass);

    //     // Blend clip mixer
    //     mixer = AnimationMixerPlayable.Create(graph, WalkBlendClips.Count);


    //     /// ----TODO--- Create sample points using blend clips
    //     clips = new AnimationClip[WalkBlendClips.Count];
    //     float[][] samplePoints = new float[WalkBlendClips.Count][];

    //     for (int i = 0; i < WalkBlendClips.Count; i++)
    //     {
    //         clips[i] = WalkBlendClips[i].Clip;
    //         samplePoints[i] = new float[] { WalkBlendClips[i].Position.x, WalkBlendClips[i].Position.y };
    //     }

    //     // Create the interpolator for this blend tree (mixer) using sample points TODO
    //     interpolator = new PolarGradientBandInterpolator(samplePoints);

    //     // Initalize playable clips list
    //     clipPlayables = new AnimationClipPlayable[WalkBlendClips.Count];

    //     // Setup mixer with clip playables
    //     for (int i = 0; i < WalkBlendClips.Count; i++)
    //     {
    //         // Create and setup this clip as a clip playable
    //         clipPlayables[i] = AnimationClipPlayable.Create(graph, WalkBlendClips[i].Clip);
    //         clipPlayables[i].SetSpeed(1);
    //         clipPlayables[i].SetTime(0);
    //         clipPlayables[i].SetApplyPlayableIK(_animIKPass);

    //         // Connect it to the mixer and set the input weight
    //         graph.Connect(clipPlayables[i], 0, mixer, i);
    //         mixer.SetInputWeight(i, i == 0 ? 1f : 0f);
    //     }

    //     // Connect layers correctly
    //     graph.Connect(mixer, 0, layerMixer, 0);
    //     graph.Connect(upperBodyClipPlayable, 0, layerMixer, 1);

    //     // Apply avatar mask to layer 1
    //     layerMixer.SetLayerMaskFromAvatarMask(1, UpperBodyMask);

    //     layerMixer.SetInputWeight(0, 1f);
    //     layerMixer.SetInputWeight(1, 1f);

    //     var output = AnimationPlayableOutput.Create(graph, "Animation", _targetAnimator);
    //     output.SetSourcePlayable(layerMixer);

    //     graph.Play();
    // }


    // private void SetupSystem()
    // {
    //     manualTime = 0.0;
    //     // lastNetworkTime = networkTime;
    //     effectivePhaseSpeed = 0.0;
    // }

    // public override void NetworkFixedUpdate()
    // {
    //     if (!Object.IsProxy)
    //     {
    //         OnSetValues.Invoke();

    //         Movement = Vector2.Lerp(Movement, new Vector2(MoveX, MoveY), LerpSpeed * Sandbox.FixedDeltaTime);

    //         AnimTime += Sandbox.FixedDeltaTime;

    //         // SetAnimationOld(Sandbox.LocalInterpolation.Time);
    //         // Animate(MoveX, MoveY, Sandbox.LocalInterpolation.Time);

    //         // Animate(MoveX, MoveY, AnimTime);
    //     }

    //     Animate(Movement, AnimTime);
    // }

    // public override void NetworkRender()
    // {
    //     // Sadly the approach of using interpolation time (and using interpolation for lag compensation so far hasn't worked)
    //     // SetAnimationOld(Object.IsProxy ? Sandbox.RemoteInterpolation.Time : Sandbox.LocalInterpolation.Time);
    //     // Animate(MoveX, MoveY, Object.IsProxy ? Sandbox.RemoteInterpolation.Time : Sandbox.LocalInterpolation.Time);

    //     // Animate(MoveX, MoveY, AnimTime);
    // }

    // private void Animate(Vector2 movement, float time)
    // {
    //     if (!graph.IsValid()) return;

    //     // Set mixer time.
    //     mixer.SetPropagateSetTime(true);
    //     mixer.SetTime(time);

    //     movement = Vector2.ClampMagnitude(movement, 1);

    //     // moveX = Mathf.Clamp(moveX, -1f, 1f);
    //     // moveY = Mathf.Clamp(moveY, -1f, 1f);

    //     float[] input = new float[] { movement.x, movement.y };
    //     float[] weights = interpolator.Interpolate(input, true);

    //     for (int i = 0; i < clipPlayables.Length; i++)
    //     {
    //         mixer.SetInputWeight(i, weights[i]);
    //     }



    //     if (InvokeSyncLayers) _rigBuilder.SyncLayers();
    //     graph.Evaluate();
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























    // [Header("Graph Management")]
    // [SerializeField] private RigBuilder _rigBuilder;
    // [SerializeField] private Animator _targetAnimator;
    // [SerializeField] private bool InvokeSyncLayers = true;

    // [Header("Locomotion")]
    // public List<BlendClip> WalkBlendClips = new();
    // public List<BlendClip> RunBlendClips = new();
    // public List<BlendClip> CrouchBlendClips = new();

    // [Header("Upper Body")]
    // public AnimationClip UpperBodyClip;
    // public AvatarMask UpperBodyMask;

    // public float LerpSpeed = 10;

    // [Networked, Smooth(false)] public float MoveX { get; set; }
    // [Networked, Smooth(false)] public float MoveY { get; set; }
    // [Networked, Smooth(false)] public float StateValue { get; set; } // 0 = crouch, 0.5 = walk, 1 = run
    // [Networked, Smooth(false)] public Vector2 MovementSmooth { get; set; }
    // [Networked, Smooth(false)] public float StateValueSmooth { get; set; } // 0 = crouch, 0.5 = walk, 1 = run
    // [Networked, Smooth] public float AnimTime { get; set; }

    // private PlayableGraph graph;
    // private AnimationMixerPlayable walkMixer;
    // private AnimationMixerPlayable runMixer;
    // private AnimationMixerPlayable crouchMixer;
    // private AnimationMixerPlayable locomotionMixer;
    // private AnimationClipPlayable[] walkClips;
    // private AnimationClipPlayable[] runClips;
    // private AnimationClipPlayable[] crouchClips;
    // private PolarGradientBandInterpolator walkInterpolator;
    // private PolarGradientBandInterpolator runInterpolator;
    // private PolarGradientBandInterpolator crouchInterpolator;

    // public Action OnSetValues;

    // void Start()
    // {
    //     InitializeGraph();
    //     CreateLocomotionGraphs();
    // }

    // private void InitializeGraph()
    // {
    //     _rigBuilder.Build();
    //     graph = _rigBuilder.graph;
    //     graph.SetTimeUpdateMode(DirectorUpdateMode.Manual);
    // }

    // private void CreateLocomotionGraphs()
    // {
    //     if (_targetAnimator == null) return;

    //     // --- Walk Mixer ---
    //     walkMixer = AnimationMixerPlayable.Create(graph, WalkBlendClips.Count);
    //     walkClips = new AnimationClipPlayable[WalkBlendClips.Count];
    //     float[][] walkSamplePoints = new float[WalkBlendClips.Count][];
    //     for (int i = 0; i < WalkBlendClips.Count; i++)
    //     {
    //         walkClips[i] = AnimationClipPlayable.Create(graph, WalkBlendClips[i].Clip);
    //         walkClips[i].SetApplyPlayableIK(false);
    //         graph.Connect(walkClips[i], 0, walkMixer, i);
    //         walkMixer.SetInputWeight(i, i == 0 ? 1f : 0f);
    //         walkSamplePoints[i] = new float[] { WalkBlendClips[i].Position.x, WalkBlendClips[i].Position.y };
    //     }
    //     walkInterpolator = new PolarGradientBandInterpolator(walkSamplePoints);

    //     // --- Run Mixer ---
    //     runMixer = AnimationMixerPlayable.Create(graph, RunBlendClips.Count);
    //     runClips = new AnimationClipPlayable[RunBlendClips.Count];
    //     float[][] runSamplePoints = new float[RunBlendClips.Count][];
    //     for (int i = 0; i < RunBlendClips.Count; i++)
    //     {
    //         runClips[i] = AnimationClipPlayable.Create(graph, RunBlendClips[i].Clip);
    //         runClips[i].SetApplyPlayableIK(false);
    //         graph.Connect(runClips[i], 0, runMixer, i);
    //         runMixer.SetInputWeight(i, i == 0 ? 1f : 0f);
    //         runSamplePoints[i] = new float[] { RunBlendClips[i].Position.x, RunBlendClips[i].Position.y };
    //     }
    //     runInterpolator = new PolarGradientBandInterpolator(runSamplePoints);

    //     // --- Crouch Mixer ---
    //     crouchMixer = AnimationMixerPlayable.Create(graph, CrouchBlendClips.Count);
    //     crouchClips = new AnimationClipPlayable[CrouchBlendClips.Count];
    //     float[][] crouchSamplePoints = new float[CrouchBlendClips.Count][];
    //     for (int i = 0; i < CrouchBlendClips.Count; i++)
    //     {
    //         crouchClips[i] = AnimationClipPlayable.Create(graph, CrouchBlendClips[i].Clip);
    //         crouchClips[i].SetApplyPlayableIK(false);
    //         graph.Connect(crouchClips[i], 0, crouchMixer, i);
    //         crouchMixer.SetInputWeight(i, i == 0 ? 1f : 0f);
    //         crouchSamplePoints[i] = new float[] { CrouchBlendClips[i].Position.x, CrouchBlendClips[i].Position.y };
    //     }
    //     crouchInterpolator = new PolarGradientBandInterpolator(crouchSamplePoints);

    //     // --- Combine all locomotion mixers ---
    //     locomotionMixer = AnimationMixerPlayable.Create(graph, 3);
    //     graph.Connect(crouchMixer, 0, locomotionMixer, 0);
    //     graph.Connect(walkMixer, 0, locomotionMixer, 1);
    //     graph.Connect(runMixer, 0, locomotionMixer, 2);

    //     // Upper body clip
    //     var upperBodyPlayable = AnimationClipPlayable.Create(graph, UpperBodyClip);
    //     var layerMixer = AnimationLayerMixerPlayable.Create(graph, 2);
    //     graph.Connect(locomotionMixer, 0, layerMixer, 0);
    //     graph.Connect(upperBodyPlayable, 0, layerMixer, 1);
    //     layerMixer.SetLayerMaskFromAvatarMask(1, UpperBodyMask);
    //     layerMixer.SetInputWeight(0, 1f);
    //     layerMixer.SetInputWeight(1, 1f);

    //     var output = AnimationPlayableOutput.Create(graph, "Animation", _targetAnimator);
    //     output.SetSourcePlayable(layerMixer);

    //     graph.Play();
    // }

    // public override void NetworkFixedUpdate()
    // {
    //     if (!Object.IsProxy)
    //     {
    //         OnSetValues.Invoke();

    //         Vector2 currentMovement = new Vector2(MoveX, MoveY);
    //         MovementSmooth = Vector2.Lerp(MovementSmooth, currentMovement, LerpSpeed * Sandbox.FixedDeltaTime);
    //         StateValueSmooth = Mathf.Lerp(StateValueSmooth, StateValue, LerpSpeed * Sandbox.FixedDeltaTime);

    //         // Blend all mixers using StateValue: 0 = crouch, 0.5 = walk, 1 = run
    //         float crouchWeight = Mathf.Clamp01(1f - (StateValueSmooth * 2f));       // 0->1 when StateValue 0->0.5
    //         float walkWeight = Mathf.Clamp01(1f - Mathf.Abs(StateValueSmooth - 0.5f) * 2f); // peak at 0.5
    //         float runWeight = Mathf.Clamp01((StateValueSmooth - 0.5f) * 2f);         // 0->1 when StateValue 0.5->1

    //         locomotionMixer.SetInputWeight(0, crouchWeight);
    //         locomotionMixer.SetInputWeight(1, walkWeight);
    //         locomotionMixer.SetInputWeight(2, runWeight);

    //         // Animate each blend tree internally
    //         AnimateBlendTree(crouchMixer, crouchClips, crouchInterpolator, MovementSmooth);
    //         AnimateBlendTree(walkMixer, walkClips, walkInterpolator, MovementSmooth);
    //         AnimateBlendTree(runMixer, runClips, runInterpolator, MovementSmooth);

    //         AnimTime += Sandbox.FixedDeltaTime;
    //         locomotionMixer.SetTime(AnimTime);
    //     }
    // }

    // private void AnimateBlendTree(AnimationMixerPlayable mixer, AnimationClipPlayable[] clips, PolarGradientBandInterpolator interpolator, Vector2 input)
    // {
    //     if (!graph.IsValid()) return;
    //     mixer.SetPropagateSetTime(true);
    //     mixer.SetTime(AnimTime);
    //     // input = Vector2.ClampMagnitude(input, 1);
    //     float[] weights = interpolator.Interpolate(new float[] { MovementSmooth.x, MovementSmooth.y }, true);
    //     for (int i = 0; i < clips.Length; i++)
    //     {
    //         mixer.SetInputWeight(i, weights[i]);
    //     }
    //     graph.Evaluate();
    // }

    // public override void NetworkRender() { }

    // public override void NetworkDestroy()
    // {
    //     if (graph.IsValid()) graph.Destroy();
    // }

    // void OnDestroy()
    // {
    //     if (graph.IsValid()) graph.Destroy();
    // }
    // [Header("Graph Management")]
    // [SerializeField] private RigBuilder _rigBuilder;
    // [SerializeField] private Animator _targetAnimator;
    // [SerializeField] private bool InvokeSyncLayers = true;

    // [Header("Locomotion")]
    // [SerializeField] private bool _enableAnimFootIK = true;
    // [SerializeField] private bool _enableIKPass = false;
    // public List<BlendClip> WalkBlendClips = new();
    // public List<BlendClip> RunBlendClips = new();
    // public List<BlendClip> CrouchBlendClips = new();
    // public AnimationClip AirBorneClip;


    // [Header("Upper Body")]
    // public AnimationClip UpperBodyClip;
    // public AvatarMask UpperBodyMask;

    // [Header("Smoothing")]
    // public float LerpSpeed = 10;
    // [SerializeField] private AnimationCurve SmoothCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    // [Networked, Smooth] public float MoveX { get; set; }
    // [Networked, Smooth] public float MoveY { get; set; }
    // [Networked, Smooth] public float StateValue { get; set; } // 0=crouch, 0.5=walk, 1=run
    // [Networked, Smooth] public Vector2 MovementSmooth { get; set; }
    // [Networked, Smooth] public float StateValueSmooth { get; set; }
    // [Networked, Smooth] public float AnimTime { get; set; }

    // private PlayableGraph graph;
    // private AnimationMixerPlayable locomotionMixer;
    // private AnimationMixerPlayable walkMixer;
    // private AnimationMixerPlayable runMixer;
    // private AnimationMixerPlayable crouchMixer;

    // private AnimationClipPlayable[] walkClips;
    // private AnimationClipPlayable[] runClips;
    // private AnimationClipPlayable[] crouchClips;

    // private PolarGradientBandInterpolator walkInterpolator;
    // private PolarGradientBandInterpolator runInterpolator;
    // private PolarGradientBandInterpolator crouchInterpolator;

    // public Action OnSetValues;

    // void Start()
    // {
    //     InitializeGraph();
    //     CreatePlayableGraph();
    // }

    // private void InitializeGraph()
    // {
    //     _rigBuilder.Build();
    //     graph = _rigBuilder.graph;
    //     graph.SetTimeUpdateMode(DirectorUpdateMode.Manual);
    // }

    // private void CreatePlayableGraph()
    // {
    //     if (_targetAnimator == null) return;

    //     // Create all blend trees
    //     (walkMixer, walkClips, walkInterpolator) = CreateBlendTree(WalkBlendClips);
    //     (runMixer, runClips, runInterpolator) = CreateBlendTree(RunBlendClips);
    //     (crouchMixer, crouchClips, crouchInterpolator) = CreateBlendTree(CrouchBlendClips);

    //     // Initialize mixer for all states walk/run/crouch/airborne
    //     locomotionMixer = AnimationMixerPlayable.Create(graph, 4);
    //     graph.Connect(crouchMixer, 0, locomotionMixer, 0);
    //     graph.Connect(walkMixer, 0, locomotionMixer, 1);
    //     graph.Connect(runMixer, 0, locomotionMixer, 2);

    //     // Now using Create playable from clip instead of manual stuff
    //     // var airborneClipPlayable = AnimationClipPlayable.Create(graph, AirBorneClip);
    //     // airborneClipPlayable.SetApplyPlayableIK(_enableIKPass);
    //     // airborneClipPlayable.SetApplyFootIK(_enableAnimFootIK);
    //     graph.Connect(CreatePlayableFromClip(AirBorneClip), 0, locomotionMixer, 3);// tODO simplify rest of the script
    //     // graph.Connect(airborneClipPlayable, 0, locomotionMixer, 3);

    //     // Now using Create playable from clip instead of manual stuff
    //     // var upperBodyPlayable = AnimationClipPlayable.Create(graph, UpperBodyClip);
    //     // upperBodyPlayable.SetApplyPlayableIK(_enableIKPass);
    //     // upperBodyPlayable.SetApplyFootIK(_enableAnimFootIK);

    //     // Create main layer mixer
    //     var layerMixer = AnimationLayerMixerPlayable.Create(graph, 2);

    //     // Connect playable's to the main layer mixer
    //     graph.Connect(locomotionMixer, 0, layerMixer, 0);
    //     graph.Connect(CreatePlayableFromClip(UpperBodyClip), 0, layerMixer, 1);

    //     // Set masks and weights
    //     layerMixer.SetLayerMaskFromAvatarMask(1, UpperBodyMask);
    //     layerMixer.SetInputWeight(0, 1f);
    //     layerMixer.SetInputWeight(1, 1f);

    //     // Setup output
    //     var output = AnimationPlayableOutput.Create(graph, "Animation", _targetAnimator);
    //     output.SetSourcePlayable(layerMixer);

    //     // Play the graph (it won't actually play..., just calling it to start the graph?...)
    //     graph.Play();
    // }

    // private (AnimationMixerPlayable, AnimationClipPlayable[], PolarGradientBandInterpolator)
    //     CreateBlendTree(List<BlendClip> clips)
    // {
    //     int count = clips.Count;
    //     var mixer = AnimationMixerPlayable.Create(graph, count);
    //     var clipPlayables = new AnimationClipPlayable[count];
    //     float[][] samplePoints = new float[count][];

    //     for (int i = 0; i < count; i++)
    //     {
    //         // clipPlayables[i] = AnimationClipPlayable.Create(graph, clips[i].Clip);
    //         // clipPlayables[i].SetApplyPlayableIK(_enableIKPass);
    //         // clipPlayables[i].SetApplyFootIK(_enableAnimFootIK);
    //         clipPlayables[i] = CreatePlayableFromClip(clips[i].Clip);

    //         graph.Connect(clipPlayables[i], 0, mixer, i);
    //         mixer.SetInputWeight(i, i == 0 ? 1f : 0f);
    //         samplePoints[i] = new float[] { clips[i].Position.x, clips[i].Position.y };
    //     }

    //     var interpolator = new PolarGradientBandInterpolator(samplePoints);
    //     return (mixer, clipPlayables, interpolator);
    // }

    // /// <summary>
    // /// Helper function which returns the playable for the clip using the graph, sets up ik stuff enabling too
    // /// </summary>
    // /// <returns></returns>
    // private AnimationClipPlayable CreatePlayableFromClip(AnimationClip clip)
    // {
    //     AnimationClipPlayable clipPlayable = AnimationClipPlayable.Create(graph, clip);
    //     clipPlayable.SetApplyPlayableIK(_enableIKPass);
    //     clipPlayable.SetApplyFootIK(_enableAnimFootIK);

    //     return clipPlayable;
    // }

    // public override void NetworkFixedUpdate()
    // {
    //     if (!Object.IsProxy)
    //     {
    //         AnimTime += Sandbox.FixedDeltaTime;

    //         UpdateAnimationData();
    //         UpdateAnimation();
    //     }
    // }
    // public override void NetworkRender()
    // {
    //     UpdateAnimation();
    // }


    // // private Vector2 movementStart;
    // private float stateStart;
    // // private Vector2 prevTargetMovement;
    // private float prevTargetState;
    // private float curveStartTime;
    // private bool curveActive;
    // public float SmoothDuration = 0.25f; // duration in seconds

    // private void UpdateAnimationData()
    // {
    //     OnSetValues?.Invoke();

    //     Vector2 currentMovement = new(MoveX, MoveY);
    //     // MovementSmooth = Vector2.Lerp(MovementSmooth, currentMovement, LerpSpeed * Sandbox.FixedDeltaTime);
    //     // StateValueSmooth = Mathf.Lerp(StateValueSmooth, StateValue, LerpSpeed * Sandbox.FixedDeltaTime);

    //     // New approach using move towards to always reach target (Lerp doesn't reach the exact value)
    //     float maxStep = LerpSpeed * Sandbox.FixedDeltaTime;

    //     MovementSmooth = Vector2.MoveTowards(MovementSmooth, currentMovement, maxStep);

    //     // StateValueSmooth = Mathf.MoveTowards(StateValueSmooth, StateValue, maxStep);

    //     CheckStartStateCurve();
    // }

    // private void CheckStartStateCurve()
    // {
    //     float targetState = StateValue;

    //     if (targetState != prevTargetState)
    //     {
    //         stateStart = StateValueSmooth;
    //         curveStartTime = AnimTime;
    //         curveActive = true;
    //         prevTargetState = targetState;
    //     }

    //     if (curveActive)
    //     {
    //         float elapsed = AnimTime - curveStartTime;
    //         float t = Mathf.Clamp01(elapsed / SmoothDuration);
    //         float curveT = SmoothCurve.Evaluate(t);

    //         StateValueSmooth = Mathf.Lerp(stateStart, targetState, curveT);

    //         if (t >= 1f) curveActive = false;
    //     }
    // }

    // private void UpdateAnimation()
    // {
    //     if (!graph.IsValid()) return;

    //     float crouchWeight = Mathf.Clamp01(1f - (StateValueSmooth * 2f));
    //     float walkWeight = Mathf.Clamp01(1f - Mathf.Abs(StateValueSmooth - 0.5f) * 2f);
    //     float runWeight = Mathf.Clamp01((StateValueSmooth - 0.5f) * 2f);

    //     locomotionMixer.SetInputWeight(0, crouchWeight);
    //     locomotionMixer.SetInputWeight(1, walkWeight);
    //     locomotionMixer.SetInputWeight(2, runWeight);

    //     AnimateBlendTree(crouchMixer, crouchClips, crouchInterpolator);
    //     AnimateBlendTree(walkMixer, walkClips, walkInterpolator);
    //     AnimateBlendTree(runMixer, runClips, runInterpolator);

    //     locomotionMixer.SetPropagateSetTime(true);
    //     locomotionMixer.SetTime(AnimTime);
    // }

    // private void AnimateBlendTree(AnimationMixerPlayable mixer, AnimationClipPlayable[] clips, PolarGradientBandInterpolator interpolator)
    // {
    //     if (!graph.IsValid()) return;

    //     mixer.SetPropagateSetTime(true);
    //     mixer.SetTime(AnimTime);

    //     float[] weights = interpolator.Interpolate(new float[] { MovementSmooth.x, MovementSmooth.y }, true);
    //     for (int i = 0; i < clips.Length; i++)
    //     {
    //         mixer.SetInputWeight(i, weights[i]);
    //     }

    //     graph.Evaluate();
    // }


    // public override void NetworkDestroy()
    // {
    //     if (graph.IsValid()) graph.Destroy();
    // }

    // void OnDestroy()
    // {
    //     if (graph.IsValid()) graph.Destroy();
    // }



    [Header("Graph Management")]
    [SerializeField] private RigBuilder _rigBuilder;
    [SerializeField] private Animator _targetAnimator;
    [SerializeField] private bool InvokeSyncLayers = true;

    [Header("Locomotion")]
    [SerializeField] private bool _enableAnimFootIK = true;
    [SerializeField] private bool _enableIKPass = false;
    public List<BlendClip> WalkBlendClips = new();
    public List<BlendClip> RunBlendClips = new();
    public List<BlendClip> CrouchBlendClips = new();
    public AnimationClip AirBorneClip;

    [Header("Upper Body")]
    public AnimationClip UpperBodyClip;
    public AvatarMask UpperBodyMask;

    [Header("Smoothing")]
    public float LerpSpeed = 10f;
    [SerializeField] private AnimationCurve SmoothCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public float SmoothDuration = 0.25f;

    [Networked, Smooth] public float MoveX { get; set; }
    [Networked, Smooth] public float MoveY { get; set; }
    [Networked, Smooth] public float StateValue { get; set; }
    [Networked, Smooth] public Vector2 MovementSmooth { get; set; }
    [Networked, Smooth] public float StateValueSmooth { get; set; }
    [Networked, Smooth] public float AnimTime { get; set; }

    private PlayableGraph graph;
    private AnimationMixerPlayable locomotionMixer;
    private AnimationMixerPlayable walkMixer;
    private AnimationMixerPlayable runMixer;
    private AnimationMixerPlayable crouchMixer;

    private AnimationClipPlayable[] walkClips;
    private AnimationClipPlayable[] runClips;
    private AnimationClipPlayable[] crouchClips;
    private AnimationClipPlayable airbornePlayable;
    private AnimationClipPlayable upperBodyPlayable;

    private PolarGradientBandInterpolator walkInterpolator;
    private PolarGradientBandInterpolator runInterpolator;
    private PolarGradientBandInterpolator crouchInterpolator;

    private float stateStart;
    private float prevTargetState;
    private float curveStartTime;
    private bool curveActive;

    [Networked] public double manualTime { get; set; }
    private double lastNetworkTime;

    public Action OnSetValues;

    void Start()
    {
        InitializeGraph();
        CreatePlayableGraph();
    }

    private void InitializeGraph()
    {
        _rigBuilder.Build();
        graph = _rigBuilder.graph;
        graph.SetTimeUpdateMode(DirectorUpdateMode.Manual);
    }

    private void CreatePlayableGraph()
    {
        if (_targetAnimator == null) return;

        (walkMixer, walkClips, walkInterpolator) = CreateBlendTree(WalkBlendClips);
        (runMixer, runClips, runInterpolator) = CreateBlendTree(RunBlendClips);
        (crouchMixer, crouchClips, crouchInterpolator) = CreateBlendTree(CrouchBlendClips);

        locomotionMixer = AnimationMixerPlayable.Create(graph, 4);
        graph.Connect(crouchMixer, 0, locomotionMixer, 0);
        graph.Connect(walkMixer, 0, locomotionMixer, 1);
        graph.Connect(runMixer, 0, locomotionMixer, 2);

        airbornePlayable = CreatePlayableFromClip(AirBorneClip);
        graph.Connect(airbornePlayable, 0, locomotionMixer, 3);

        var layerMixer = AnimationLayerMixerPlayable.Create(graph, 2);
        graph.Connect(locomotionMixer, 0, layerMixer, 0);

        upperBodyPlayable = CreatePlayableFromClip(UpperBodyClip);
        graph.Connect(upperBodyPlayable, 0, layerMixer, 1);
        layerMixer.SetLayerMaskFromAvatarMask(1, UpperBodyMask);
        layerMixer.SetInputWeight(0, 1f);
        layerMixer.SetInputWeight(1, 1f);

        var output = AnimationPlayableOutput.Create(graph, "Animation", _targetAnimator);
        output.SetSourcePlayable(layerMixer);

        graph.Play();
    }

    private (AnimationMixerPlayable, AnimationClipPlayable[], PolarGradientBandInterpolator)
        CreateBlendTree(List<BlendClip> clips)
    {
        int count = clips.Count;
        var mixer = AnimationMixerPlayable.Create(graph, count);
        var clipPlayables = new AnimationClipPlayable[count];
        float[][] samplePoints = new float[count][];

        for (int i = 0; i < count; i++)
        {
            clipPlayables[i] = CreatePlayableFromClip(clips[i].Clip);
            graph.Connect(clipPlayables[i], 0, mixer, i);
            mixer.SetInputWeight(i, i == 0 ? 1f : 0f);
            samplePoints[i] = new float[] { clips[i].Position.x, clips[i].Position.y };
        }

        var interpolator = new PolarGradientBandInterpolator(samplePoints);
        return (mixer, clipPlayables, interpolator);
    }

    private AnimationClipPlayable CreatePlayableFromClip(AnimationClip clip)
    {
        var playable = AnimationClipPlayable.Create(graph, clip);
        playable.SetApplyPlayableIK(_enableIKPass);
        playable.SetApplyFootIK(_enableAnimFootIK);
        return playable;
    }

    public override void NetworkFixedUpdate()
    {
        if (!Object.IsProxy)
        {
            AnimTime += Sandbox.FixedDeltaTime;

            UpdateAnimationData();
            UpdateAnimation();
        }
    }

    public override void NetworkRender()
    {
        UpdateAnimation();
    }

    private void UpdateAnimationData()
    {
        OnSetValues?.Invoke();

        Vector2 currentMovement = new(MoveX, MoveY);
        MovementSmooth = Vector2.MoveTowards(MovementSmooth, currentMovement, LerpSpeed * Sandbox.FixedDeltaTime);

        // Ease-in/out for StateValueSmooth
        float targetState = StateValue;
        if (targetState != prevTargetState)
        {
            stateStart = StateValueSmooth;
            curveStartTime = AnimTime;
            curveActive = true;
            prevTargetState = targetState;
        }

        if (curveActive)
        {
            float elapsed = AnimTime - curveStartTime;
            float t = Mathf.Clamp01(elapsed / SmoothDuration);
            StateValueSmooth = Mathf.Lerp(stateStart, targetState, SmoothCurve.Evaluate(t));
            if (t >= 1f) curveActive = false;
        }
    }

    private void UpdateAnimation()
    {
        if (!graph.IsValid()) return;

        // Subtree weights
        float crouchWeight = Mathf.Clamp01(1f - (StateValueSmooth * 2f));
        float walkWeight = Mathf.Clamp01(1f - Mathf.Abs(StateValueSmooth - 0.5f) * 2f);
        float runWeight = Mathf.Clamp01((StateValueSmooth - 0.5f) * 2f);
        float airborneWeight = 1f - (crouchWeight + walkWeight + runWeight);

        locomotionMixer.SetInputWeight(0, crouchWeight);
        locomotionMixer.SetInputWeight(1, walkWeight);
        locomotionMixer.SetInputWeight(2, runWeight);
        locomotionMixer.SetInputWeight(3, airborneWeight);

        // Compute weighted durations for each blend tree
        double crouchDur = ComputeWeightedDuration(crouchClips, crouchInterpolator, MovementSmooth);
        double walkDur = ComputeWeightedDuration(walkClips, walkInterpolator, MovementSmooth);
        double runDur = ComputeWeightedDuration(runClips, runInterpolator, MovementSmooth);
        double airborneDur = AirBorneClip.length;

        // Compute top-level weighted duration
        double totalWeight = crouchWeight + walkWeight + runWeight + airborneWeight;
        double topDuration =
            (crouchDur * crouchWeight +
             walkDur * walkWeight +
             runDur * runWeight +
             airborneDur * airborneWeight) / Math.Max(0.0001, totalWeight);

        // Advance manualTime
        double deltaTime = AnimTime - lastNetworkTime;
        lastNetworkTime = AnimTime;
        manualTime += deltaTime / topDuration;
        manualTime %= 1.0;
        

        // Apply time to all clips
        ApplyTimeToBlendTree(crouchMixer, crouchClips, crouchInterpolator, crouchWeight);
        ApplyTimeToBlendTree(walkMixer, walkClips, walkInterpolator, walkWeight);
        ApplyTimeToBlendTree(runMixer, runClips, runInterpolator, runWeight);
        airbornePlayable.SetTime(manualTime * AirBorneClip.length);

        graph.Evaluate();
    }

    private double ComputeWeightedDuration(AnimationClipPlayable[] clips, PolarGradientBandInterpolator interpolator, Vector2 input)
    {
        double weighted = 0.0;
        double totalWeight = 0.0;

        float[] weights = interpolator.Interpolate(new float[] { input.x, input.y }, true);
        for (int i = 0; i < clips.Length; i++)
        {
            if (clips[i].IsValid() && clips[i].GetAnimationClip() != null)
            {
                double len = clips[i].GetAnimationClip().length;
                weighted += len * weights[i];
                totalWeight += weights[i];
            }
        }

        if (totalWeight > 0.0001)
            weighted /= totalWeight;
        else
            weighted = 1.0;

        return weighted;
    }

    // private void ApplyTimeToBlendTree(AnimationClipPlayable[] clips, PolarGradientBandInterpolator interpolator, float subtreeWeight)
    // {
    //     float[] weights = interpolator.Interpolate(new float[] { MovementSmooth.x, MovementSmooth.y }, true);

    //     double totalWeight = 0.0;
    //     for (int i = 0; i < weights.Length; i++) totalWeight += weights[i];
    //     if (totalWeight < 0.0001) totalWeight = 1.0;

    //     for (int i = 0; i < clips.Length; i++)
    //     {
    //         if (!clips[i].IsValid() || clips[i].GetAnimationClip() == null) continue;
    //         double t = manualTime * clips[i].GetAnimationClip().length;
    //         clips[i].SetTime(t);
    //     }
    // }
    private void ApplyTimeToBlendTree(AnimationMixerPlayable mixer, AnimationClipPlayable[] clips, PolarGradientBandInterpolator interpolator, float subtreeWeight)
    {
        float[] weights = interpolator.Interpolate(new float[] { MovementSmooth.x, MovementSmooth.y }, true);

        double totalWeight = 0.0;
        for (int i = 0; i < weights.Length; i++) totalWeight += weights[i];
        if (totalWeight < 0.0001) totalWeight = 1.0;

        for (int i = 0; i < clips.Length; i++)
        {
            if (!clips[i].IsValid() || clips[i].GetAnimationClip() == null) continue;

            // Set time
            double t = manualTime * clips[i].GetAnimationClip().length;
            clips[i].SetTime(t);

            // Set weight multiplied by subtree weight (top-level mixer weight)
            // float finalWeight = (weights[i] / (float)totalWeight);
            mixer.SetInputWeight(i, weights[i]);
        }
    }

    public override void NetworkDestroy()
    {
        if (graph.IsValid()) graph.Destroy();
    }

    void OnDestroy()
    {
        if (graph.IsValid()) graph.Destroy();
    }

    // OLD Stuff to be removed Below
    #region
    // private void CreateGraphOld()
    // {
    //     if (_targetAnimator == null || WalkBlendClips == null || WalkBlendClips.Count == 0)
    //     {
    //         Debug.LogError("TickAnimator: Assign Animator and BlendClips before starting!");
    //         return;
    //     }

    //     int count = WalkBlendClips.Count;
    //     clips = new AnimationClip[count];
    //     float[][] samplePoints = new float[count][];

    //     for (int i = 0; i < count; i++)
    //     {
    //         clips[i] = WalkBlendClips[i].Clip;
    //         samplePoints[i] = new float[] { WalkBlendClips[i].Position.x, WalkBlendClips[i].Position.y };
    //     }

    //     interpolator = new PolarGradientBandInterpolator(samplePoints);

    //     // Base locomotion mixer
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

    //     // Layer mixer (2 layers)
    //     var layerMixer = AnimationLayerMixerPlayable.Create(graph, 2);

    //     // Upper body clip
    //     var upperBodyClipPlayable = AnimationClipPlayable.Create(graph, UpperBodyClip);
    //     graph.Connect(mixer, 0, layerMixer, 0);
    //     graph.Connect(upperBodyClipPlayable, 0, layerMixer, 1);

    //     // Apply avatar mask to layer 1
    //     layerMixer.SetLayerMaskFromAvatarMask(1, UpperBodyMask);

    //     layerMixer.SetInputWeight(0, 1f);
    //     layerMixer.SetInputWeight(1, 1f);

    //     var output = AnimationPlayableOutput.Create(graph, "Animation", _targetAnimator);
    //     output.SetSourcePlayable(layerMixer);

    //     graph.Play();
    // }
    // private void SetAnimationOld(double currentNetworkTime)
    // {
    //     if (!graph.IsValid()) return;

    //     // if (!TakeControl) networkTime += Time.deltaTime;

    //     float cMoveX = Mathf.Clamp(MoveX, -1f, 1f);
    //     float cMoveY = Mathf.Clamp(MoveY, -1f, 1f);

    //     float[] input = new float[] { cMoveX, cMoveY };
    //     float[] weights = interpolator.Interpolate(input, true);

    //     if (weights == null || weights.Length != clips.Length)
    //     {
    //         weights = new float[clips.Length];
    //         weights[0] = 1f;
    //     }

    //     // Compute weighted effective duration of the blend
    //     double weightedDuration = 0.0;
    //     double totalWeight = 0.0;
    //     for (int i = 0; i < clips.Length; i++)
    //     {
    //         if (clips[i] == null || clips[i].length < 0.0001) continue;
    //         weightedDuration += clips[i].length * weights[i];
    //         totalWeight += weights[i];
    //     }
    //     if (totalWeight > 0.0001)
    //         weightedDuration /= totalWeight;
    //     else
    //         weightedDuration = 1.0;

    //     // Advance manualTime based on network time delta and weighted duration
    //     double deltaTime = currentNetworkTime - lastNetworkTime;
    //     lastNetworkTime = currentNetworkTime;

    //     manualTime += (deltaTime / weightedDuration) * Speed;
    //     manualTime %= 1.0;

    //     // Apply manualTime to each clip
    //     for (int i = 0; i < clipPlayables.Length; i++)
    //     {
    //         var clip = clips[i];
    //         if (clip == null || clip.length < 0.0001) continue;

    //         double t = manualTime * clip.length;
    //         clipPlayables[i].SetTime(t);
    //         clipPlayables[i].SetApplyFootIK(_animStateFootIK);
    //         clipPlayables[i].SetApplyPlayableIK(_animIKPass);

    //         mixer.SetInputWeight(i, weights[i]);
    //     }

    //     if (InvokeSyncLayers) _rigBuilder.SyncLayers();
    //     graph.Evaluate(0f);
    // }
    #endregion
}