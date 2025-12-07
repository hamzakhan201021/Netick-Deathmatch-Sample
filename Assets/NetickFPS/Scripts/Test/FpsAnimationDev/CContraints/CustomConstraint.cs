using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Animations.Rigging;

[DisallowMultipleComponent]
public class PositionOnlyConstraint : RigConstraint<
    PositionOnlyConstraintJob,
    PositionOnlyConstraintData,
    PositionOnlyConstraintJobBinder
>
{ }

[System.Serializable]
public struct PositionOnlyConstraintData : IAnimationJobData
{
    public Transform constrainedObject;
    [SyncSceneToStream] public Transform sourcePositionObject;
    [SyncSceneToStream] public Transform sourceRotationObject;

    public bool IsValid() =>
        constrainedObject != null &&
        sourcePositionObject != null &&
        sourceRotationObject != null;

    public void SetDefaultValues()
    {
        constrainedObject = null;
        sourcePositionObject = null;
        sourceRotationObject = null;
    }
}

public struct PositionOnlyConstraintJob : IWeightedAnimationJob
{
    public ReadWriteTransformHandle constrainedHandle;
    public ReadOnlyTransformHandle sourcePositionHandle;
    public ReadOnlyTransformHandle sourceRotationHandle;
    public FloatProperty jobWeight;

    FloatProperty IWeightedAnimationJob.jobWeight { get => jobWeight; set => jobWeight = value; }

    public void ProcessRootMotion(AnimationStream stream) { }

    public void ProcessAnimation(AnimationStream stream)
    {
        float w = jobWeight.Get(stream);
        if (w <= 0f) return;

        Vector3 targetPos = sourcePositionHandle.GetPosition(stream);
        constrainedHandle.SetPosition(stream, Vector3.Lerp(constrainedHandle.GetPosition(stream), targetPos, w));

        Quaternion targetRot = sourceRotationHandle.GetRotation(stream);
        constrainedHandle.SetRotation(stream, targetRot);
    }
}

public class PositionOnlyConstraintJobBinder : AnimationJobBinder<PositionOnlyConstraintJob, PositionOnlyConstraintData>
{
    public override PositionOnlyConstraintJob Create(Animator animator, ref PositionOnlyConstraintData data, Component component)
    {
        var job = new PositionOnlyConstraintJob
        {
            constrainedHandle = ReadWriteTransformHandle.Bind(animator, data.constrainedObject),
            sourcePositionHandle = ReadOnlyTransformHandle.Bind(animator, data.sourcePositionObject),
            sourceRotationHandle = ReadOnlyTransformHandle.Bind(animator, data.sourceRotationObject)
        };

        return job;
    }

    public override void Destroy(PositionOnlyConstraintJob job) { }

    public override void Update(PositionOnlyConstraintJob job, ref PositionOnlyConstraintData data) { }
}
