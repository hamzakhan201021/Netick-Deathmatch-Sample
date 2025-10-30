using Netick;
using Netick.Unity;
using UnityEngine;

public class PlayerCharacterVisual : NetworkBehaviour
{
    [Networked] public Color MeshColor { get; set; }

    public MeshRenderer meshRenderer;

    public override void NetworkFixedUpdate()
    {
        if (FetchInput(out PlayerCharacterInput input))
        {
            if (input.RandomizeColor)
                MeshColor = Random.ColorHSV(0f, 1f);
        }
    }

    [OnChanged(nameof(MeshColor))]
    private void OnColorChanged(OnChangedData onChangedData)
    {
        meshRenderer.material.color = MeshColor;
    }
}