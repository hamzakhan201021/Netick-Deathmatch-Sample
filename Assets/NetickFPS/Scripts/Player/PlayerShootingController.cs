using UnityEngine;
using Netick.Unity;
using Netick;
using PG.LagCompensation;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public struct LCHitInfo
{
    public ColliderCastHit CCHit;
    public HitColliderCollection HitColliderCollection;
    public int HitColliderIndex;

    public static LCHitInfo Zero
    {
        get { return new LCHitInfo { CCHit = ColliderCastHit.Zero, HitColliderCollection = null, HitColliderIndex = -1 }; }
    }
}

public class PlayerShootingController : NetworkBehaviour
{
    [Header("Weapon")]
    [SerializeField] private WeaponEffects _weaponEffects;
    // TODO maybe later we could add weapon patterns...
    [SerializeField] private Vector3 _weaponRecoilEffect = new Vector3(-5, 0, 0);
    [SerializeField] private float _camRecoilRotAmount = 2;
    [Space]
    [SerializeField] private Transform _gunFirePoint;
    [SerializeField] private ParticleSystem _gunFireEffect;
    [SerializeField] private AudioSource _fireAudioSource;
    [SerializeField] private float _shotCoolDown = 0.1f;
    [SerializeField] private float _maxDistance = 100;
    [SerializeField] private LayerMask _shootableLayerMask;
    [Header("Reload")]
    [SerializeField] private int _magSize = 30;
    //[SerializeField] private int _totalAmmo = 120;
    [SerializeField] private float ReloadTime = 1;

    // Networked Vars
    [Networked] public float GunTimer { get; set; } = 0;
    [Networked] public float ReloadTimer { get; set; } = 0;

    [Networked] public NetworkBool IsFiring { get; set; } = false;
    [Networked] public NetworkBool IsReloading { get; set; } = false;

    [Networked] public int CurrentAmmo { get; set; } = 30;
    [Networked] public int TotalAmmo { get; set; } = 120;

    [Header("Player")]
    [SerializeField] private PlayerHealthController _playerHealthController;
    [SerializeField] private PlayerMovementController _playerMovementController;

    // TODO change to hit collection etc, update to new LC(Lag Compensation)
    [SerializeField] private List<Collider> _rollbackColliders;
    [SerializeField] private HitColliderCollection _hitColliderCollection;

    [Header("Auto lag comp test settings")]
    [SerializeField] private bool _useAutoLagCompTest;
    [SerializeField] private Toggle _constantShootInput;

    [Header("UI")]
    [SerializeField] private TMP_Text _ammoText;
    [SerializeField] private GameObject _reloadingOverlay;

    [HideInInspector] public Vector3 HitPosition;
    [HideInInspector] public Quaternion HitRotation;
    //[HideInInspector] public int HitAuthTick;

    /// <summary>
    /// used to not have to constantly get reference to the lag compensation manager.
    /// </summary>
    private LagCompensationManager _lagCompManager;

    public override void NetworkStart()
    {
        // Get lag comp manager from sandbox.
        _lagCompManager = Sandbox.GetComponent<LagCompensationManager>();

        UpdateWeaponUI();

        _reloadingOverlay.SetActive(false);
    }

    public override void NetworkFixedUpdate()
    {
        HandleShooting();
        HandleReloading();
        HandleEffects();

        if (Input.GetKeyDown(KeyCode.G))
        {
            ColliderCastSystem.DebugDrawColliders();
        }
    }

    public override void NetworkUpdate()
    {
        PlayerInput cInput = Sandbox.GetInput<PlayerInput>();
        //cInput.ClientTick = Sandbox.AuthoritativeTick + 1;

        if (_useAutoLagCompTest && _constantShootInput.isOn)
        {
            cInput.ShootInput = true;
        }
        else
        {
            cInput.ShootInput |= Input.GetKey(KeyCode.Mouse0);
        }

        cInput.ReloadInput |= Input.GetKeyDown(KeyCode.R);

        Sandbox.SetInput(cInput);

        UpdateWeaponUI();
    }

    private void HandleShooting()
    {
        if (FetchInput(out PlayerInput input))
        {
            if (GunTimer > 0)
            {
                GunTimer -= Sandbox.FixedDeltaTime;
            }
            else if (GunTimer < 0)
            {
                GunTimer = 0;
            }

            IsFiring = false;

            if (input.ShootInput)
            {
                if (CanShoot())
                {
                    GunTimer = _shotCoolDown;
                    IsFiring = true;

                    CurrentAmmo -= 1;

                    _weaponEffects.AddRecoil(_weaponRecoilEffect);

                    _playerMovementController.AddRecoilRotation(_camRecoilRotAmount);

                    if (!Sandbox.IsResimulating)
                    {
                        Shoot(input);
                    }
                }
                else if (CanReload()) // auto reload if possible...
                {
                    Reload();
                }
            }

            //if (input.ShootInput && CanShoot())
            //{
            //    GunTimer = _shotCoolDown;
            //    IsFiring = true;

            //    CurrentAmmo -= 1;

            //    if (!Sandbox.IsResimulating)
            //    {
            //        Shoot(input);
            //    }
            //}
        }

        // TODO remove once are tests completed.
        #region OLD Structure Doesn't work well.

        //// Shoot checks:

        ////if (Sandbox.IsResimulating) return;
        //if (!FetchInput(out PlayerInput input)) return;
        
        //if (!input.ShootInput) return;
        //if (GunTimer > 0) return;
        ////if (!_timerFireRate.IsExpiredOrNotRunning(Sandbox)) return;

        //GunTimer = _shotCoolDown;
        ////_timerFireRate = TickTimer.CreateFromSeconds(Sandbox, _shotCoolDown);

        //// Play effects.
        //_gunFireEffect.Play();

        //// init ray...
        //Ray ray = new Ray(_gunFirePoint.position, _gunFirePoint.forward);

        //if (IsServer)
        //{
        //    // Debugging
        //    Debug.Log("Data tick diff, input tick from = " + input.ClientTick + " server tick " + Sandbox.AuthoritativeTick);

        //    //ColliderRollback cR = GetComponentInChildren<ColliderRollback>();

        //    // TODO make rollback module take input source
        //    if (_lagCompManager.RaycastCR(ray, input.ClientTick, out RaycastHit hitInfo, _rollbackColliders.ToArray(), _shootableLayerMask))
        //    //if (_lagCompManager.RaycastLC(ray, input.ClientTick, out LCHitInfo hitInfo, _maxDistance, _hitColliderCollection))
        //    {
        //        Debug.Log("Hit was found");

        //        // LOL anyways
        //        //_playerHealthController.ChangeHealth(-20); SIlly Will you seriously deplete your own health!?


        //        //// LC
        //        //GameObject hitObject = hitInfo.HitColliderCollection.gameObject;

        //        //PlayerHealthController pHC = hitObject.GetComponentInParent<PlayerHealthController>();

        //        //if (pHC)
        //        //{
        //        //    if (pHC == _playerHealthController)
        //        //    {
        //        //        Debug.Log("WHat BRO fired himself no way");
        //        //    }

        //        //    pHC.ChangeHealth(-1);
        //        //}
        //        ////

        //        // Deplete health of hit object if possible
        //        if (hitInfo.transform.TryGetComponent(out ColliderRollback colliderRollback))
        //        {
        //            if (colliderRollback.RootTransform.TryGetComponent(out PlayerHealthController playerHealthController))
        //            {
        //                if (playerHealthController == _playerHealthController)
        //                {
        //                    Debug.Log("WHat BRO fired himself no way");
        //                    Debug.Log("Need to exclude from CR Collider Rollback");
        //                }

        //                playerHealthController.ChangeHealth(-1);
        //            }
        //        }
        //    }

        //    //ColliderCastSystem.Simulate(input.ClientTick);
        //}
        //else
        //{


        //    //if (ColliderCastSystem.ColliderCastTransform(ray.origin, ray.direction, _maxDistance, out ColliderCastHit hit, out HitColliderCollection collection, out int index))
        //    //{
        //    //    HitColliderGeneric col = collection.GetHitColliderAtIndex(index);

        //    //    _lagCompManager.SendClientHitObjectDataRpc(col.transform.position, col.transform.rotation, false, Sandbox.AuthoritativeTick);
        //    //}

        //    bool didHit = Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, _shootableLayerMask);

        //    if (didHit && hitInfo.transform.TryGetComponent(out ColliderRollback cR))
        //    {
        //        // Spawn the client hit perspective duplicate of the collider
        //        Debug.Log("Client");

        //        // TODO fix precision check.
        //        // TODO improve hit precision check...
        //        _lagCompManager.SendClientHitObjectDataRpc(cR.transform.position, cR.transform.rotation, false, Sandbox.AuthoritativeTick);
        //    }
        //}

        #endregion
    }

    private void Shoot(PlayerInput input)
    {
        Ray ray = new Ray(_gunFirePoint.position, _gunFirePoint.forward);

        if (IsServer)
        {
            //if (IsHost)
            //{
            //    HitAuthTick = Sandbox.AuthoritativeTick;
            //}

            // Debugging
            Debug.Log("Data tick diff, input tick from = " + input.ClientTick + " server tick " + Sandbox.AuthoritativeTick);
            Debug.Log("Sandbox Remote Interpolation Tick to is = " + input.InterpolationTickTo);
            Debug.Log("Sandbox Remote Interpolation Tick from is = " + input.InterpolationTickFrom);
            Debug.Log("Sandbox Remote Interpolation Tick to 2 is = " + input.InterpolationTickTo2);
            Debug.Log("Sandbox Remote Interpolation Tick from 2 is = " + input.InterpolationTickFrom2);

            //ColliderRollback cR = GetComponentInChildren<ColliderRollback>();
            TickInterpolation interpData = new TickInterpolation(input.InterpolationTickTo, input.InterpolationAlpha);

            // TODO make rollback module take input source
            //if (_lagCompManager.RaycastCR(ray, input.ClientTick, out RaycastHit hitInfo, _rollbackColliders.ToArray(), _shootableLayerMask))
            if (_lagCompManager.RaycastLC(ray, input.ClientTick, interpData, out LCHitInfo hitInfo, _maxDistance, _hitColliderCollection))
            {
                Debug.Log("Hit was found");

                //// LC
                GameObject hitObject = hitInfo.HitColliderCollection.gameObject;

                PlayerHealthController pHC = hitObject.GetComponentInParent<PlayerHealthController>();

                if (pHC)
                {
                    if (pHC == _playerHealthController)
                    {
                        Debug.Log("WHat BRO fired himself no way(THIS MUST NOT HAPPEN)");
                    }

                    pHC.ChangeHealth(-1);
                }
                ////

                //// Deplete health of hit object if possible
                //if (hitInfo.transform.TryGetComponent(out ColliderRollback colliderRollback))
                //{
                //    if (colliderRollback.RootTransform.TryGetComponent(out PlayerHealthController playerHealthController))
                //    {
                //        if (playerHealthController == _playerHealthController)
                //        {
                //            Debug.Log("WHat BRO fired himself no way");
                //            Debug.Log("Need to exclude from CR Collider Rollback");
                //        }

                //        playerHealthController.ChangeHealth(-1);
                //    }
                //}
            }

            //ColliderCastSystem.Simulate(input.ClientTick);
        }
        else
        {

            LagCompensationManager lagComp = Sandbox.GetComponent<LagCompensationManager>();

            if (!lagComp.CompareAndCalculatePrecision) return;

            bool useInterpData = lagComp.UseInterpData;

            if (ColliderCastSystem.ColliderCastTransformWithExclusion(ray.origin, ray.direction, _maxDistance, useInterpData, out ColliderCastHit hit, out HitColliderCollection collection, out int index, _hitColliderCollection, false))
            {
                HitColliderGeneric col = collection.GetHitColliderAtIndex(index);

                // TODO remove XD
                // this is for the precision check data
                // HitPosition = col.transform.position;
                // HitRotation = col.transform.rotation;
                //HitAuthTick = Sandbox.AuthoritativeTick;

                _lagCompManager.SendClientHitObjectDataRpc(col.transform.position, col.transform.rotation, false, useInterpData ? input.InterpolationTickTo : input.ClientTick);

            }

            //bool didHit = Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, _shootableLayerMask);

            //if (didHit && hitInfo.transform.TryGetComponent(out ColliderRollback cR))
            //{
            //    // Spawn the client hit perspective duplicate of the collider
            //    Debug.Log("Client");

            //    // TODO fix precision check.
            //    // TODO improve hit precision check...
            //    _lagCompManager.SendClientHitObjectDataRpc(cR.transform.position, cR.transform.rotation, false, Sandbox.AuthoritativeTick);
            //}
        }
    }


    private void HandleReloading()
    {
        if (FetchInput(out PlayerInput input))
        {
            // Update reload timer
            if (ReloadTimer > 0)
            {
                ReloadTimer -= Sandbox.FixedDeltaTime;
            }
            else if (ReloadTimer < 0)
            {
                ReloadTimer = 0;
            }

            // Reset is reloading after timer ends.
            if (ReloadTimer <= 0 && IsReloading)
            {
                IsReloading = false;
            }

            // Check for reload
            if (input.ReloadInput && CanReload())
            {
                
                //int missing = magSize - currentAmmo;
                //int toLoad = Mathf.Min(missing, reserveAmmo);
                //reserveAmmo -= toLoad;
                //currentAmmo += toLoad;

                Reload();
            }
        }
    }

    private void Reload()
    {
        ReloadTimer = ReloadTime;

        IsReloading = true;

        int usedBullets = _magSize - CurrentAmmo;
        int bulletsToLoad = Mathf.Min(usedBullets, TotalAmmo);

        TotalAmmo -= bulletsToLoad;
        CurrentAmmo += bulletsToLoad;
    }

    private void HandleEffects()
    {
        if (IsFiring && !IsResimulating)
        {
            // Effects
            _gunFireEffect.Play();
            _fireAudioSource.Play();
        }
    }

    private bool CanShoot()
    {
        return GunTimer <= 0 && CurrentAmmo != 0 && !IsReloading;
    }

    // TODO maybe we could use can shoot as one check within
    private bool CanReload()
    {
        return GunTimer <= 0 && ReloadTimer <= 0 && CurrentAmmo < _magSize && TotalAmmo != 0 && !IsReloading;
    }

    private void UpdateWeaponUI()
    {
        _ammoText.text = CurrentAmmo + "/" + TotalAmmo;

        _reloadingOverlay.SetActive(IsReloading);
    }
}