using UnityEngine;

public class FpsAnimationIK : MonoBehaviour
{

    #region OLD version, to be removed
    // [SerializeField] private Transform _leftHandGrip;
    // [SerializeField] private Transform _rightHandGrip;
    // [SerializeField] public HandIKTargets _leftHandT;
    // [SerializeField] public HandIKTargets _rightHandT;
    // [SerializeField] private bool _useFinger = true;

    // [Header("Grip Position Offsets")]
    // [SerializeField] private Vector3 _lGripOffset = Vector3.zero;
    // [SerializeField] private Vector3 _rGripOffset = Vector3.zero;
    // [SerializeField] private float _lWeight = 1;
    // [SerializeField] private float _rWeight = 1;

    // private Animator _animator;

    // [System.Serializable]
    // public class HandIKTargets
    // {
    //     [Header("Hand")]
    //     public Transform HandIK;
    //     [Header("Thumb")]
    //     public Transform ThumbProximal;
    //     public Transform ThumbIntermediate;
    //     public Transform ThumbDistal;
    //     [Header("Index")]
    //     public Transform IndexProximal;
    //     public Transform IndexIntermediate;
    //     public Transform IndexDistal;
    //     [Header("Middle")]
    //     public Transform MiddleProximal;
    //     public Transform MiddleIntermediate;
    //     public Transform MiddleDistal;
    //     [Header("Ring")]
    //     public Transform RingProximal;
    //     public Transform RingIntermediate;
    //     public Transform RingDistal;
    //     [Header("Pinky")]
    //     public Transform LittleProximal;
    //     public Transform LittleIntermediate;
    //     public Transform LittleDistal;
    // }


    // void Awake()
    // {
    //     _animator = GetComponent<Animator>();
    // }

    // void OnAnimatorIK(int layerIndex)
    // {
    //     // Set Hand IK
    //     SetHandIK();

    //     // Set Finger bones.
    //     SetFingerBones();
    // }

    // private void SetHandIK()
    // {
    //     // Set IK Weights
    //     _animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, _lWeight);
    //     _animator.SetIKPositionWeight(AvatarIKGoal.RightHand, _rWeight);

    //     _animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, _lWeight);
    //     _animator.SetIKRotationWeight(AvatarIKGoal.RightHand, _rWeight);

    //     // Set IK positions
    //     _animator.SetIKPosition(AvatarIKGoal.LeftHand, _leftHandT.HandIK.position);
    //     _animator.SetIKPosition(AvatarIKGoal.RightHand, _rightHandT.HandIK.position);

    //     // _animator.SetIKHintPosition(AvatarIKHint.LeftElbow, _weaponLGripHint.position);
    //     // _animator.SetIKHintPosition(AvatarIKHint.RightElbow, _weaponRGripHint.position);

    //     // Set IK rotations
    //     _animator.SetIKRotation(AvatarIKGoal.LeftHand, _leftHandT.HandIK.rotation * Quaternion.Euler(_lGripOffset));
    //     _animator.SetIKRotation(AvatarIKGoal.RightHand, _rightHandT.HandIK.rotation * Quaternion.Euler(_rGripOffset));
    // }

    // private void SetFingerBones()
    // {
    //     if (!_useFinger) return;

    //     _animator.SetBoneLocalRotation(HumanBodyBones.LeftThumbProximal, _leftHandT.ThumbProximal.localRotation);
    //     _animator.SetBoneLocalRotation(HumanBodyBones.LeftThumbIntermediate, _leftHandT.ThumbIntermediate.localRotation);
    //     _animator.SetBoneLocalRotation(HumanBodyBones.LeftThumbDistal, _leftHandT.ThumbDistal.localRotation);

    //     _animator.SetBoneLocalRotation(HumanBodyBones.LeftIndexProximal, _leftHandT.IndexProximal.localRotation);
    //     _animator.SetBoneLocalRotation(HumanBodyBones.LeftIndexIntermediate, _leftHandT.IndexIntermediate.localRotation);
    //     _animator.SetBoneLocalRotation(HumanBodyBones.LeftIndexDistal, _leftHandT.IndexDistal.localRotation);

    //     _animator.SetBoneLocalRotation(HumanBodyBones.LeftMiddleProximal, _leftHandT.MiddleProximal.localRotation);
    //     _animator.SetBoneLocalRotation(HumanBodyBones.LeftMiddleIntermediate, _leftHandT.MiddleIntermediate.localRotation);
    //     _animator.SetBoneLocalRotation(HumanBodyBones.LeftMiddleDistal, _leftHandT.MiddleDistal.localRotation);

    //     _animator.SetBoneLocalRotation(HumanBodyBones.LeftRingProximal, _leftHandT.RingProximal.localRotation);
    //     _animator.SetBoneLocalRotation(HumanBodyBones.LeftRingIntermediate, _leftHandT.RingIntermediate.localRotation);
    //     _animator.SetBoneLocalRotation(HumanBodyBones.LeftRingDistal, _leftHandT.RingDistal.localRotation);

    //     _animator.SetBoneLocalRotation(HumanBodyBones.LeftLittleProximal, _leftHandT.LittleProximal.localRotation);
    //     _animator.SetBoneLocalRotation(HumanBodyBones.LeftLittleIntermediate, _leftHandT.LittleIntermediate.localRotation);
    //     _animator.SetBoneLocalRotation(HumanBodyBones.LeftLittleDistal, _leftHandT.LittleDistal.localRotation);

    //     _animator.SetBoneLocalRotation(HumanBodyBones.RightThumbProximal, _rightHandT.ThumbProximal.localRotation);
    //     _animator.SetBoneLocalRotation(HumanBodyBones.RightThumbIntermediate, _rightHandT.ThumbIntermediate.localRotation);
    //     _animator.SetBoneLocalRotation(HumanBodyBones.RightThumbDistal, _rightHandT.ThumbDistal.localRotation);

    //     _animator.SetBoneLocalRotation(HumanBodyBones.RightIndexProximal, _rightHandT.IndexProximal.localRotation);
    //     _animator.SetBoneLocalRotation(HumanBodyBones.RightIndexIntermediate, _rightHandT.IndexIntermediate.localRotation);
    //     _animator.SetBoneLocalRotation(HumanBodyBones.RightIndexDistal, _rightHandT.IndexDistal.localRotation);

    //     _animator.SetBoneLocalRotation(HumanBodyBones.RightMiddleProximal, _rightHandT.MiddleProximal.localRotation);
    //     _animator.SetBoneLocalRotation(HumanBodyBones.RightMiddleIntermediate, _rightHandT.MiddleIntermediate.localRotation);
    //     _animator.SetBoneLocalRotation(HumanBodyBones.RightMiddleDistal, _rightHandT.MiddleDistal.localRotation);

    //     _animator.SetBoneLocalRotation(HumanBodyBones.RightRingProximal, _rightHandT.RingProximal.localRotation);
    //     _animator.SetBoneLocalRotation(HumanBodyBones.RightRingIntermediate, _rightHandT.RingIntermediate.localRotation);
    //     _animator.SetBoneLocalRotation(HumanBodyBones.RightRingDistal, _rightHandT.RingDistal.localRotation);

    //     _animator.SetBoneLocalRotation(HumanBodyBones.RightLittleProximal, _rightHandT.LittleProximal.localRotation);
    //     _animator.SetBoneLocalRotation(HumanBodyBones.RightLittleIntermediate, _rightHandT.LittleIntermediate.localRotation);
    //     _animator.SetBoneLocalRotation(HumanBodyBones.RightLittleDistal, _rightHandT.LittleDistal.localRotation);
    // }
    #endregion

    [System.Serializable]
    public class FingerOffset
    {
        public Vector3 ThumbProximal;
        public Vector3 ThumbIntermediate;
        public Vector3 ThumbDistal;

        public Vector3 IndexProximal;
        public Vector3 IndexIntermediate;
        public Vector3 IndexDistal;

        public Vector3 MiddleProximal;
        public Vector3 MiddleIntermediate;
        public Vector3 MiddleDistal;

        public Vector3 RingProximal;
        public Vector3 RingIntermediate;
        public Vector3 RingDistal;

        public Vector3 LittleProximal;
        public Vector3 LittleIntermediate;
        public Vector3 LittleDistal;
    }

    [System.Serializable]
    public class HandIKTargets
    {
        public Transform HandIK;
        public Transform ThumbProximal;
        public Transform ThumbIntermediate;
        public Transform ThumbDistal;
        public Transform IndexProximal;
        public Transform IndexIntermediate;
        public Transform IndexDistal;
        public Transform MiddleProximal;
        public Transform MiddleIntermediate;
        public Transform MiddleDistal;
        public Transform RingProximal;
        public Transform RingIntermediate;
        public Transform RingDistal;
        public Transform LittleProximal;
        public Transform LittleIntermediate;
        public Transform LittleDistal;
    }



    [Header("Character Hand Bones")]
    [SerializeField] private HandIKTargets _leftHand;
    [SerializeField] private HandIKTargets _rightHand;

    [Header("Targets")]
    [SerializeField] public HandIKTargets _leftHandT;
    [SerializeField] public HandIKTargets _rightHandT;

    [SerializeField] private bool _useFinger = true;

    [Header("Hand IK Offsets")]
    [SerializeField] private Vector3 _lHandPosOffset = Vector3.zero;
    [SerializeField] private Vector3 _rHandPosOffset = Vector3.zero;
    [SerializeField] private Vector3 _lHandRotOffset = Vector3.zero;
    [SerializeField] private Vector3 _rHandRotOffset = Vector3.zero;

    [Header("Finger Rotation Offsets")]
    [SerializeField] public FingerOffset _leftOffsets;
    [SerializeField] public FingerOffset _rightOffsets;

    private Animator _animator;



    void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    void OnAnimatorIK(int layerIndex)
    {
        SetHandIK();
        // SetFingerBones();
    }

    void SetHandIK()
    {
        _animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1f);
        _animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1f);

        _animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1f);
        _animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1f);

        _animator.SetIKPosition(AvatarIKGoal.LeftHand, _leftHandT.HandIK.position + _lHandPosOffset);
        _animator.SetIKPosition(AvatarIKGoal.RightHand, _rightHandT.HandIK.position + _rHandPosOffset);

        _animator.SetIKRotation(AvatarIKGoal.LeftHand, _leftHandT.HandIK.rotation * Quaternion.Euler(_lHandRotOffset));
        _animator.SetIKRotation(AvatarIKGoal.RightHand, _rightHandT.HandIK.rotation * Quaternion.Euler(_rHandRotOffset));
    }

    void LateUpdate()
    {
        UpdateFingers();
    }

    private void UpdateFingers()
    {
        if (!_useFinger) return;

        _rightHand.IndexProximal.rotation = _rightHandT.IndexProximal.rotation * Quaternion.Euler(_rightOffsets.IndexProximal);
    }

    #region Animator SetBoneLocalRotation Method, doesn't produce retargetable result

    // void SetFingerBones()
    // {
    //     if (!_useFinger) return;

    //     // Quaternion rotationL = _animator.GetBoneTransform(HumanBodyBones.RightIndexProximal).localRotation;
    //     // rotationL = rotationL * _rightHandT.IndexProximal.localRotation;
    //     // _animator.SetBoneLocalRotation(HumanBodyBones.RightIndexProximal, rotationL);

    //     // ApplyFinger(_leftHandT, _leftOffsets, true);
    //     // ApplyFinger(_rightHandT, _rightOffsets, false);
    // }



    // void ApplyFinger(HandIKTargets h, FingerOffset o, bool isLeft)
    // {
    //     var side = isLeft ? HumanBodyBones.LeftThumbProximal : HumanBodyBones.RightThumbProximal;
    //     var prefix = isLeft ? HumanBodyBones.LeftThumbProximal : HumanBodyBones.RightThumbProximal;

    //     _animator.SetBoneLocalRotation(isLeft ? HumanBodyBones.LeftThumbProximal : HumanBodyBones.RightThumbProximal,
    //         h.ThumbProximal.localRotation * Quaternion.Euler(o.ThumbProximal));
    //     _animator.SetBoneLocalRotation(isLeft ? HumanBodyBones.LeftThumbIntermediate : HumanBodyBones.RightThumbIntermediate,
    //         h.ThumbIntermediate.localRotation * Quaternion.Euler(o.ThumbIntermediate));
    //     _animator.SetBoneLocalRotation(isLeft ? HumanBodyBones.LeftThumbDistal : HumanBodyBones.RightThumbDistal,
    //         h.ThumbDistal.localRotation * Quaternion.Euler(o.ThumbDistal));

    //     _animator.SetBoneLocalRotation(isLeft ? HumanBodyBones.LeftIndexProximal : HumanBodyBones.RightIndexProximal,
    //         h.IndexProximal.localRotation * Quaternion.Euler(o.IndexProximal));
    //     _animator.SetBoneLocalRotation(isLeft ? HumanBodyBones.LeftIndexIntermediate : HumanBodyBones.RightIndexIntermediate,
    //         h.IndexIntermediate.localRotation * Quaternion.Euler(o.IndexIntermediate));
    //     _animator.SetBoneLocalRotation(isLeft ? HumanBodyBones.LeftIndexDistal : HumanBodyBones.RightIndexDistal,
    //         h.IndexDistal.localRotation * Quaternion.Euler(o.IndexDistal));

    //     _animator.SetBoneLocalRotation(isLeft ? HumanBodyBones.LeftMiddleProximal : HumanBodyBones.RightMiddleProximal,
    //         h.MiddleProximal.localRotation * Quaternion.Euler(o.MiddleProximal));
    //     _animator.SetBoneLocalRotation(isLeft ? HumanBodyBones.LeftMiddleIntermediate : HumanBodyBones.RightMiddleIntermediate,
    //         h.MiddleIntermediate.localRotation * Quaternion.Euler(o.MiddleIntermediate));
    //     _animator.SetBoneLocalRotation(isLeft ? HumanBodyBones.LeftMiddleDistal : HumanBodyBones.RightMiddleDistal,
    //         h.MiddleDistal.localRotation * Quaternion.Euler(o.MiddleDistal));

    //     _animator.SetBoneLocalRotation(isLeft ? HumanBodyBones.LeftRingProximal : HumanBodyBones.RightRingProximal,
    //         h.RingProximal.localRotation * Quaternion.Euler(o.RingProximal));
    //     _animator.SetBoneLocalRotation(isLeft ? HumanBodyBones.LeftRingIntermediate : HumanBodyBones.RightRingIntermediate,
    //         h.RingIntermediate.localRotation * Quaternion.Euler(o.RingIntermediate));
    //     _animator.SetBoneLocalRotation(isLeft ? HumanBodyBones.LeftRingDistal : HumanBodyBones.RightRingDistal,
    //         h.RingDistal.localRotation * Quaternion.Euler(o.RingDistal));

    //     _animator.SetBoneLocalRotation(isLeft ? HumanBodyBones.LeftLittleProximal : HumanBodyBones.RightLittleProximal,
    //         h.LittleProximal.localRotation * Quaternion.Euler(o.LittleProximal));
    //     _animator.SetBoneLocalRotation(isLeft ? HumanBodyBones.LeftLittleIntermediate : HumanBodyBones.RightLittleIntermediate,
    //         h.LittleIntermediate.localRotation * Quaternion.Euler(o.LittleIntermediate));
    //     _animator.SetBoneLocalRotation(isLeft ? HumanBodyBones.LeftLittleDistal : HumanBodyBones.RightLittleDistal,
    //         h.LittleDistal.localRotation * Quaternion.Euler(o.LittleDistal));
    // }

    // void ApplyFinger(HandIKTargets h, FingerOffset o, bool isLeft)
    // {
    //     HumanBodyBones thumbP = isLeft ? HumanBodyBones.LeftThumbProximal : HumanBodyBones.RightThumbProximal;
    //     HumanBodyBones thumbI = isLeft ? HumanBodyBones.LeftThumbIntermediate : HumanBodyBones.RightThumbIntermediate;
    //     HumanBodyBones thumbD = isLeft ? HumanBodyBones.LeftThumbDistal : HumanBodyBones.RightThumbDistal;

    //     HumanBodyBones indexP = isLeft ? HumanBodyBones.LeftIndexProximal : HumanBodyBones.RightIndexProximal;
    //     HumanBodyBones indexI = isLeft ? HumanBodyBones.LeftIndexIntermediate : HumanBodyBones.RightIndexIntermediate;
    //     HumanBodyBones indexD = isLeft ? HumanBodyBones.LeftIndexDistal : HumanBodyBones.RightIndexDistal;

    //     HumanBodyBones middleP = isLeft ? HumanBodyBones.LeftMiddleProximal : HumanBodyBones.RightMiddleProximal;
    //     HumanBodyBones middleI = isLeft ? HumanBodyBones.LeftMiddleIntermediate : HumanBodyBones.RightMiddleIntermediate;
    //     HumanBodyBones middleD = isLeft ? HumanBodyBones.LeftMiddleDistal : HumanBodyBones.RightMiddleDistal;

    //     HumanBodyBones ringP = isLeft ? HumanBodyBones.LeftRingProximal : HumanBodyBones.RightRingProximal;
    //     HumanBodyBones ringI = isLeft ? HumanBodyBones.LeftRingIntermediate : HumanBodyBones.RightRingIntermediate;
    //     HumanBodyBones ringD = isLeft ? HumanBodyBones.LeftRingDistal : HumanBodyBones.RightRingDistal;

    //     HumanBodyBones littleP = isLeft ? HumanBodyBones.LeftLittleProximal : HumanBodyBones.RightLittleProximal;
    //     HumanBodyBones littleI = isLeft ? HumanBodyBones.LeftLittleIntermediate : HumanBodyBones.RightLittleIntermediate;
    //     HumanBodyBones littleD = isLeft ? HumanBodyBones.LeftLittleDistal : HumanBodyBones.RightLittleDistal;

    //     _animator.SetBoneLocalRotation(thumbP, Quaternion.Euler(h.ThumbProximal.localEulerAngles + o.ThumbProximal));
    //     _animator.SetBoneLocalRotation(thumbI, Quaternion.Euler(h.ThumbIntermediate.localEulerAngles + o.ThumbIntermediate));
    //     _animator.SetBoneLocalRotation(thumbD, Quaternion.Euler(h.ThumbDistal.localEulerAngles + o.ThumbDistal));

    //     _animator.SetBoneLocalRotation(indexP, Quaternion.Euler(h.IndexProximal.localEulerAngles + o.IndexProximal));
    //     _animator.SetBoneLocalRotation(indexI, Quaternion.Euler(h.IndexIntermediate.localEulerAngles + o.IndexIntermediate));
    //     _animator.SetBoneLocalRotation(indexD, Quaternion.Euler(h.IndexDistal.localEulerAngles + o.IndexDistal));

    //     _animator.SetBoneLocalRotation(middleP, Quaternion.Euler(h.MiddleProximal.localEulerAngles + o.MiddleProximal));
    //     _animator.SetBoneLocalRotation(middleI, Quaternion.Euler(h.MiddleIntermediate.localEulerAngles + o.MiddleIntermediate));
    //     _animator.SetBoneLocalRotation(middleD, Quaternion.Euler(h.MiddleDistal.localEulerAngles + o.MiddleDistal));

    //     _animator.SetBoneLocalRotation(ringP, Quaternion.Euler(h.RingProximal.localEulerAngles + o.RingProximal));
    //     _animator.SetBoneLocalRotation(ringI, Quaternion.Euler(h.RingIntermediate.localEulerAngles + o.RingIntermediate));
    //     _animator.SetBoneLocalRotation(ringD, Quaternion.Euler(h.RingDistal.localEulerAngles + o.RingDistal));

    //     _animator.SetBoneLocalRotation(littleP, Quaternion.Euler(h.LittleProximal.localEulerAngles + o.LittleProximal));
    //     _animator.SetBoneLocalRotation(littleI, Quaternion.Euler(h.LittleIntermediate.localEulerAngles + o.LittleIntermediate));
    //     _animator.SetBoneLocalRotation(littleD, Quaternion.Euler(h.LittleDistal.localEulerAngles + o.LittleDistal));
    // }
    #endregion


}