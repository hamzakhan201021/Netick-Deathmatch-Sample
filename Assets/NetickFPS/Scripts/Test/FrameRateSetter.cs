using UnityEngine;

public class FrameRateSetter : MonoBehaviour
{
    public int targetFPS = 30;

    void Start()
    {
        Application.targetFrameRate = targetFPS;
        // QualitySettings.vSyncCount = 0;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Application.targetFrameRate = 30;
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Application.targetFrameRate = 60;
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Application.targetFrameRate = 120;
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            Application.targetFrameRate = 240;
        }
    }
}
