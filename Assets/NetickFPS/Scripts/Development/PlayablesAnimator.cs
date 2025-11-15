using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Animations.Rigging;
using UnityEngine.Playables;

[RequireComponent(typeof(Animator))]
public class PlayablesAnimator : MonoBehaviour
{
    [System.Serializable]
    public class BlendClip
    {
        public AnimationClip Clip;
        public Vector2 Position;
    }

    public enum CreateGraphMode
    {
        CreateNew,
        UseRigBuildersGraph,
    }

    [Header("Graph Management")]
    [SerializeField] private RigBuilder CRigBuilder;
    [SerializeField] private CreateGraphMode GraphCreationMode;
    [SerializeField] private string GraphName = "CustomAnimationGraph";
    [SerializeField] private bool InvokeSyncLayers = true;

    public List<BlendClip> BlendClips = new();

    [Range(-1f, 1f)] public float MoveX;
    [Range(-1f, 1f)] public float MoveY;
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
    public double networkTime;
    void Start()
    {
        if (BlendClips == null || BlendClips.Count == 0)
        {
            Debug.LogError("Please assign some BlendClips before playing!");
            return;
        }

        switch (GraphCreationMode)
        {
            case CreateGraphMode.CreateNew:
                graph = PlayableGraph.Create(GraphName);
                graph.SetTimeUpdateMode(DirectorUpdateMode.Manual);
                
                CRigBuilder?.Build(graph);

                break;
            case CreateGraphMode.UseRigBuildersGraph:
                CRigBuilder.Build();

                graph = CRigBuilder.graph;

                graph.SetTimeUpdateMode(DirectorUpdateMode.Manual);

                break;
        }

        // if (CreateOwnGraph)
        // {
        //     graph = PlayableGraph.Create(GraphName);
        //     graph.SetTimeUpdateMode(DirectorUpdateMode.Manual);

        //     CRigBuilder.Build(graph);
        // }
        // else
        // {
        //     CRigBuilder.Build();
        //     graph = CRigBuilder.graph;

        //     if (graph.IsValid()) graph.SetTimeUpdateMode(DirectorUpdateMode.Manual);
        //     else Debug.Log("Invalid graph");
        // }

        int count = BlendClips.Count;
        clips = new AnimationClip[count];
        float[][] samplePoints = new float[count][];

        for (int i = 0; i < count; i++)
        {
            clips[i] = BlendClips[i].Clip;
            samplePoints[i] = new float[] { BlendClips[i].Position.x, BlendClips[i].Position.y };
        }

        interpolator = new PolarGradientBandInterpolator(samplePoints);

        var animator = GetComponent<Animator>();

        // graph = PlayableGraph.Create();
        // graph.SetTimeUpdateMode(DirectorUpdateMode.Manual);

        var output = AnimationPlayableOutput.Create(graph, "Animation", animator);

        mixer = AnimationMixerPlayable.Create(graph, count);
        output.SetSourcePlayable(mixer);

        clipPlayables = new AnimationClipPlayable[count];

        for (int i = 0; i < count; i++)
        {
            var c = clips[i] != null ? clips[i] : new AnimationClip();
            clipPlayables[i] = AnimationClipPlayable.Create(graph, c);
            // clipPlayables[i].SetApplyFootIK(_footIK);
            clipPlayables[i].SetSpeed(1.0);
            clipPlayables[i].SetTime(0.0);
            graph.Connect(clipPlayables[i], 0, mixer, i);
            mixer.SetInputWeight(i, i == 0 ? 1f : 0f);
        }

        graph.Play();
        manualTime = 0.0;
    }




    double lastNetworkTime = 0.0f;
    double effectivePhaseSpeed = 0.0;

    void Update()
    {
        #region Testing / Old code
        // if (!graph.IsValid()) return;

        // double delta = Time.deltaTime;
        // manualTime += delta * Speed;
        // // normalizedTime %= 1.0;

        // // timeTrue += Time.deltaTime;
        // // timeTrue %= 1.0;

        // // graph.Evaluate((float)delta);

        // // mixer.SetTime(timeTrue);

        // // PlayableExtensions.SetTime(graph.GetRootPlayable(0), timeTrue);
        // // PlayableExtensions.SetSpeed(graph.GetRootPlayable(0), Speed);

        // MoveX = Mathf.Clamp(MoveX, -1f, 1f);
        // MoveY = Mathf.Clamp(MoveY, -1f, 1f);

        // float[] input = new float[] { MoveX, MoveY };
        // float[] weights = interpolator.Interpolate(input, true);
        // if (weights == null || weights.Length != clips.Length)
        // {
        //     weights = new float[clips.Length];
        //     weights[0] = 1f;
        // }

        // for (int i = 0; i < clipPlayables.Length; i++)
        // {
        //     var clip = clips[i];
        //     double length = (clip != null && clip.length > 0.0001) ? clip.length : 1.0;

        //     manualTime %= 1.0;

        //     double t = manualTime * length;
        //     clipPlayables[i].SetTime(t);
        //     clipPlayables[i].SetApplyFootIK(_animStateFootIK);
        //     clipPlayables[i].SetApplyPlayableIK(_animIKPass);
        //     mixer.SetInputWeight(i, weights[i]);
        // }




        // TODO use this for tick accurate animator system.
        // if (!graph.IsValid()) return;

        // double delta = Time.deltaTime;
        // manualTime += delta * Speed;

        // MoveX = Mathf.Clamp(MoveX, -1f, 1f);
        // MoveY = Mathf.Clamp(MoveY, -1f, 1f);

        // float[] input = new float[] { MoveX, MoveY };
        // float[] weights = interpolator.Interpolate(input, true);
        // if (weights == null || weights.Length != clips.Length)
        // {
        //     weights = new float[clips.Length];
        //     weights[0] = 1f;
        // }

        // // Find dominant clip (highest weight)
        // // int dominantIndex = 0;
        // // float maxWeight = 0f;
        // // for (int i = 0; i < weights.Length; i++)
        // // {
        // //     if (weights[i] > maxWeight)
        // //     {
        // //         maxWeight = weights[i];
        // //         dominantIndex = i;
        // //     }
        // // }

        // // double dominantLength = (clips[dominantIndex] != null && clips[dominantIndex].length > 0.0001)
        // //     ? clips[dominantIndex].length
        // //     : 1.0;
        // // Compute weighted average clip length (smoother than hard dominant)
        // // Determine current target duration based on weights
        // double targetLength = 0.0;
        // double totalWeight = 0.0;
        // for (int i = 0; i < clips.Length; i++)
        // {
        //     if (clips[i] == null || clips[i].length < 0.0001) continue;
        //     targetLength += clips[i].length * weights[i];
        //     totalWeight += weights[i];
        // }
        // if (totalWeight > 0.0001)
        //     targetLength /= totalWeight;
        // else
        //     targetLength = 1.0;

        // // Smoothly converge to new effective length (avoids jitter)
        // if (effectiveLength <= 0.0)
        //     effectiveLength = targetLength;
        // else
        //     effectiveLength = Mathf.Lerp((float)effectiveLength, (float)targetLength, 10f * Time.deltaTime);

        // // Advance global normalized time by real seconds / effective length
        // manualTime += delta * Speed;
        // double normalizedTime = manualTime % 1.0;

        // // Apply to each clip
        // for (int i = 0; i < clipPlayables.Length; i++)
        // {
        //     var clip = clips[i];
        //     if (clip == null || clip.length < 0.0001) continue;

        //     double t = normalizedTime * clip.length;
        //     clipPlayables[i].SetTime(t);
        //     clipPlayables[i].SetApplyFootIK(_animStateFootIK);
        //     clipPlayables[i].SetApplyPlayableIK(_animIKPass);
        //     mixer.SetInputWeight(i, weights[i]);
        // }

        // // graph.Evaluate((float)delta);
        // graph.Evaluate();
        #endregion

        // SEEMS TO BE THE BEST RESULT BUT WITHOUT PROPER TIME CONTROL YET
        #region BEST X No Time Control
        // if (!graph.IsValid()) return;

        // double delta = Time.deltaTime;
        // MoveX = Mathf.Clamp(MoveX, -1f, 1f);
        // MoveY = Mathf.Clamp(MoveY, -1f, 1f);

        // float[] input = new float[] { MoveX, MoveY };
        // float[] weights = interpolator.Interpolate(input, true);
        // if (weights == null || weights.Length != clips.Length)
        // {
        //     weights = new float[clips.Length];
        //     weights[0] = 1f;
        // }

        // double phaseSpeed = 0.0;
        // for (int i = 0; i < clips.Length; i++)
        // {
        //     double len = (clips[i] != null && clips[i].length > 0.0001) ? clips[i].length : 1.0;
        //     phaseSpeed += weights[i] / len;
        // }

        // manualTime += delta * phaseSpeed * Speed;
        // manualTime %= 1.0;

        // for (int i = 0; i < clipPlayables.Length; i++)
        // {
        //     var clip = clips[i];
        //     double len = (clip != null && clip.length > 0.0001) ? clip.length : 1.0;
        //     double t = manualTime * len;

        //     clipPlayables[i].SetTime(t);
        //     clipPlayables[i].SetApplyFootIK(_animStateFootIK);
        //     clipPlayables[i].SetApplyPlayableIK(_animIKPass);
        //     mixer.SetInputWeight(i, weights[i]);
        // }

        // graph.Evaluate();

        #endregion

        // if (!graph.IsValid()) return;

        // if (!TakeControl)
        // {
        //     networkTime += Reverse ? -Time.deltaTime : Time.deltaTime;
        // }

        // MoveX = Mathf.Clamp(MoveX, -1f, 1f);
        // MoveY = Mathf.Clamp(MoveY, -1f, 1f);

        // float[] input = new float[] { MoveX, MoveY };
        // float[] weights = interpolator.Interpolate(input, true);
        // if (weights == null || weights.Length != clips.Length)
        // {
        //     weights = new float[clips.Length];
        //     weights[0] = 1f;
        // }

        // // Compute target weighted phase speed like Unity
        // double targetPhaseSpeed = 0.0;
        // for (int i = 0; i < clips.Length; i++)
        // {
        //     double len = (clips[i] != null && clips[i].length > 0.0001) ? clips[i].length : 1.0;
        //     targetPhaseSpeed += weights[i] / len;
        // }

        // // Smoothly interpolate phase speed
        // // effectivePhaseSpeed = Mathf.Lerp((float)effectivePhaseSpeed, (float)targetPhaseSpeed, 10f * (float)(networkTime - lastNetworkTime));
        // effectivePhaseSpeed = (float)targetPhaseSpeed;
        // // TODO use the upper thing if its better

        // // Advance manualTime based on actual network time delta
        // double deltaTime = networkTime - lastNetworkTime;
        // manualTime += deltaTime * effectivePhaseSpeed * Speed;
        // manualTime %= 1.0;
        // lastNetworkTime = networkTime;

        // // Apply manualTime to each clip
        // for (int i = 0; i < clipPlayables.Length; i++)
        // {
        //     var clip = clips[i];
        //     double len = (clip != null && clip.length > 0.0001) ? clip.length : 1.0;
        //     double t = manualTime * len;

        //     clipPlayables[i].SetTime(t);
        //     clipPlayables[i].SetApplyFootIK(_animStateFootIK);
        //     clipPlayables[i].SetApplyPlayableIK(_animIKPass);
        //     mixer.SetInputWeight(i, weights[i]);
        // }

        // if (InvokeSyncLayers) CRigBuilder.SyncLayers();

        // graph.Evaluate(0f);



        // LESS COMPLEX TREE

        // if (!graph.IsValid()) return;

        // if (!TakeControl) networkTime += Time.deltaTime;

        // MoveX = Mathf.Clamp(MoveX, -1f, 1f);
        // MoveY = Mathf.Clamp(MoveY, -1f, 1f);

        // float[] input = new float[] { MoveX, MoveY };
        // float[] weights = interpolator.Interpolate(input, true);

        // if (weights == null || weights.Length != clips.Length)
        // {
        //     weights = new float[clips.Length];
        //     weights[0] = 1f;
        // }

        // // Determine deltaTime from network ticks
        // double deltaTime = networkTime - lastNetworkTime;
        // lastNetworkTime = networkTime;

        // // Advance each clip naturally
        // for (int i = 0; i < clipPlayables.Length; i++)
        // {
        //     var clip = clips[i];
        //     if (clip == null) continue;

        //     double currentTime = clipPlayables[i].GetTime() + deltaTime;
        //     if (currentTime > clip.length) currentTime %= clip.length;
        //     clipPlayables[i].SetTime(currentTime);

        //     clipPlayables[i].SetApplyFootIK(_animStateFootIK);
        //     clipPlayables[i].SetApplyPlayableIK(_animIKPass);

        //     // Apply blended weight
        //     mixer.SetInputWeight(i, weights[i]);
        // }

        // if (InvokeSyncLayers) CRigBuilder.SyncLayers();
        // graph.Evaluate(0f);



        // BR COMPLEX
        if (!graph.IsValid()) return;

        if (!TakeControl) networkTime += Time.deltaTime;

        MoveX = Mathf.Clamp(MoveX, -1f, 1f);
        MoveY = Mathf.Clamp(MoveY, -1f, 1f);

        float[] input = new float[] { MoveX, MoveY };
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
        double deltaTime = networkTime - lastNetworkTime;
        lastNetworkTime = networkTime;

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

        if (InvokeSyncLayers) CRigBuilder.SyncLayers();
        graph.Evaluate(0f);
    }

    void OnDisable()
    {
        if (graph.IsValid()) graph.Destroy();
    }

    class PolarGradientBandInterpolator
    {
        float[][] samples;
        public PolarGradientBandInterpolator(float[][] samplePoints) { samples = samplePoints; }

        float[] BasicChecks(float[] output)
        {
            if (output == null) return null;
            int d = output.Length;
            if (samples == null || samples.Length == 0) return null;
            if (samples[0].Length != d) return null;
            for (int i = 0; i < samples.Length; i++)
            {
                bool equal = true;
                for (int k = 0; k < d; k++)
                {
                    if (!Mathf.Approximately(samples[i][k], output[k])) { equal = false; break; }
                }
                if (equal)
                {
                    float[] w = new float[samples.Length];
                    w[i] = 1f;
                    return w;
                }
            }
            return null;
        }

        public float[] Interpolate(float[] output, bool normalize)
        {
            float[] weights = BasicChecks(output);
            if (weights != null) return weights;
            weights = new float[samples.Length];

            Vector3 outp;
            Vector3[] samp = new Vector3[samples.Length];
            if (output.Length == 2)
            {
                outp = new Vector3(output[0], output[1], 0);
                for (int i = 0; i < samples.Length; i++) samp[i] = new Vector3(samples[i][0], samples[i][1], 0);
            }
            else if (output.Length == 3)
            {
                outp = new Vector3(output[0], output[1], output[2]);
                for (int i = 0; i < samples.Length; i++) samp[i] = new Vector3(samples[i][0], samples[i][1], samples[i][2]);
            }
            else return null;

            for (int i = 0; i < samples.Length; i++)
            {
                bool outsideHull = false;
                float value = 1f;
                for (int j = 0; j < samples.Length; j++)
                {
                    if (i == j) continue;
                    Vector3 sampleI = samp[i];
                    Vector3 sampleJ = samp[j];

                    float iAngle, oAngle;
                    Vector3 outputProj;
                    float angleMultiplier = 2f;

                    if (sampleI == Vector3.zero)
                    {
                        iAngle = Vector3.Angle(outp, sampleJ) * Mathf.Deg2Rad;
                        oAngle = 0f;
                        outputProj = outp;
                        angleMultiplier = 1f;
                    }
                    else if (sampleJ == Vector3.zero)
                    {
                        iAngle = Vector3.Angle(outp, sampleI) * Mathf.Deg2Rad;
                        oAngle = iAngle;
                        outputProj = outp;
                        angleMultiplier = 1f;
                    }
                    else
                    {
                        iAngle = Vector3.Angle(sampleI, sampleJ) * Mathf.Deg2Rad;
                        if (iAngle > 0f)
                        {
                            if (outp == Vector3.zero)
                            {
                                oAngle = iAngle;
                                outputProj = outp;
                            }
                            else
                            {
                                Vector3 axis = Vector3.Cross(sampleI, sampleJ);
                                outputProj = Util.ProjectOntoPlane(outp, axis);
                                oAngle = Vector3.Angle(sampleI, outputProj) * Mathf.Deg2Rad;
                                if (iAngle < Mathf.PI * 0.99f)
                                {
                                    if (Vector3.Dot(Vector3.Cross(sampleI, outputProj), axis) < 0f)
                                        oAngle *= -1f;
                                }
                            }
                        }
                        else
                        {
                            outputProj = outp;
                            oAngle = 0f;
                        }
                    }

                    float magI = sampleI.magnitude;
                    float magJ = sampleJ.magnitude;
                    float magO = outputProj.magnitude;
                    float avgMag = (magI + magJ) * 0.5f;
                    if (avgMag == 0f) avgMag = 1f;
                    magI /= avgMag;
                    magJ /= avgMag;
                    magO /= avgMag;
                    Vector3 vecIJ = new Vector3(iAngle * angleMultiplier, magJ - magI, 0f);
                    Vector3 vecIO = new Vector3(oAngle * angleMultiplier, magO - magI, 0f);

                    float denom = vecIJ.sqrMagnitude;
                    if (denom == 0f) denom = Mathf.Epsilon;
                    float newValue = 1f - Vector3.Dot(vecIJ, vecIO) / denom;
                    if (newValue < 0f)
                    {
                        outsideHull = true;
                        break;
                    }
                    value = Mathf.Min(value, newValue);
                }
                if (!outsideHull) weights[i] = value;
            }

            if (normalize)
            {
                float summedWeight = 0f;
                for (int i = 0; i < samples.Length; i++) summedWeight += weights[i];
                if (summedWeight > 0f)
                {
                    for (int i = 0; i < samples.Length; i++) weights[i] /= summedWeight;
                }
            }
            return weights;
        }
    }

    static class Util
    {
        public static Vector3 ProjectOntoPlane(Vector3 v, Vector3 planeNormal)
        {
            if (planeNormal == Vector3.zero) return v;
            return v - Vector3.Project(v, planeNormal);
        }
    }

    // public AnimationClip IdleClip;
    // public AnimationClip WalkForwardClip;
    // public AnimationClip WalkBackwardClip;
    // public AnimationClip WalkLeftClip;
    // public AnimationClip WalkRightClip;

    // [Range(-1f, 1f)] public float MoveX;
    // [Range(-1f, 1f)] public float MoveY;
    // [Range(0.0f, 5f)] public float Speed = 1f;

    // PlayableGraph graph;
    // AnimationMixerPlayable mixer;
    // AnimationClipPlayable[] clipPlayables;
    // AnimationClip[] clips;
    // PolarGradientBandInterpolator interpolator;
    // double normalizedTime;

    // void Start()
    // {
    //     clips = new[]
    //     {
    //         IdleClip,
    //         WalkForwardClip,
    //         WalkBackwardClip,
    //         WalkLeftClip,
    //         WalkRightClip
    //     };

    //     float[][] samplePoints = new float[5][];
    //     samplePoints[0] = new float[] { 0f, 0f };
    //     samplePoints[1] = new float[] { 0f, 1f };
    //     samplePoints[2] = new float[] { 0f, -1f };
    //     samplePoints[3] = new float[] { -1f, 0f };
    //     samplePoints[4] = new float[] { 1f, 0f };

    //     interpolator = new PolarGradientBandInterpolator(samplePoints);

    //     var animator = GetComponent<Animator>();

    //     graph = PlayableGraph.Create();
    //     graph.SetTimeUpdateMode(DirectorUpdateMode.Manual);

    //     var output = AnimationPlayableOutput.Create(graph, "Animation", animator);

    //     mixer = AnimationMixerPlayable.Create(graph, clips.Length);
    //     output.SetSourcePlayable(mixer);

    //     clipPlayables = new AnimationClipPlayable[clips.Length];

    //     for (int i = 0; i < clips.Length; i++)
    //     {
    //         var c = clips[i] != null ? clips[i] : new AnimationClip();
    //         clipPlayables[i] = AnimationClipPlayable.Create(graph, c);
    //         clipPlayables[i].SetApplyFootIK(true);
    //         clipPlayables[i].SetSpeed(1.0);
    //         clipPlayables[i].SetTime(0.0);
    //         graph.Connect(clipPlayables[i], 0, mixer, i);
    //         mixer.SetInputWeight(i, i == 0 ? 1f : 0f);
    //     }

    //     graph.Play();
    //     normalizedTime = 0.0;
    // }

    // void Update()
    // {
    //     double delta = Time.deltaTime;
    //     normalizedTime += delta * Speed;
    //     normalizedTime %= 1.0;

    //     graph.Evaluate((float)delta);

    //     MoveX = Mathf.Clamp(MoveX, -1f, 1f);
    //     MoveY = Mathf.Clamp(MoveY, -1f, 1f);

    //     float[] input = new float[] { MoveX, MoveY };
    //     float[] weights = interpolator.Interpolate(input, true);
    //     if (weights == null || weights.Length != clips.Length)
    //     {
    //         weights = new float[clips.Length];
    //         weights[0] = 1f;
    //     }

    //     for (int i = 0; i < clipPlayables.Length; i++)
    //     {
    //         var clip = clips[i];
    //         double length = (clip != null && clip.length > 0.0001) ? clip.length : 1.0;
    //         double t = normalizedTime * length;
    //         clipPlayables[i].SetTime(t);
    //         mixer.SetInputWeight(i, weights[i]);
    //     }
    // }

    // void OnDisable()
    // {
    //     if (graph.IsValid()) graph.Destroy();
    // }

    // class PolarGradientBandInterpolator
    // {
    //     float[][] samples;

    //     public PolarGradientBandInterpolator(float[][] samplePoints)
    //     {
    //         samples = samplePoints;
    //     }

    //     float[] BasicChecks(float[] output)
    //     {
    //         if (output == null) return null;
    //         int d = output.Length;
    //         if (samples == null || samples.Length == 0) return null;
    //         if (samples[0].Length != d) return null;
    //         for (int i = 0; i < samples.Length; i++)
    //         {
    //             bool equal = true;
    //             for (int k = 0; k < d; k++)
    //             {
    //                 if (!Mathf.Approximately(samples[i][k], output[k])) { equal = false; break; }
    //             }
    //             if (equal)
    //             {
    //                 float[] w = new float[samples.Length];
    //                 w[i] = 1f;
    //                 return w;
    //             }
    //         }
    //         return null;
    //     }

    //     public float[] Interpolate(float[] output, bool normalize)
    //     {
    //         float[] weights = BasicChecks(output);
    //         if (weights != null) return weights;
    //         weights = new float[samples.Length];

    //         Vector3 outp;
    //         Vector3[] samp = new Vector3[samples.Length];
    //         if (output.Length == 2)
    //         {
    //             outp = new Vector3(output[0], output[1], 0);
    //             for (int i = 0; i < samples.Length; i++) samp[i] = new Vector3(samples[i][0], samples[i][1], 0);
    //         }
    //         else if (output.Length == 3)
    //         {
    //             outp = new Vector3(output[0], output[1], output[2]);
    //             for (int i = 0; i < samples.Length; i++) samp[i] = new Vector3(samples[i][0], samples[i][1], samples[i][2]);
    //         }
    //         else return null;

    //         for (int i = 0; i < samples.Length; i++)
    //         {
    //             bool outsideHull = false;
    //             float value = 1f;
    //             for (int j = 0; j < samples.Length; j++)
    //             {
    //                 if (i == j) continue;

    //                 Vector3 sampleI = samp[i];
    //                 Vector3 sampleJ = samp[j];

    //                 float iAngle, oAngle;
    //                 Vector3 outputProj;
    //                 float angleMultiplier = 2f;

    //                 if (sampleI == Vector3.zero)
    //                 {
    //                     iAngle = Vector3.Angle(outp, sampleJ) * Mathf.Deg2Rad;
    //                     oAngle = 0f;
    //                     outputProj = outp;
    //                     angleMultiplier = 1f;
    //                 }
    //                 else if (sampleJ == Vector3.zero)
    //                 {
    //                     iAngle = Vector3.Angle(outp, sampleI) * Mathf.Deg2Rad;
    //                     oAngle = iAngle;
    //                     outputProj = outp;
    //                     angleMultiplier = 1f;
    //                 }
    //                 else
    //                 {
    //                     iAngle = Vector3.Angle(sampleI, sampleJ) * Mathf.Deg2Rad;
    //                     if (iAngle > 0f)
    //                     {
    //                         if (outp == Vector3.zero)
    //                         {
    //                             oAngle = iAngle;
    //                             outputProj = outp;
    //                         }
    //                         else
    //                         {
    //                             Vector3 axis = Vector3.Cross(sampleI, sampleJ);
    //                             outputProj = Util.ProjectOntoPlane(outp, axis);
    //                             oAngle = Vector3.Angle(sampleI, outputProj) * Mathf.Deg2Rad;
    //                             if (iAngle < Mathf.PI * 0.99f)
    //                             {
    //                                 if (Vector3.Dot(Vector3.Cross(sampleI, outputProj), axis) < 0f)
    //                                 {
    //                                     oAngle *= -1f;
    //                                 }
    //                             }
    //                         }
    //                     }
    //                     else
    //                     {
    //                         outputProj = outp;
    //                         oAngle = 0f;
    //                     }
    //                 }

    //                 float magI = sampleI.magnitude;
    //                 float magJ = sampleJ.magnitude;
    //                 float magO = outputProj.magnitude;
    //                 float avgMag = (magI + magJ) * 0.5f;
    //                 if (avgMag == 0f) avgMag = 1f;
    //                 magI /= avgMag;
    //                 magJ /= avgMag;
    //                 magO /= avgMag;
    //                 Vector3 vecIJ = new Vector3(iAngle * angleMultiplier, magJ - magI, 0f);
    //                 Vector3 vecIO = new Vector3(oAngle * angleMultiplier, magO - magI, 0f);

    //                 float denom = vecIJ.sqrMagnitude;
    //                 if (denom == 0f) denom = Mathf.Epsilon;
    //                 float newValue = 1f - Vector3.Dot(vecIJ, vecIO) / denom;

    //                 if (newValue < 0f)
    //                 {
    //                     outsideHull = true;
    //                     break;
    //                 }
    //                 value = Mathf.Min(value, newValue);
    //             }
    //             if (!outsideHull) weights[i] = value;
    //         }

    //         if (normalize)
    //         {
    //             float summedWeight = 0f;
    //             for (int i = 0; i < samples.Length; i++) summedWeight += weights[i];
    //             if (summedWeight > 0f)
    //             {
    //                 for (int i = 0; i < samples.Length; i++) weights[i] /= summedWeight;
    //             }
    //         }

    //         return weights;
    //     }
    // }

    // static class Util
    // {
    //     public static Vector3 ProjectOntoPlane(Vector3 v, Vector3 planeNormal)
    //     {
    //         if (planeNormal == Vector3.zero) return v;
    //         return v - Vector3.Project(v, planeNormal);
    //     }
    // }




    public void PlayFootSound()
    {
        // Just here to prevent errors.

    }
}