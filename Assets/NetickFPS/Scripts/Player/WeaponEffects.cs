using UnityEngine;
using Netick.Unity;
using Netick;
using System;

public class WeaponEffects : NetworkBehaviour
{

    // Weapon Effects (Adopted from HK FPS, See link in readme in case you want to XD) the script gun animator and weaponbase





    [Header("Headbob Settings")]
    [SerializeField] private HeadBobSettings _headBobSettings;

    [Header("Sway and Recoil")]
    [SerializeField] private float _returnSpeed = 10;
    [SerializeField] private float _snappiness = 5;
    [SerializeField] private float _swayAmount = 10;
    [SerializeField] private float _swaySmooth = 10;
    [SerializeField] private float _swayMaxAngle = 10;


    private Vector3 _startPos;
    private Vector3 _startRot;

    [Networked, Smooth] public Vector3 CurrentRotation { get; set; }
    [Networked, Smooth] public Vector3 TargetRotation { get; set; }

    public override void NetworkStart()
    {
        _startPos = transform.localPosition;
        _startRot = transform.localRotation.eulerAngles;
    }

    public override void NetworkFixedUpdate()
    {
        if (FetchInput(out PlayerInput input))
        {
            UpdateHeadBob();
            UpdateRecoilAndSway(input.MouseInput);
        }
    }

    // TODO Add headbobbing
    private void UpdateHeadBob()
    {

    }

    //private void UpdateRecoilAndSway(Vector2 lookInput)
    //{
    //    // Lerp the target rotation to zero.
    //    TargetRotation = Vector3.Lerp(TargetRotation, Vector3.zero, _returnSpeed * Time.deltaTime);

    //    // Slerp the current rotation to the target rotation.
    //    CurrentRotation = Vector3.Slerp(CurrentRotation, TargetRotation, _snappiness * Time.deltaTime);

    //    //CurrentRotation = ClampZ(CurrentRotation, -_swayMaxAngle, _swayMaxAngle);

    //    // CHANGES (
    //    //float lookX = _controller.Input.Player.Look.ReadValue<Vector2>().x * WeaponData.SwayAmount;
    //    //float lookY = _controller.Input.Player.Look.ReadValue<Vector2>().y * WeaponData.SwayAmount;

    //    float lookX = lookInput.x * _swayAmount;
    //    float lookY = lookInput.y * _swayAmount;
    //    // CHANGES )

    //    Quaternion swayRotation;
    //    Quaternion finalRot;

    //    // CHANGES (
    //    //float currentLookUpLimit = -_controller.GetLookUpLimit();
    //    //float currentLookDownLimit = _controller.GetLookDownLimit();

    //    //float currentLookUpLimit = -lookUpLimit;
    //    //float currentLookDownLimit = lookDownLimit;
    //    // CHANGES )

    //    // CHANGES OLD CODE HERE IS COMMENTED OUT.
    //    //if (_controller.GetXRotation() > (currentLookDownLimit - 1) || _controller.GetXRotation() < (currentLookUpLimit + 1))
    //    //if (xRotation > (currentLookDownLimit - 1) || xRotation < (currentLookUpLimit + 1))
    //    //{
    //    //    finalRot = Quaternion.AngleAxis(-lookX, Vector3.forward);
    //    //}
    //    //else
    //    //{
    //        finalRot = Quaternion.AngleAxis(-lookY, Vector3.right) * Quaternion.AngleAxis(-lookX, Vector3.forward);
    //    //}

    //    swayRotation = Quaternion.Slerp(transform.localRotation, finalRot *
    //            Quaternion.Euler(CurrentRotation), Time.deltaTime * _swaySmooth);

    //    transform.localRotation = swayRotation;
    //}
    private void UpdateRecoilAndSway(Vector2 lookInput)
    {
        TargetRotation = Vector3.Lerp(TargetRotation, Vector3.zero, _returnSpeed * Sandbox.FixedDeltaTime);
        CurrentRotation = Vector3.Slerp(CurrentRotation, TargetRotation, _snappiness * Sandbox.FixedDeltaTime);

        float lookX = lookInput.x * _swayAmount;
        float lookY = lookInput.y * _swayAmount;

        // ADDED: Clamp sway to prevent over-rotation
        lookX = Mathf.Clamp(lookX, -_swayMaxAngle, _swayMaxAngle);
        lookY = -Mathf.Clamp(lookY, -_swayMaxAngle, _swayMaxAngle);
        // END ADDED

        Quaternion swayRotation;
        Quaternion finalRot;

        finalRot = Quaternion.AngleAxis(-lookY, Vector3.right) * Quaternion.AngleAxis(-lookX, Vector3.forward);

        swayRotation = Quaternion.Slerp(transform.localRotation, finalRot *
                Quaternion.Euler(CurrentRotation), Sandbox.FixedDeltaTime * _swaySmooth);

        transform.localRotation = swayRotation;
    }


    public void AddBump(float bump)
    {

    }

    public void AddRecoil(Vector3 recoil)
    {
        TargetRotation += recoil;
    }

}

[Serializable]
public class HeadBobSettings
{
    [Header("Bobbing Settings")]
    public float WalkBobAmount = 0.0035f;
    public float WalkBobSpeed = 14f;
    [Space(5)]
    public float SprintBobAmount = 0.015f;
    public float SprintBobSpeed = 20f;
    [Space(5)]
    public float CrouchBobAmount = 0.004f;
    public float CrouchBobSpeed = 12f;
    [Space(5)]
    public float Smoothing = 8f;
    public float XTiltMulti = 30f;
    [Space(5)]
    public float WalkXMulti = 1.6f;
    public float WalkYMulti = 1.4f;
    [Space(5)]
    public float SprintXMulti = 1.2f;
    public float SprintYMulti = 1.4f;
    public float SprintZMulti = 1.4f;
    [Space(5)]
    public float CrouchXMulti = 1.6f;
    public float CrouchYMulti = 1.4f;
    [Space(5)]
    public Vector3 SprintRotation = new Vector3(10, -10, 0);
    public Vector3 SprintPositionOffset = Vector3.zero;
}