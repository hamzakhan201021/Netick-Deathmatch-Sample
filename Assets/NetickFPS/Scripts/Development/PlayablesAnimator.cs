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

    public RigBuilder CRigBuilder;

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
    void Start()
    {
        if (BlendClips == null || BlendClips.Count == 0)
        {
            Debug.LogError("Please assign some BlendClips before playing!");
            return;
        }

        CRigBuilder.Build();
        graph = CRigBuilder.graph;
        if (graph.IsValid()) graph.SetTimeUpdateMode(DirectorUpdateMode.Manual);
        else Debug.Log("Invalid graph");

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

    void Update()
    {
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
        if (!graph.IsValid()) return;

        double delta = Time.deltaTime;
        manualTime += delta * Speed;

        MoveX = Mathf.Clamp(MoveX, -1f, 1f);
        MoveY = Mathf.Clamp(MoveY, -1f, 1f);

        float[] input = new float[] { MoveX, MoveY };
        float[] weights = interpolator.Interpolate(input, true);
        if (weights == null || weights.Length != clips.Length)
        {
            weights = new float[clips.Length];
            weights[0] = 1f;
        }

        // Find dominant clip (highest weight)
        int dominantIndex = 0;
        float maxWeight = 0f;
        for (int i = 0; i < weights.Length; i++)
        {
            if (weights[i] > maxWeight)
            {
                maxWeight = weights[i];
                dominantIndex = i;
            }
        }

        double dominantLength = (clips[dominantIndex] != null && clips[dominantIndex].length > 0.0001)
            ? clips[dominantIndex].length
            : 1.0;

        // Advance each clip proportionally to dominant clip
        for (int i = 0; i < clipPlayables.Length; i++)
        {
            var clip = clips[i];
            if (clip == null || clip.length < 0.0001) continue;

            double normalizedTime = manualTime / dominantLength; // [0, ∞)
            normalizedTime %= 1.0; // loop

            double t = normalizedTime * clip.length; // scale to clip length
            clipPlayables[i].SetTime(t);
            clipPlayables[i].SetApplyFootIK(_animStateFootIK);
            clipPlayables[i].SetApplyPlayableIK(_animIKPass);
            mixer.SetInputWeight(i, weights[i]);
        }

        graph.Evaluate((float)delta);
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

    }
}