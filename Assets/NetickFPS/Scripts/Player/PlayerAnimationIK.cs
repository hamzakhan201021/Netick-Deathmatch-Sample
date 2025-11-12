using System;
using System.Collections.Generic;
using UnityEngine;

// TODO get this properly working make networked and test
public class PlayerAnimationIK : MonoBehaviour
{

    [Serializable]
    public class RotationBoneCustom
    {
        [Tooltip("Constrained Bone")]
        public Transform Bone;
        public HumanBodyBones BoneID;
        [Range(0, 1)]
        [Tooltip("Weight scale of the rotation applied to this bone")]
        public float Weight;
        [Tooltip("Used Internally, the source offset of his bone")]
        public Quaternion SourceOffset;
        [Tooltip("Used internally to store this bone's rotation")]
        public Quaternion CurrentRotation;
    }

    [Serializable]
    public class RotationBone
    {
        public Transform Constraint;
        public float Weight;
        public Vector3 Offset;
    }

    // TODO, we need to improve and cleanup this whole codebase XD
    [Header("Body Rotation")]
    [SerializeField] private string _iKLayerName = "UpperBody";
    [SerializeField] private int _iKLayerID = 1;
    [Tooltip("Whether or not to check the layer of the IK callback")]
    [SerializeField] private bool _checkLayer = false;
    [SerializeField] private PlayerMovementController _playerMovementController;

    // [SerializeField] private List<RotationBoneCustom> _rotationBones;
    [SerializeField] private List<RotationBone> _rotationBonesAR;

    [SerializeField] private float _rotationSpeed = 15;

    [Header("Weapon Bone")]
    [SerializeField] private Transform _camPosition;
    [SerializeField] private Transform _targetBone;

    [Header("Weapon Grip")]
    [SerializeField] private Transform _weaponLGrip;
    [SerializeField] private Transform _weaponLGripHint;
    [SerializeField] private Transform _weaponRGrip;
    [SerializeField] private Transform _weaponRGripHint;

    [Header("Grip Position Offsets")]
    [SerializeField] private Vector3 _lGripOffset = Vector3.zero;
    [SerializeField] private Vector3 _rGripOffset = Vector3.zero;

    // [Header("Testing")]
    // public HumanBodyBones Bone1;
    // public Vector3 LTRotation1;
    // private Quaternion OffsetRot1;
    // public HumanBodyBones Bone2;
    // public Vector3 LTRotation2;
    // private Quaternion OffsetRot2;
    // public HumanBodyBones Bone3;
    // public Vector3 LTRotation3;
    // private Quaternion OffsetRot3;

    private Animator _animator;

    //private Quaternion _currentRot;

    private void Start()
    {
        _animator = GetComponent<Animator>();

        // TESTING
        //_sourceOffset = Quaternion.Inverse(_source.rotation) * _target.rotation;
        //_sourceOffset = Quaternion.Inverse(transform.rotation) * _target.rotation;

        // CalculateRotationBonesOffsets();
        // TESTCalcRotationOffsets();
    }

    // private void TESTCalcRotationOffsets()
    // {
    //     // OffsetRot1 = Quaternion.Inverse(transform.rotation) * _animator.GetBoneTransform(Bone1).rotation;
    //     // OffsetRot2 = Quaternion.Inverse(transform.rotation) * _animator.GetBoneTransform(Bone2).rotation;
    //     // OffsetRot3 = Quaternion.Inverse(transform.rotation) * _animator.GetBoneTransform(Bone3).rotation;
    // }

    // private void CalculateRotationBonesOffsets()
    // {
    //     for (int i = 0; i < _rotationBones.Count; i++)
    //     {
    //         // Calculate source offset..
    //         // Might not be the best to use the transform.rotation, but as thats our reference rotation it should work fine.
    //         _rotationBones[i].SourceOffset = Quaternion.Inverse(transform.rotation) * _rotationBones[i].Bone.rotation;
    //     }
    // }

    private void Update()
    {
        Vector3 localEuler = GetLatestRotation();
        localEuler.y = 0;

        for (int i = 0; i < _rotationBonesAR.Count; i++)
        {
            // TODO NEW without smoothing

            Quaternion rawTargetRot = Quaternion.Euler(localEuler) * Quaternion.Euler(_rotationBonesAR[i].Offset);
            Quaternion targetRot = Quaternion.Lerp(Quaternion.identity, rawTargetRot, _rotationBonesAR[i].Weight);

            _rotationBonesAR[i].Constraint.localRotation = targetRot;
        }
    }
    
    private void OnAnimatorIK(int layerIndex)
    {
        if (_checkLayer && layerIndex != _iKLayerID) return;

        // Set Position and rotation of the main cam which also has the weapon etc.
        // _camPosition.position = _animator.GetBoneTransform(HumanBodyBones.Head).position;

        // Vector3 localEuler = GetLatestRotation();
        // localEuler.y = 0;

        // _camPosition.localRotation = Quaternion.Euler(localEuler);

        // Set IK Weights
        _animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
        _animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);

        _animator.SetIKHintPositionWeight(AvatarIKHint.LeftElbow, 1);
        _animator.SetIKHintPositionWeight(AvatarIKHint.RightElbow, 1);

        _animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
        _animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);

        // Set IK positions
        _animator.SetIKPosition(AvatarIKGoal.LeftHand, _weaponLGrip.position);
        _animator.SetIKPosition(AvatarIKGoal.RightHand, _weaponRGrip.position);

        _animator.SetIKHintPosition(AvatarIKHint.LeftElbow, _weaponLGripHint.position);
        _animator.SetIKHintPosition(AvatarIKHint.RightElbow, _weaponRGripHint.position);

        // Set IK rotations
        _animator.SetIKRotation(AvatarIKGoal.LeftHand, _weaponLGrip.rotation * Quaternion.Euler(_lGripOffset));
        _animator.SetIKRotation(AvatarIKGoal.RightHand, _weaponRGrip.rotation * Quaternion.Euler(_rGripOffset));
    }

    private Vector3 GetLatestRotation()
    {
        return new Vector3(_playerMovementController.YawPitch.y, _playerMovementController.YawPitch.x, 0);
    }

    // TODO remove old broken code.
    #region
    //[SerializeField] private PlayerMovementController _playerMovementController;
    //[SerializeField] private Transform _spine;
    //[SerializeField] private Transform _chest;
    //[SerializeField] private Transform _upperChest;

    //[Range(0, 1), SerializeField] private float _spineWeight;
    //[Range(0, 1), SerializeField] private float _chestWeight;
    //[Range(0, 1), SerializeField] private float _upperChestWeight;

    //private Animator _animator;

    //Dictionary<HumanBodyBones, Quaternion> boneRestRotations;

    //void Awake()
    //{
    //    _animator = GetComponent<Animator>();

    //    boneRestRotations = new Dictionary<HumanBodyBones, Quaternion>();
    //    CacheRestRotation(HumanBodyBones.Spine);
    //    CacheRestRotation(HumanBodyBones.Chest);
    //    CacheRestRotation(HumanBodyBones.UpperChest);
    //}

    //void CacheRestRotation(HumanBodyBones bone)
    //{
    //    var t = _animator.GetBoneTransform(bone);
    //    if (t != null)
    //        boneRestRotations[bone] = t.localRotation;
    //}

    //void LateUpdate()
    //{
    //    var spine = _animator.GetBoneTransform(HumanBodyBones.Spine);

    //    var spineRest = boneRestRotations[HumanBodyBones.Spine];
    //    //spine.localRotation = spineRest * Quaternion.AngleAxis(yaw * 0.25f, spine.transform.up);
    //    //chest.localRotation = chestRest * Quaternion.AngleAxis(yaw * 0.55f, chest.transform.up);
    //    //upperChest.localRotation = upperChestRest * Quaternion.AngleAxis(yaw * 1.0f, upperChest.transform.up);
    //    Vector3 rotation = new Vector3(_playerMovementController.YawPitch.y, _playerMovementController.YawPitch.x, 0);

    //    spine.rotation = spineRest * Quaternion.Euler(rotation);
    //}

    //[SerializeField] PlayerMovementController _playerMovementController;
    //[SerializeField] HumanBodyBones spineBone = HumanBodyBones.Spine;
    //[SerializeField] HumanBodyBones chestBone = HumanBodyBones.Chest;
    //[SerializeField] HumanBodyBones upperChestBone = HumanBodyBones.UpperChest;

    //[Range(0f, 1f)] public float spineWeight = 0.25f;
    //[Range(0f, 1f)] public float chestWeight = 0.55f;
    //[Range(0f, 1f)] public float upperChestWeight = 1f;

    //Animator anim;

    //struct BoneInfo
    //{
    //    public Transform t;
    //    public Quaternion offset; // boneWorld * inv(rootWorld)
    //}

    //BoneInfo spineInfo;
    //BoneInfo chestInfo;
    //BoneInfo upperChestInfo;

    //bool initialized;

    //void Start()
    //{
    //    anim = GetComponent<Animator>();

    //    Transform root = transform;

    //    void InitBone(ref BoneInfo info, HumanBodyBones bone)
    //    {
    //        info.t = anim.GetBoneTransform(bone);
    //        if (info.t != null)
    //            info.offset = info.t.rotation * Quaternion.Inverse(root.rotation);
    //    }

    //    InitBone(ref spineInfo, spineBone);
    //    InitBone(ref chestInfo, chestBone);
    //    InitBone(ref upperChestInfo, upperChestBone);

    //    initialized = true;
    //}

    //private void LateUpdate()
    //{
    //    //if (anim.GetLayerName(layerIndex) != "UpperBody") return;
    //    if (!initialized) return;

    //    // yaw/pitch from your controller (adjust axes as you use them)
    //    float yaw = _playerMovementController?.YawPitch.x ?? 0f;   // left/right
    //    float pitch = _playerMovementController?.YawPitch.y ?? 0f; // up/down

    //    ApplyBone(spineInfo, spineWeight, yaw, pitch);
    //    ApplyBone(chestInfo, chestWeight, yaw, pitch);
    //    ApplyBone(upperChestInfo, upperChestWeight, yaw, pitch);
    //}

    //void ApplyBone(BoneInfo info, float weight, float yawDeg, float pitchDeg)
    //{
    //    if (info.t == null) return;

    //    // Build target rotations relative to the root (world space)
    //    // Use root's axes so it's consistent across rigs
    //    Quaternion yawRot = Quaternion.AngleAxis(yawDeg * weight, transform.up);
    //    Quaternion pitchRot = Quaternion.AngleAxis(-pitchDeg * weight, transform.right);

    //    // Compose desired bone world rotation:
    //    // desiredWorld = offset * rootRotation * yaw * pitch
    //    Quaternion desiredWorld = info.offset * transform.rotation * yawRot * pitchRot;

    //    // Apply in world-space after the animator has run.
    //    // We use Transform.rotation here because SetBoneLocalRotation expects a local
    //    // rotation and that still depends on the bone parent; setting world rotation
    //    // keeps this simple and robust across rigs.
    //    info.t.rotation = desiredWorld;
    //}

    //private void OnAnimatorIK(int layerIndex)
    //{
    //    if (_animator.GetLayerName(layerIndex) != "UpperBody") return;

    //    Vector3 rotation = new Vector3(_playerMovementController.YawPitch.y, _playerMovementController.YawPitch.x, 0);

    //    _animator.SetBoneLocalRotation(HumanBodyBones.Spine, Quaternion.Euler(rotation));
    //}
    #endregion
}