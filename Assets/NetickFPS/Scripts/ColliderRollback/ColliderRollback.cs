using System.Collections.Generic;
using Netick.Unity;
using UnityEngine;

public class ColliderRollback : NetworkBehaviour
{
    [SerializeField] private float _storeHistoryInSeconds = 5;

    [Header("Optional field to help identify root")]
    public Transform RootTransform;

    //private Collider3DState[] _history;
    //public Collider Collider { get; private set; }
    [HideInInspector] public Collider Collider;

    private bool _registered = false;

    //private int _historyLength;

    //public override void NetworkStart()
    //{
    //    //Collider = GetComponent<Collider>();


    //}

    public override void NetworkStart()
    {
        if (IsServer)
        {
            //Collider = GetComponent<Collider>();

            //int historyLength = Mathf.CeilToInt(_storeHistoryInSeconds * Sandbox.Config.TickRate);

            //RollbackModule.Instance.Register(this, historyLength);
            //RollbackModule.Instance.OnReady += OnRollbackModuleReady;
            OnRollbackModuleReady();
        }
    }

    public override void NetworkDestroy()
    {
        if (IsServer)
        {
            if (_registered)
            {
                Sandbox.GetComponent<LagCompensationManager>().Unregister(this);
            }

            //RollbackModule.Instance.OnReady -= OnRollbackModuleReady;
        }
    }

    public void OnRollbackModuleReady()
    {
        Collider = GetComponent<Collider>();

        int historyLength = Mathf.CeilToInt(_storeHistoryInSeconds * Sandbox.Config.TickRate);

        //LagCompensationManager.Instance.Register(this, historyLength);
        Sandbox.GetComponent<LagCompensationManager>().Register(this, historyLength);

        _registered = true;
    }
}