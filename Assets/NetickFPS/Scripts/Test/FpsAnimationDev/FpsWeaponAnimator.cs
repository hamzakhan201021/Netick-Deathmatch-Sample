using UnityEngine;
using Cinemachine;
using Unity.Cinemachine;

public class FpsWeaponAnimator : MonoBehaviour
{

    [SerializeField] private string _reloadClipName = "WeaponRifleReload";
    [SerializeField] private float _nTransitionTime = 0.2f;
    [SerializeField] private bool _reload = false;
    [SerializeField] private CinemachineImpulseSource _cinemachineImpulse;

    [SerializeField] private GameObject _weaponMag;
    [SerializeField] private Transform _magInHand;
    [SerializeField] private float _destroyMagTime = 3;

    private Animator _animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (_reload)
        {
            _reload = false;
            Reload();
        }
    } 

    public void Reload()
    {
        _animator.CrossFade(_reloadClipName, _nTransitionTime);
    }

    public void OnDropMag(AnimationEvent animationEvent)
    {
        // Spawn a clone of the mag etc and drop it
        // Some basic code to spawn a mag.
        GameObject magObject = Instantiate(_weaponMag, _magInHand.position, _magInHand.rotation);
        magObject.AddComponent<Rigidbody>();
        magObject.SetActive(true);

        Destroy(magObject, _destroyMagTime);
    }

    public void CamImpulse(AnimationEvent animationEvent)
    {
        _cinemachineImpulse.GenerateImpulse();
    }
}
