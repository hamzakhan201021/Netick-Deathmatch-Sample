using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Animations.Rigging;
using UnityEngine.Playables;

[DefaultExecutionOrder(1000)]
public class CustomRigUpdater : MonoBehaviour
{
    // [SerializeField] Animator animator;
    // [SerializeField] RigBuilder rigBuilder;
    // [SerializeField] AnimationClip baseClip;

    // PlayableGraph graph;
    // AnimationPlayableOutput output;
    // AnimationLayerMixerPlayable layerMixer;
    // AnimationClipPlayable clipPlayable;

    // void Awake()
    // {
    //     //  var graph = new AnimancerGraph(_RigBuilder.graph);
    //     rigBuilder.Build(); // creates graph internally
    //     graph = rigBuilder.graph;

    //     if (graph.IsValid())
    //     {
    //         Debug.Log("Valid");
    //         graph.SetTimeUpdateMode(DirectorUpdateMode.Manual);
    //     }

    //     var clipPlayable = AnimationClipPlayable.Create(graph, baseClip);
    //     output = AnimationPlayableOutput.Create(graph, "CustomAnim", animator);
    //     output.SetSourcePlayable(clipPlayable);
    // }

    // void Update()
    // {
    //     graph.Evaluate(Time.deltaTime);
    // }





    // WORKING 

    // [SerializeField] private Animator animator;
    // [SerializeField] private RigBuilder rigBuilder;
    // [SerializeField] private AnimationClip clipA;
    // [SerializeField] private AnimationClip clipB;
    // [SerializeField] private AnimationClip clipC;

    // [Range(0f, 1f)]
    // [SerializeField] private float blend = 0f; // 0 = clipA, 0.5 = clipB, 1 = clipC

    // private PlayableGraph graph;
    // private AnimationPlayableOutput output;
    // private AnimationMixerPlayable mixer;
    // private AnimationClipPlayable clipPlayableA;
    // private AnimationClipPlayable clipPlayableB;
    // private AnimationClipPlayable clipPlayableC;

    // void Awake()
    // {
    //     rigBuilder.Build(); 
    //     graph = rigBuilder.graph;

    //     if (!graph.IsValid())
    //     {
    //         Debug.LogError("RigBuilder graph invalid!");
    //         return;
    //     }

    //     graph.SetTimeUpdateMode(DirectorUpdateMode.Manual);

    //     // Create the mixer with 3 inputs
    //     mixer = AnimationMixerPlayable.Create(graph, 3);

    //     // Create playable clips
    //     clipPlayableA = AnimationClipPlayable.Create(graph, clipA);
    //     clipPlayableB = AnimationClipPlayable.Create(graph, clipB);
    //     clipPlayableC = AnimationClipPlayable.Create(graph, clipC);

    //     // Connect clips to mixer
    //     graph.Connect(clipPlayableA, 0, mixer, 0);
    //     graph.Connect(clipPlayableB, 0, mixer, 1);
    //     graph.Connect(clipPlayableC, 0, mixer, 2);

    //     // Output
    //     output = AnimationPlayableOutput.Create(graph, "CustomAnim", animator);
    //     output.SetSourcePlayable(mixer);
    // }

    // void Update()
    // {
    //     // Blend logic: simple 3-way blend based on 'blend' float
    //     float wA, wB, wC;

    //     if (blend <= 0.5f)
    //     {
    //         wA = 1f - blend * 2f; // 1 -> 0 as blend 0 -> 0.5
    //         wB = blend * 2f;      // 0 -> 1 as blend 0 -> 0.5
    //         wC = 0f;
    //     }
    //     else
    //     {
    //         wA = 0f;
    //         wB = 1f - (blend - 0.5f) * 2f; // 1 -> 0 as blend 0.5 -> 1
    //         wC = (blend - 0.5f) * 2f;      // 0 -> 1 as blend 0.5 -> 1
    //     }

    //     mixer.SetInputWeight(0, wA);
    //     mixer.SetInputWeight(1, wB);
    //     mixer.SetInputWeight(2, wC);

    //     // Evaluate the rigBuilder + mixer manually
    //     graph.Evaluate(Time.deltaTime);
    // }




    // [SerializeField] Animator animator;
    // [SerializeField] RigBuilder rigBuilder;
    // [SerializeField] AnimationClip baseClip;

    // [Range(0f, 10f)]
    // public float clipTime = 0f; // Time in seconds to control manually

    // PlayableGraph graph;
    // AnimationPlayableOutput output;
    // AnimationClipPlayable clipPlayable;

    // void Awake()
    // {
    //     rigBuilder.Build();
    //     graph = rigBuilder.graph;

    //     if (graph.IsValid())
    //     {
    //         graph.SetTimeUpdateMode(DirectorUpdateMode.Manual);
    //     }

    //     clipPlayable = AnimationClipPlayable.Create(graph, baseClip);
    //     output = AnimationPlayableOutput.Create(graph, "CustomAnim", animator);
    //     output.SetSourcePlayable(clipPlayable);
    // }

    // void Update()
    // {
    //     if (clipPlayable.IsValid())
    //     {
    //         // Manually set the clip time based on the inspector value
    //         clipPlayable.SetTime(clipTime);
    //     }

    //     if (graph.IsValid())
    //         graph.Evaluate(Time.deltaTime);
    // }










    [SerializeField] Animator animator;
    [SerializeField] RigBuilder rigBuilder;
    [SerializeField] AnimationClip clipA;
    [SerializeField] AnimationClip clipB;
    [SerializeField] AnimationClip clipC;

    [Range(0f, 1f)] public float blendAB = 0f; // Blend A-B
    [Range(0f, 1f)] public float blendBC = 0f; // Blend B-C
    public bool AutoUpdateTime = true;
    public float clipTime = 0f; // Manual time for all clips

    PlayableGraph graph;
    AnimationPlayableOutput output;
    AnimationClipPlayable[] clips;
    AnimationMixerPlayable mixer;

    void Awake()
    {
        rigBuilder.Build();
        graph = rigBuilder.graph;
        if (graph.IsValid())
            graph.SetTimeUpdateMode(DirectorUpdateMode.Manual);

        // Create clip playables
        clips = new AnimationClipPlayable[3];
        clips[0] = AnimationClipPlayable.Create(graph, clipA);
        clips[1] = AnimationClipPlayable.Create(graph, clipB);
        clips[2] = AnimationClipPlayable.Create(graph, clipC);

        // Create mixer
        mixer = AnimationMixerPlayable.Create(graph, 3);

        // Connect clips to mixer
        for (int i = 0; i < 3; i++)
        {
            graph.Connect(clips[i], 0, mixer, i);
            mixer.SetInputWeight(i, i == 0 ? 1f : 0f);
        }

        // Create output
        output = AnimationPlayableOutput.Create(graph, "CustomAnim", animator);
        output.SetSourcePlayable(mixer);
    }

    void Update()
    {
        if (AutoUpdateTime)
        {
            clipTime += Time.deltaTime;
        }

        UpdateAnimationWithTime(clipTime);
    }

    void UpdateAnimationWithTime(float time)
    {
        // Manually set each clip's time
        for (int i = 0; i < 3; i++)
        {
            if (clips[i].IsValid())
            {
                clips[i].SetTime(time % clips[i].GetAnimationClip().length);
            }
        }

        // Blend logic
        mixer.SetInputWeight(0, 1f - blendAB);
        mixer.SetInputWeight(1, blendAB * (1f - blendBC));
        mixer.SetInputWeight(2, blendBC);

        // Evaluate graph
        if (graph.IsValid())
            graph.Evaluate();
    }













    // void Start()
    // {
    //     graph = PlayableGraph.Create("CharacterGraph");
    //     graph.SetTimeUpdateMode(DirectorUpdateMode.Manual);

    //     // new AnimancerGraph(graph);

    //     output = AnimationPlayableOutput.Create(graph, "Output", animator);

    //     // 1️⃣ Create your base animation (without AnimatorController)
    //     clipPlayable = AnimationClipPlayable.Create(graph, baseClip);
    //     clipPlayable.SetApplyFootIK(true);
    //     clipPlayable.SetApplyPlayableIK(true);

    //     // 2️⃣ Rig layer mixer
    //     layerMixer = AnimationLayerMixerPlayable.Create(graph, 1);
    //     graph.Connect(clipPlayable, 0, layerMixer, 0);
    //     layerMixer.SetInputWeight(0, 1f);

    //     output.SetSourcePlayable(layerMixer);

    //     // 3️⃣ Attach the RigBuilder to the same graph
    //     rigBuilder.Build(graph);

    //     // 4️⃣ Start the graph
    //     graph.Play();
    // }

    // void LateUpdate()
    // {
    //     if (!graph.IsValid()) return;

    //     // 5️⃣ Push transforms to the rig constraints
    //     rigBuilder.SyncLayers();

    //     // Advance your animation manually
    //     graph.Evaluate(Time.deltaTime);
    // }

    void OnDestroy()
    {
        if (graph.IsValid())
            graph.Destroy();
    }
}
