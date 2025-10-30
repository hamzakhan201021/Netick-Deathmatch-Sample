using UnityEngine;

public class FrameRateDebugger : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text _frameRateText;

    private void Start()
    {
        InvokeRepeating(nameof(UpdateDisplay), 0, 0.4f);
    }

    void UpdateDisplay()
    {
        float fps = 1.0f / Time.deltaTime;

        _frameRateText.text = $"Frame rate {fps}";
    }
}
