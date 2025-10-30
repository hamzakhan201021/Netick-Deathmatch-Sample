using UnityEngine;
using Netick.Unity;
using Netick;
using PG.LagCompensation;
using System.Collections.Generic;
using UnityEngine.UI;

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

    [SerializeField] private Transform _gunFirePoint;
    [SerializeField] private ParticleSystem _gunFireEffect;
    [SerializeField] private PlayerHealthController _playerHealthController;

    [SerializeField] private LayerMask _shootableLayerMask;

    // TODO change to hit collection etc, update to new LC(Lag Compensation)
    [SerializeField] private List<Collider> _rollbackColliders;
    [SerializeField] private HitColliderCollection _hitColliderCollection;

    [SerializeField] private float _shotCoolDown = 0.1f;
    [SerializeField] private float _maxDistance = 100;

    [Networked] public float GunTimer { get; set; } = 0;

    [Header("Auto lag comp test settings")]
    [SerializeField] private bool _useAutoLagCompTest;
    [SerializeField] private Toggle _constantShootInput;

    //[Networked] private TickTimer _timerFireRate { get; set; }

    /// <summary>
    /// used to not have to constantly get reference to the lag compensation manager.
    /// </summary>
    private LagCompensationManager _lagCompManager;

    //public override void NetworkUpdate()
    //{
    //    if (!IsInputSource || !Sandbox.InputEnabled)
    //        return;

    //    var networkInput = Sandbox.GetInput<FirstPersonInput>();

    //    networkInput.ShootInput = Input.GetKey(KeyCode.Mouse0);

    //    Sandbox.SetInput(networkInput);
    //}

    public override void NetworkStart()
    {
        // Get lag comp manager from sandbox.
        _lagCompManager = Sandbox.GetComponent<LagCompensationManager>();
    }

    public override void NetworkFixedUpdate()
    {
        HandleShooting();

        //if (GunTimer > 0)
        //{
        //    GunTimer -= Sandbox.FixedDeltaTime;
        //}

        //if (GunTimer < 0)
        //{
        //    GunTimer = 0;
        //}

        //if (FetchInput(out PlayerInput input))
        //{
        //    if (input.ShootInput && GunTimer <= 0)
        //    {
        //        GunTimer = _shotCoolDown;

        //        if (!Sandbox.IsResimulating)
        //        {
        //            Debug.Log("One shot");
        //        }
        //    }
        //}
    }

    public override void NetworkUpdate()
    {
        PlayerInput cInput = Sandbox.GetInput<PlayerInput>();
        cInput.ClientTick = Sandbox.AuthoritativeTick + 1;

        if (_useAutoLagCompTest && _constantShootInput.isOn)
        {
            cInput.ShootInput = true;
        }
        else
        {
            cInput.ShootInput |= Input.GetKey(KeyCode.Mouse0);
        }

        Sandbox.SetInput(cInput);
    }

    private void HandleShooting()
    {
        // Manage timer TODO use netick ticktimer
        if (GunTimer > 0)
        {
            GunTimer -= Sandbox.FixedDeltaTime;
        }
        else if (GunTimer < 0)
        {
            GunTimer = 0;
        }

        if (FetchInput(out PlayerInput input))
        {
            if (input.ShootInput && GunTimer <= 0)
            {
                GunTimer = _shotCoolDown;

                if (!Sandbox.IsResimulating)
                {
                    Shoot(input);
                }
            }
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
            // Debugging
            Debug.Log("Data tick diff, input tick from = " + input.ClientTick + " server tick " + Sandbox.AuthoritativeTick);

            //ColliderRollback cR = GetComponentInChildren<ColliderRollback>();

            // TODO make rollback module take input source
            //if (_lagCompManager.RaycastCR(ray, input.ClientTick, out RaycastHit hitInfo, _rollbackColliders.ToArray(), _shootableLayerMask))
            if (_lagCompManager.RaycastLC(ray, input.ClientTick, out LCHitInfo hitInfo, _maxDistance, _hitColliderCollection))
            {
                Debug.Log("Hit was found");

                // LOL anyways
                //_playerHealthController.ChangeHealth(-20); SIlly Will you seriously deplete your own health!?


                //// LC
                GameObject hitObject = hitInfo.HitColliderCollection.gameObject;

                PlayerHealthController pHC = hitObject.GetComponentInParent<PlayerHealthController>();

                if (pHC)
                {
                    if (pHC == _playerHealthController)
                    {
                        Debug.Log("WHat BRO fired himself no way");
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


            if (ColliderCastSystem.ColliderCastTransform(ray.origin, ray.direction, _maxDistance, out ColliderCastHit hit, out HitColliderCollection collection, out int index))
            {
                HitColliderGeneric col = collection.GetHitColliderAtIndex(index);

                _lagCompManager.SendClientHitObjectDataRpc(col.transform.position, col.transform.rotation, false, Sandbox.AuthoritativeTick);
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
}