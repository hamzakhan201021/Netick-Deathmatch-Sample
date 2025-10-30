using UnityEngine;
using Netick.Unity;
using System.Collections.Generic;
using Netick;
using PG.LagCompensation;
using System.Collections;

// TODO use namespace, cleanup .. ..
public class LagCompensationManager : NetworkBehaviour
{

    public class ColliderHistory
    {
        public Collider Collider;
        public Queue<Collider3DState> States;
        public int MaxHistory;

        public ColliderHistory(Collider collider, int maxHistory)
        {
            Collider = collider;
            MaxHistory = maxHistory;
            States = new Queue<Collider3DState>(maxHistory);
        }

        public void Record(int tick)
        {
            if (States.Count >= MaxHistory)
                States.Dequeue();
            States.Enqueue(new Collider3DState(Collider, tick));
        }

        public bool GetStateAtOrBefore(int targetTick, out Collider3DState state)
        {
            state = default;
            if (States.Count == 0) return false;
            var arr = States.ToArray();
            for (int i = arr.Length - 1; i >= 0; i--)
            {
                // TODO make sure it works
                if (arr[i].Tick == targetTick)
                {
                    state = arr[i];
                    return true;
                }
            }
            return false;
        }
    }

    [Header("Debugging")]
    [SerializeField] private bool _logData = true;

    [SerializeField] private bool _comparisonCubes = true;
    [SerializeField] private GameObject _serverCubeLagComp;
    [SerializeField] private GameObject _clientCubeLagComp;
    [Tooltip("Time before destroying the spawned object")]
    [SerializeField] private float _cubeCycleDuration = 15;

    //private static readonly Dictionary<Collider, ColliderHistory> _collider3DStates = new();

    //PhysicsScene _physicsScene;
    //const string PHYSICS_SCENE_NAME = "RollbackScene";

    private readonly List<Collider> _colliders3D = new();
    private readonly Dictionary<Collider, ColliderHistory> _collider3DStates = new();

    // Debugging:
    private readonly Dictionary<int, TransformFrameData> clientHits = new();
    private readonly Dictionary<int, TransformFrameData> serverHits = new();

    // Singleton for now

    //public static LagCompensationManager Instance;

    //public Action OnReady;

    public void Register(ColliderRollback c, int historyLength)
    {
        //float tickRate = .TickRate;
        //int historyLength = Mathf.CeilToInt(c.RollbackSeconds * tickRate);

        //_collider3DStates[c.Id] = new ColliderHistory
        //{
        //    Collider = c.Collider,
        //    //States = new NativeRingQueue<Collider3DState>(capacity: 10, Allocator.Persistent),
        //    States = new Queue<Collider3DState>(historyLength),
        //    HistoryLength = historyLength
        //};

        //Debug.Log(c.Collider.name);

        //Collider collider = c.GetComponent<Collider>();

        // TODO remove all useless code, cleanup project to use only custom lag comp
        _collider3DStates.Add(c.Collider, new ColliderHistory(c.Collider, historyLength));
        _colliders3D.Add(c.Collider);

        //_collider3DStates.Add(collider, new ColliderHistory
        //{
        //    //Collider = c.Collider,
        //    //States = new NativeRingQueue<Collider3DState>(capacity: 10, Allocator.Persistent),
        //    States = new Queue<Collider3DState>(historyLength),
        //    HistoryLength = historyLength
        //});
    }

    public void Unregister(ColliderRollback c)
    {
        _collider3DStates.Remove(c.Collider);
        _colliders3D.Remove(c.Collider);
    }

    // TODO remove this
    public override void NetworkAwake()
    {
        //Instance = this;
    }

    public override void NetworkStart()
    {
        if (!IsServer) return;

        // Prevent from destruction..
        DontDestroyOnLoad(this);

        // Register to late fixed update
        Sandbox.PostNetworkFixedUpdate += OnPostNetworkFixedUpdate;

        // Create Physics Scene...
        //var parameters = new CreateSceneParameters(LocalPhysicsMode.Physics3D);

        //Scene rollbackScene = SceneManager.CreateScene(PHYSICS_SCENE_NAME, parameters);

        //_physicsScene = rollbackScene.GetPhysicsScene();


        // invoke at the end to ensure ready for objects to come.
        //OnReady.Invoke();
    }

    public override void NetworkDestroy()
    {
        // Deregister to late fixed update
        Sandbox.PostNetworkFixedUpdate -= OnPostNetworkFixedUpdate;
    }

    private void OnPostNetworkFixedUpdate()
    {
        RecordStates();

        // Debugging
        LogStates();
    }

    private void RecordStates()
    {
        // Don't record on clients.
        if (!IsServer) return;


        // Don't need this check if we are on the server anyways.
        //if (IsResimulating) return;


        for (int i = 0; i < ColliderCastSystem.SimulationObjects.Count; i++)
        {
            ColliderCastSystem.SimulationObjects[i].AddStateAll(Sandbox.Tick);
        }


        // OLD WORKING code below

        // Don't record when there is nothing to record // lol
        // exit early.
        if (_collider3DStates.Count == 0) return;

        for (int i = 0; i < _colliders3D.Count; i++)
        {
            _collider3DStates[_colliders3D[i]].Record(Sandbox.Tick);
        }

        // END





        // Ancient Man's Code.

        //for (var i = 0; i < _colliders3D.Count; i++)
        //{
        //    var col = _colliders3D[i];

        //    if (_collider3DStates.TryGetValue(col, out var history))
        //    {
        //        int tick = Sandbox.Tick;
        //        ulong uTick = (ulong)tick;

        //        history.Write(uTick, new Collider3DState(col));
        //    }   
        //}

        //foreach (var collider in _collider3DStates.Keys)
        //{
        //    Collider3DState state = new Collider3DState(collider);

        //    var colliderHistory = _collider3DStates[collider];

        //    if (colliderHistory..Count == colliderHistory.HistoryLength)
        //    {
        //        colliderHistory.States.Dequeue();
        //    }

        //    colliderHistory.States.Enqueue(state);
        //}
    }

    private void LogStates()
    {
        //if (!_logStatesData) return;
        //if (!IsServer) return;

        //Debug.Log("Collider States Count " + _collider3DStates.Count);

        //foreach (var collider in _collider3DStates.Keys)
        //{
        //    Debug.Log("Collider name " + collider.name);
        //}

        //for (int i = 0; i < _collider3DStates.Count; i++)
        //{
        //    var collider = _collider3DStates[_colliders3D[i]];

        //    //for (int j = 0; j < 5; i++)
        //    //{
        //    //    //Debug.Log("States of this collider, position: " + collider[i].position + " rotation: " + collider[i].rotation + " scale: " + collider[i].scale);
        //    //    Debug.Log("States of this collider, position: " + collider.States.position + " rotation: " + collider[i].rotation + " scale: " + collider[i].scale);
        //    //}

        //    int displayCount = 5;
        //    int displayCounter = 0;

        //    foreach (var state in collider.States)
        //    {
        //        Debug.Log("States of this collider, position: " + state.position + " rotation: " + state.rotation + " scale: " + state.scale);

        //        displayCounter++;

        //        if (displayCounter >= displayCount) break;
        //    }
        //}

        //foreach (var collider in _collider3DStates.Values)
        //{
        //    int displayCount = 5;
        //    int displayCounter = 0;

        //    foreach (var colState in collider)
        //    {
        //        Debug.Log("States of this collider, position: " + colState.position + " rotation: " + colState.rotation + " scale: " + colState.scale);

        //        displayCounter++;

        //        if (displayCounter >= displayCount)
        //        {
        //            break;
        //        }
        //    }
        //}
    }

    // Cast Functions Development;



    //public bool RaycastAtTick(Ray ray, int tickOffset, out RaycastHit hit)
    //{
    //    hit = default;
    //    float closest = float.MaxValue;
    //    bool hitFound = false;

    //    int targetTick = Sandbox.Tick - Mathf.Max(0, tickOffset);

    //    foreach (var kvp in _collider3DStates)
    //    {
    //        var history = kvp.Value;
    //        if (history == null) continue;
    //        if (!history.GetStateAtOrBefore(targetTick, out var state)) continue;
    //        if (!history.Collider) continue;

    //        var matrix = Matrix4x4.TRS(state.position, state.rotation, state.scale);
    //        var inv = matrix.inverse;

    //        var localOrigin = inv.MultiplyPoint3x4(ray.origin);
    //        var localDir = inv.MultiplyVector(ray.direction);

    //        var localRay = new Ray(localOrigin, localDir);

    //        if (history.Collider.Raycast(localRay, out var localHit, Mathf.Infinity))
    //        {
    //            var worldPoint = matrix.MultiplyPoint3x4(localHit.point);
    //            var worldNormal = matrix.MultiplyVector(localHit.normal).normalized;
    //            var dist = Vector3.Distance(ray.origin, worldPoint);

    //            if (dist < closest)
    //            {
    //                //closest = dist;
    //                //hitFound = true;
    //                //hit = localHit;
    //                //hit.point = worldPoint;
    //                //hit.normal = worldNormal;
    //                //hit.distance = dist;
    //                closest = dist;
    //                hitFound = true;

    //                localHit.point = worldPoint;
    //                localHit.normal = worldNormal;
    //                localHit.distance = dist;

    //                hit = localHit;

    //                // TODO for now disabled as we don't need this
    //                //hit.collider = history.Collider;
    //            }
    //        }
    //    }

    //    return hitFound;
    //}

    /// <summary>
    /// [DEPRECATED] Raycast using (OLD) collider rollback method against all the colliders at a specific tick, while ignoring specific colliders
    /// Use RaycastLC, this won't do anything XD.
    /// </summary>
    /// <param name="ray"></param>
    /// <param name="tick"></param>
    /// <param name="hit"></param>
    /// <param name="ignoreCollider"></param>
    /// <returns></returns>
    public bool RaycastCR(Ray ray, int preciseTick, out RaycastHit hit, Collider[] ignoreColliders, LayerMask layersToHit)
    {
        hit = default;
        //return false;
        // Get target tick, e.g. -1 for some reason gets the correct tick idk why.
        int targetTick = preciseTick - 1;

        var originalStates = new List<(Transform transform, Vector3 pos, Quaternion rot, Vector3 scale)>(_colliders3D.Count);

        //Debug.Log("The target tick is " + targetTick);
        //Debug.Log("More tick values coming ");

        //Debug.Log(interpData.RemoteInterpFrom);
        //Debug.Log(interpData.RemoteInterpTo);
        //Debug.Log(interpData.RemoteInterpAlpha);
        //interpData.RemoteInterpFrom += 3;
        //interpData.RemoteInterpTo += 3;

        foreach (var col in _colliders3D)
        {
            if (!col) continue;


            /// New list ignore
            bool ignore = false;

            foreach (Collider collider in ignoreColliders)
            {
                if (col == collider)
                {
                    ignore = true;

                    break;
                }
            }

            if (ignore) continue;

            // older one col support only.

            //if (ignoreCollider && col == ignoreCollider) continue; // TODO use input source instead

            var tr = col.transform;
            originalStates.Add((tr, tr.position, tr.rotation, tr.localScale));

            if (_collider3DStates.TryGetValue(col, out var history))
            {
                if (history.GetStateAtOrBefore(targetTick, out var state))
                {
                    tr.SetPositionAndRotation(state.position, state.rotation);
                    tr.localScale = state.scale;
                }
                //if (history.GetStateAtOrBefore(interpData.RemoteInterpFrom, out var fromState) && history.GetStateAtOrBefore(interpData.RemoteInterpTo, out var toState))
                //{
                //    var state = fromState.Interpolate(toState, interpData.RemoteInterpAlpha);

                //    tr.SetPositionAndRotation(state.position, state.rotation);
                //    tr.localScale = state.scale;
                //}
            }
        }

        Physics.SyncTransforms();

        bool hitFound = Physics.Raycast(ray, out hit, Mathf.Infinity, layersToHit, QueryTriggerInteraction.Ignore);


        // Server temporary object for visual
        if (hitFound)
        {
            if (hit.transform.TryGetComponent(out ColliderRollback cR))
            {
                SendClientHitObjectDataRpc(cR.transform.position, cR.transform.rotation, true, preciseTick);

                //if (cR.RootTransform.TryGetComponent(out PlayerHealthController playerHealthController))
                //{
                //    // For Visualising


                //}
            }
        }




        for (int i = 0; i < originalStates.Count; i++)
        {
            var s = originalStates[i];
            s.transform.SetPositionAndRotation(s.pos, s.rot);
            s.transform.localScale = s.scale;
        }

        Physics.SyncTransforms();

        return hitFound;
    }

    // TODO make precision check work.
    /// <summary>
    /// Raycast at a specific time using custom (LC) Lag Compensation
    /// </summary>
    /// <returns></returns>
    public bool RaycastLC(Ray ray, int tick, out LCHitInfo hitInfo, float range, HitColliderCollection exclude = null)
    {
        bool hitFound = false;

        hitInfo = LCHitInfo.Zero;

        //ColliderCastSystem.Simulate(tick, exclude);

        // For some reason tick - 1 gives the least error idk and not sure why XD
        // but who cares if it's the best just using it for now, if you know something better don't hesitate to let me know.
        int simTick = tick - 1;

        ColliderCastSystem.Simulate(simTick);

        //if (ColliderCastSystem.ColliderCastFromCachedData(ray.origin, ray.direction, range, out ColliderCastHit ccHit, out HitColliderCollection hitCollection, out int index))
        if (ColliderCastSystem.ColliderCastTransformWithExclusion(ray.origin, ray.direction, range, out ColliderCastHit ccHit, out HitColliderCollection hitCollection, out int index, exclude, true))
        {
            hitInfo.CCHit = ccHit;
            hitInfo.HitColliderCollection = hitCollection;
            hitInfo.HitColliderIndex = index;

            HitColliderGeneric hitCol = hitCollection.GetHitColliderAtIndex(index);

            SendClientHitObjectDataRpc(hitCol.GetCachedTRSData().position, hitCol.GetCachedTRSData().rotation, true, tick);


            // TODO etc
            // Manage Draw loops
            //if (_currentDrawLoopCoroutine != null) StopCoroutine(_currentDrawLoopCoroutine);

            //_currentDrawLoopCoroutine = StartCoroutine(DrawAllCollidersAtCachedTransformsLoop());



            hitFound = true;
        }

        return hitFound;
    }

    //private Coroutine _currentDrawLoopCoroutine;

    //private IEnumerator DrawAllCollidersAtCachedTransformsLoop()
    //{
    //    float time = 10;

    //    while (time > 0)
    //    {
    //        ColliderCastSystem.DebugDrawColliders();

    //        time -= Time.deltaTime;

    //        yield return null;
    //    }
    //}

    /// <summary>
    /// Send your shot data using this. TODO try to avoid using RPC
    /// </summary>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    /// <param name="isServer"></param>
    /// <param name="shotId"></param>
    [Rpc(RpcPeers.Everyone, RpcPeers.Owner, true)]
    public void SendClientHitObjectDataRpc(Vector3 position, Quaternion rotation, bool isServer, int shotId)
    {
#if UNITY_EDITOR
        Debug.Log("Send hit object data from: is server " + isServer + " Shot tick" + shotId);
#endif

        if (isServer)
        {
            CreateNewLCCube(position, rotation, isServer);

            serverHits[shotId] = new TransformFrameData(position, rotation);

            //if (_comparisonCubes)
            //{
            //    NetworkObject cubeObj = Sandbox.NetworkInstantiate(_serverCubeLagComp, position, rotation);

            //    // TODO destroy after time.
            //    cubeObj.TryGetComponent(out AutoDestroy autoDestroy);

            //    autoDestroy?.Begin(_cubeCycleDuration);
            //}

            //serverHits[shotId] = position;
        }
        else
        {
            CreateNewLCCube(position, rotation, isServer);

            clientHits[shotId] = new TransformFrameData(position, rotation);

            //if (_comparisonCubes) Sandbox.NetworkInstantiate(_clientCubeLagComp, position, rotation);

            //clientHits[shotId] = position;
        }

        if (clientHits.TryGetValue(shotId, out var clientTRS) &&
            serverHits.TryGetValue(shotId, out var serverTRS))
        {
            float distance = Vector3.Distance(clientTRS.position, serverTRS.position);
            float angle = Quaternion.Angle(clientTRS.rotation, serverTRS.rotation);

#if UNITY_EDITOR
            if (_logData)
            {
                Debug.Log($"[LagComp] Shot {shotId}: position error {distance:F4}m");
                Debug.Log($"[LagComp] Shot {shotId}: rotation error {angle:F4}°");
            }
#endif

            clientHits.Remove(shotId);
            serverHits.Remove(shotId);
        }
    }

    /// <summary>
    /// Creates new cube if allowed
    /// </summary>
    /// <param name="hits"></param>
    /// <returns></returns>
    private void CreateNewLCCube(Vector3 position, Quaternion rotation, bool isServer)
    {
        if (!_comparisonCubes) return;

        NetworkObject cubeObj = Sandbox.NetworkInstantiate(isServer ? _serverCubeLagComp : _clientCubeLagComp, position, rotation);

        cubeObj.TryGetComponent(out AutoDestroy autoDestroy);

        autoDestroy?.Begin(_cubeCycleDuration);
    }
    

    //public bool Raycast(int tick, Vector3 position, Vector3 direction, float length)
    //{
    //    return RaycastLocal(tick, position, direction, length);
    //}

    //public bool RaycastLocal(int tick, Vector3 position, Vector3 direction, float length)
    //{
    //    Debug.Log("Tick difference " + (Sandbox.Tick - tick));

    //    return true;
    //}
}