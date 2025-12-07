using System.Collections.Generic;
using FpsAnimatonDev;
using UnityEngine;

public class FpsAnimator : MonoBehaviour
{

    [SerializeField] private FpsPlayer _playerMain;

    [System.Serializable]
    public class RotationData
    {
        public Transform Constraint;
        public Vector3 Offset;
        [Range(0, 1)] public float Weight = 1;
    }

    [SerializeField] private List<RotationData> _rotationData;

    // [SerializeField] private float _pitch;

    // Update is called once per frame
    void Update()
    {
        Vector3 localEuler = GetLatestRotation();
        localEuler.y = 0;
        // smoothed = Mathf.Lerp(smoothed, localEuler.x, _speed * Time.deltaTime);
        // localEuler.x = smoothed;

        for (int i = 0; i < _rotationData.Count; i++)
        {
            // TODO NEW without smoothing

            Quaternion rawTargetRot = Quaternion.Euler(localEuler) * Quaternion.Euler(_rotationData[i].Offset);
            Quaternion targetRot = Quaternion.Lerp(Quaternion.identity, rawTargetRot, _rotationData[i].Weight);

            _rotationData[i].Constraint.localRotation = targetRot;
        }
    }

    private Vector3 GetLatestRotation()
    {
        return new Vector3(_playerMain.Pitch, transform.eulerAngles.y, 0);
    }
}
