using UnityEngine;

public class UnityIKTest : MonoBehaviour
{
    [SerializeField] private Transform _targetHandTR;
    [SerializeField] private Vector3 _fingerProximalTR;
    [SerializeField] private Vector3 _fingerInterTR;
    [SerializeField] private bool _followPosition = false;
    [SerializeField] private bool _followRotation = true;

    private Animator _animator;

    void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    void OnAnimatorIK(int layerIndex)
    {
        _animator.SetIKPositionWeight(AvatarIKGoal.RightHand, _followPosition ? 1 : 0);
        _animator.SetIKRotationWeight(AvatarIKGoal.RightHand, _followRotation ? 1 : 0);

        if (_followPosition) _animator.SetIKPosition(AvatarIKGoal.RightHand, _targetHandTR.position);
        if (_followRotation) _animator.SetIKRotation(AvatarIKGoal.RightHand, _targetHandTR.rotation);

        _animator.SetBoneLocalRotation(HumanBodyBones.RightIndexProximal, Quaternion.Euler(_fingerProximalTR));
        _animator.SetBoneLocalRotation(HumanBodyBones.RightIndexIntermediate, Quaternion.Euler(_fingerInterTR));
    }
}
