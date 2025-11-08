using System.Collections.Generic;
using Netick.Unity;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerCharacter : NetworkBehaviour
{
    [Header("Head Renderer Objects")]
    [SerializeField] private List<Renderer> _headRenderers;

    public override void NetworkStart()
    {
        for (int i = 0; i < _headRenderers.Count; i++)
        {
            //_headRenderers[i].shadowCastingMode = ShadowCastingMode.ShadowsOnly;
            SetShadowCastMode(_headRenderers[i]);
        }
    }

    private void SetShadowCastMode(Renderer renderer)
    {
        if (IsInputSource)
        {
            renderer.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
        }
        else
        {
            renderer.shadowCastingMode = ShadowCastingMode.On;
        }
    }
}