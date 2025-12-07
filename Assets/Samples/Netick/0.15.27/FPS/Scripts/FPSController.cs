using UnityEngine;
using Netick;
using Netick.Unity;
using UnityEngine.InputSystem;

namespace Netick.Samples.FPS
{
  public class FPSController : NetworkBehaviour
  {
    #region Original, Not so smooth
    // [SerializeField]
    // private Transform           _renderTransform;

    // [SerializeField]
    // private float               _movementSpeed = 10;
    // [SerializeField]
    // private float               _sensitivityX = 1.6f;
    // [SerializeField]
    // private float               _sensitivityY = -1f;
    // [SerializeField]
    // private Transform           _cameraParent;
    // private CharacterController _CC;
    // private Vector2             _camAngles;
    // private FPSInput            _lastInput;

    // // Networked Properties
    // [Networked]
    // [Smooth]
    // public Vector2              YawPitch { get; set; }

    // public override void NetworkStart()
    // {
    //   _CC = GetComponent<CharacterController>();

    //   if (IsInputSource)
    //   {
    //     var cam = Sandbox.FindObjectOfType<Camera>();
    //     cam.transform.parent = _cameraParent;
    //     cam.transform.localPosition = Vector3.zero;
    //     cam.transform.localRotation = Quaternion.identity;

    //     Cursor.visible = false;
    //     Cursor.lockState = CursorLockMode.Locked;
    //   }
    // }

    // public override void OnInputSourceLeft()
    // {
    //   // destroy the player object when its input source (controller player) leaves the game.
    //   Sandbox.Destroy(Object);
    // }

    // public override void NetworkUpdate()
    // {
    //   if (!IsInputSource || !Sandbox.InputEnabled)
    //     return;

    //   Vector2 mouseInputs = new Vector2(Input.GetAxisRaw("Mouse X") * _sensitivityX, Input.GetAxisRaw("Mouse Y") * _sensitivityY);

    //   var networkInput = Sandbox.GetInput<FPSInput>();
    //   networkInput.Movement = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    //   networkInput.ShootInput |= Input.GetMouseButton(0);
    //   networkInput.YawPitch += mouseInputs;
    //   Sandbox.SetInput(networkInput);

    //   // we apply the rotation in update too to have smooth camera control.
    //   _camAngles = ClampAngles(_camAngles.x + mouseInputs.x, _camAngles.y + mouseInputs.y);
    //   ApplyRotations(_camAngles, false);
    // }

    // public override void NetworkFixedUpdate()
    // {
    //   FetchInput(out _lastInput);

    //   if (IsInputSource || IsServer)
    //     MoveAndRotate(_lastInput);
    // }

    // private void MoveAndRotate(FPSInput input)
    // {
    //   // clamp movement inputs. 
    //   input.Movement = new Vector3(Mathf.Clamp(input.Movement.x, -1f, 1f), Mathf.Clamp(input.Movement.y, -1f, 1f));

    //   // rotation.
    //   YawPitch = ClampAngles(YawPitch.x + input.YawPitch.x, YawPitch.y + input.YawPitch.y);
    //   ApplyRotations(YawPitch, false);

    //   // movement direction.
    //   var movement = transform.TransformVector(new Vector3(input.Movement.x, 0, input.Movement.y)) * _movementSpeed;
    //   movement.y = 0;

    //   var gravity = 15f * Vector3.down;

    //   // move.
    //   _CC.Move((movement + gravity) * Sandbox.FixedDeltaTime);
    // }


    // [OnChanged(nameof(YawPitch), invokeDuringResimulation: true)]
    // private void OnYawPitchChanged(OnChangedData onChanged)
    // {
    //   ApplyRotations(YawPitch, false);
    // }

    // public override void NetworkRender()
    // {
    //   if (!IsInputSource)
    //     ApplyRotations(YawPitch, true);
    // }

    // private void ApplyRotations(Vector2 camAngles, bool isProxy)
    // {
    //   // on the player transform, we apply yaw.
    //   if (isProxy)
    //     _renderTransform.rotation = Quaternion.Euler(new Vector3(0, camAngles.x, 0));
    //   else
    //     transform.rotation = Quaternion.Euler(new Vector3(0, camAngles.x, 0));

    //   // on the weapon/camera holder, we apply the pitch angle.
    //   _cameraParent.localEulerAngles = new Vector3(camAngles.y, 0, 0);
    //   _camAngles = camAngles;
    // }

    // private Vector2 ClampAngles(float yaw, float pitch)
    // {
    //   return new Vector2(ClampAngle(yaw, -360, 360), ClampAngle(pitch, -80, 80));
    // }

    // private float ClampAngle(float angle, float min, float max)
    // {
    //   if (angle < -360F)
    //     angle += 360F;
    //   if (angle > 360F)
    //     angle -= 360F;
    //   return Mathf.Clamp(angle, min, max);
    // }
    #endregion





    // Smooth fps controller
    #region Uses Old input system
    // [SerializeField]
    // private Transform _renderTransform;

    // [SerializeField]
    // private float _movementSpeed = 10;
    // [SerializeField]
    // private float _sensitivityX = 1.6f;
    // [SerializeField]
    // private float _sensitivityY = -1f;

    // [SerializeField]
    // private float _lookSmoothTime = 0.04f;

    // [SerializeField]
    // private Transform _cameraParent;

    // private CharacterController _CC;
    // private Vector2 _camAngles;
    // private Vector2 _smoothedLook;
    // private Vector2 _lookVel;
    // private FPSInput _lastInput;

    // [Networked]
    // [Smooth]
    // public Vector2 YawPitch { get; set; }

    // public override void NetworkStart()
    // {
    //   _CC = GetComponent<CharacterController>();

    //   if (IsInputSource)
    //   {
    //     var cam = Sandbox.FindObjectOfType<Camera>();
    //     cam.transform.parent = _cameraParent;
    //     cam.transform.localPosition = Vector3.zero;
    //     cam.transform.localRotation = Quaternion.identity;
    //   }
    // }

    // public override void OnInputSourceLeft()
    // {
    //   Sandbox.Destroy(Object);
    // }

    // public override void NetworkUpdate()
    // {
    //   if (!IsInputSource || !Sandbox.InputEnabled)
    //     return;

    //   Vector2 raw = new Vector2(
    //       Input.GetAxis("Mouse X") * _sensitivityX,
    //       Input.GetAxis("Mouse Y") * _sensitivityY
    //   );

    //   _smoothedLook = Vector2.SmoothDamp(
    //       _smoothedLook,
    //       raw,
    //       ref _lookVel,
    //       _lookSmoothTime
    //   );

    //   var networkInput = Sandbox.GetInput<FPSInput>();
    //   networkInput.Movement = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    //   networkInput.ShootInput |= Input.GetMouseButton(0);
    //   networkInput.YawPitch += _smoothedLook;
    //   Sandbox.SetInput(networkInput);

    //   _camAngles = ClampAngles(
    //       _camAngles.x + _smoothedLook.x,
    //       _camAngles.y + _smoothedLook.y
    //   );

    //   ApplyRotations(_camAngles, false);
    // }

    // public override void NetworkFixedUpdate()
    // {
    //   FetchInput(out _lastInput);

    //   if (IsInputSource || IsServer)
    //     MoveAndRotate(_lastInput);
    // }

    // private void MoveAndRotate(FPSInput input)
    // {
    //   input.Movement = new Vector3(
    //       Mathf.Clamp(input.Movement.x, -1f, 1f),
    //       Mathf.Clamp(input.Movement.y, -1f, 1f)
    //   );

    //   YawPitch = ClampAngles(
    //       YawPitch.x + input.YawPitch.x,
    //       YawPitch.y + input.YawPitch.y
    //   );

    //   ApplyRotations(YawPitch, false);

    //   var movement = transform.TransformVector(new Vector3(input.Movement.x, 0, input.Movement.y)) * _movementSpeed;
    //   movement.y = 0;

    //   var gravity = 15f * Vector3.down;

    //   _CC.Move((movement + gravity) * Sandbox.FixedDeltaTime);
    // }

    // [OnChanged(nameof(YawPitch), invokeDuringResimulation: true)]
    // private void OnYawPitchChanged(OnChangedData onChanged)
    // {
    //   ApplyRotations(YawPitch, false);
    // }

    // public override void NetworkRender()
    // {
    //   if (!IsInputSource)
    //     ApplyRotations(YawPitch, true);
    // }

    // private void ApplyRotations(Vector2 camAngles, bool isProxy)
    // {
    //   if (isProxy)
    //     _renderTransform.rotation = Quaternion.Euler(0, camAngles.x, 0);
    //   else
    //     transform.rotation = Quaternion.Euler(0, camAngles.x, 0);

    //   _cameraParent.localEulerAngles = new Vector3(camAngles.y, 0, 0);
    //   _camAngles = camAngles;
    // }

    // private Vector2 ClampAngles(float yaw, float pitch)
    // {
    //   return new Vector2(
    //       ClampAngle(yaw, -360, 360),
    //       ClampAngle(pitch, -80, 80)
    //   );
    // }

    // private float ClampAngle(float angle, float min, float max)
    // {
    //   if (angle < -360F) angle += 360F;
    //   if (angle > 360F) angle -= 360F;
    //   return Mathf.Clamp(angle, min, max);
    // }
    #endregion

    #region Uses new input system
    // [SerializeField]
    // private Transform _renderTransform;

    // [SerializeField]
    // private float _movementSpeed = 10;
    // [SerializeField]
    // private float _sensitivityX = 1.6f;
    // [SerializeField]
    // private float _sensitivityY = -1f;

    // [SerializeField]
    // private float _lookSmoothTime = 0.04f;

    // [SerializeField]
    // private Transform _cameraParent;

    // private CharacterController _CC;
    // private Vector2 _camAngles;
    // private Vector2 _smoothedLook;
    // private Vector2 _lookVel;
    // private FPSInput _lastInput;

    // [Networked]
    // [Smooth]
    // public Vector2 YawPitch { get; set; }

    // private InputSystem_Actions _inputActions;

    // public override void NetworkStart()
    // {
    //   _CC = GetComponent<CharacterController>();

    //   if (IsInputSource)
    //   {
    //     var cam = Sandbox.FindObjectOfType<Camera>();
    //     cam.transform.parent = _cameraParent;
    //     cam.transform.localPosition = Vector3.zero;
    //     cam.transform.localRotation = Quaternion.identity;

    //     // New uses new input system
    //     _inputActions = new InputSystem_Actions();
    //     _inputActions.Enable();
    //   }
    // }

    // public override void OnInputSourceLeft()
    // {
    //   Sandbox.Destroy(Object);
    // }

    // public override void NetworkUpdate()
    // {
    //   if (!IsInputSource || !Sandbox.InputEnabled)
    //     return;

    //   // Vector2 raw = new Vector2(
    //   //     Input.GetAxis("Mouse X") * _sensitivityX,
    //   //     Input.GetAxis("Mouse Y") * _sensitivityY
    //   // );
    //   Vector2 mouseInput = _inputActions.Player.Look.ReadValue<Vector2>();
    //   Vector2 raw = new Vector2(mouseInput.x * _sensitivityX, mouseInput.y * _sensitivityY);

    //   _smoothedLook = Vector2.SmoothDamp(
    //       _smoothedLook,
    //       raw,
    //       ref _lookVel,
    //       _lookSmoothTime
    //   );

    //   var networkInput = Sandbox.GetInput<FPSInput>();
    //   // networkInput.Movement = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    //   networkInput.Movement = _inputActions.Player.Move.ReadValue<Vector2>();
    //   // networkInput.ShootInput |= Input.GetMouseButton(0);
    //   networkInput.YawPitch += _smoothedLook;
    //   Sandbox.SetInput(networkInput);


    //   _camAngles = ClampAngles(
    //       _camAngles.x + _smoothedLook.x,
    //       _camAngles.y + _smoothedLook.y
    //   );

    //   ApplyRotations(_camAngles, false);
    // }

    // public override void NetworkFixedUpdate()
    // {
    //   FetchInput(out _lastInput);

    //   if (IsInputSource || IsServer)
    //     MoveAndRotate(_lastInput);
    // }

    // private void MoveAndRotate(FPSInput input)
    // {
    //   input.Movement = new Vector3(
    //       Mathf.Clamp(input.Movement.x, -1f, 1f),
    //       Mathf.Clamp(input.Movement.y, -1f, 1f)
    //   );

    //   YawPitch = ClampAngles(
    //       YawPitch.x + input.YawPitch.x,
    //       YawPitch.y + input.YawPitch.y
    //   );

    //   ApplyRotations(YawPitch, false);

    //   var movement = transform.TransformVector(new Vector3(input.Movement.x, 0, input.Movement.y)) * _movementSpeed;
    //   movement.y = 0;

    //   var gravity = 15f * Vector3.down;

    //   _CC.Move((movement + gravity) * Sandbox.FixedDeltaTime);
    // }

    // [OnChanged(nameof(YawPitch), invokeDuringResimulation: true)]
    // private void OnYawPitchChanged(OnChangedData onChanged)
    // {
    //   ApplyRotations(YawPitch, false);
    // }

    // public override void NetworkRender()
    // {
    //   if (!IsInputSource)
    //     ApplyRotations(YawPitch, true);
    // }

    // private void ApplyRotations(Vector2 camAngles, bool isProxy)
    // {
    //   if (isProxy)
    //     _renderTransform.rotation = Quaternion.Euler(0, camAngles.x, 0);
    //   else
    //     transform.rotation = Quaternion.Euler(0, camAngles.x, 0);

    //   _cameraParent.localEulerAngles = new Vector3(camAngles.y, 0, 0);
    //   _camAngles = camAngles;
    // }

    // private Vector2 ClampAngles(float yaw, float pitch)
    // {
    //   return new Vector2(
    //       ClampAngle(yaw, -360, 360),
    //       ClampAngle(pitch, -80, 80)
    //   );
    // }

    // private float ClampAngle(float angle, float min, float max)
    // {
    //   if (angle < -360F) angle += 360F;
    //   if (angle > 360F) angle -= 360F;
    //   return Mathf.Clamp(angle, min, max);
    // }















    // Best version with perfect look and new input system
    [SerializeField] private Transform _renderTransform;
    [SerializeField] private float _movementSpeed = 10;
    [SerializeField] private float _sensitivityX = 1.6f;
    [SerializeField] private float _sensitivityY = -1f;
    [SerializeField] private float _lookSmoothTime = 0.04f;
    [SerializeField] private Transform _cameraParent;

    [SerializeField] private bool useAcceleration = false;
    [SerializeField] private AnimationCurve accelerationCurve = AnimationCurve.Linear(0, 1, 20, 3);

    private CharacterController _CC;
    private Vector2 _camAngles;
    private Vector2 _smoothedLook;
    private Vector2 _lookVel;
    private FPSInput _lastInput;

    [Networked][Smooth] public Vector2 YawPitch { get; set; }

    private InputSystem_Actions _inputActions;

    private bool _cursorLocked;

    public override void NetworkStart()
    {
      _CC = GetComponent<CharacterController>();
      if (IsInputSource)
      {
        // Camera
        var cam = Sandbox.FindObjectOfType<Camera>();
        cam.transform.parent = _cameraParent;
        cam.transform.localPosition = Vector3.zero;
        cam.transform.localRotation = Quaternion.identity;

        // Input
        _inputActions = new InputSystem_Actions();
        _inputActions.Enable();

        _cursorLocked = true;
        SetCursor(_cursorLocked);
      }
    }

    public override void OnInputSourceLeft()
    {
      Sandbox.Destroy(Object);
    }

    public override void NetworkUpdate()
    {
      if (!IsInputSource || !Sandbox.InputEnabled)
        return;

      // Manage cursor
      if (_inputActions.Player.Escape.triggered)
      {
        // Toggle cursor
        _cursorLocked = !_cursorLocked;
        SetCursor(_cursorLocked);

        // if (_cursorLocked)
        // {
        //   Cursor.lockState = CursorLockMode.Locked;
        //   Cursor.visible = false;
        // }
        // else
        // {
        //   Cursor.lockState = CursorLockMode.None;
        //   Cursor.visible = true;
        // }
      }

      // Update inputs
      bool currentMouse = _inputActions.Player.Look.activeControl?.device is UnityEngine.InputSystem.Mouse;

      float deltaMultiplier = currentMouse ? 1 : Sandbox.DeltaTime;

      Vector2 rawDelta = _cursorLocked ? _inputActions.Player.Look.ReadValue<Vector2>() * deltaMultiplier : Vector2.zero;
      Vector2 raw = new Vector2(rawDelta.x * _sensitivityX, rawDelta.y * _sensitivityY);

      if (useAcceleration)
      {
        float speed = raw.magnitude;
        float scale = accelerationCurve.Evaluate(speed);
        raw *= scale;
      }

      _smoothedLook = Vector2.SmoothDamp(_smoothedLook, raw, ref _lookVel, _lookSmoothTime);
      // float smoothing = 1 - Mathf.Exp(-_lookSmoothTime * Sandbox.DeltaTime);
      // _smoothedLook = Vector2.Lerp(_smoothedLook, raw, smoothing);

      var networkInput = Sandbox.GetInput<FPSInput>();
      networkInput.Movement = _inputActions.Player.Move.ReadValue<Vector2>();
      networkInput.YawPitch += _smoothedLook;
      Sandbox.SetInput(networkInput);

      _camAngles = ClampAngles(_camAngles.x + _smoothedLook.x, _camAngles.y + _smoothedLook.y);
      ApplyRotations(_camAngles, false);
    }



    private void SetCursor(bool locked)
    {
      Cursor.visible = !locked;
      Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
    }

    public override void NetworkFixedUpdate()
    {
      FetchInput(out _lastInput, out bool isDuplicated);
      if (IsInputSource || IsServer)
        MoveAndRotate(_lastInput, isDuplicated);
    }

    private void MoveAndRotate(FPSInput input, bool isDuplicated)
    {
      input.Movement = new Vector3(
          Mathf.Clamp(input.Movement.x, -1f, 1f),
          Mathf.Clamp(input.Movement.y, -1f, 1f)
      );

      if (!isDuplicated)
      {
        YawPitch = ClampAngles(YawPitch.x + input.YawPitch.x, YawPitch.y + input.YawPitch.y);
        ApplyRotations(YawPitch, false);
      }


      var movement = transform.TransformVector(new Vector3(input.Movement.x, 0, input.Movement.y)) * _movementSpeed;
      movement.y = 0;
      var gravity = 15f * Vector3.down;

      _CC.Move((movement + gravity) * Sandbox.FixedDeltaTime);
    }

    [OnChanged(nameof(YawPitch), invokeDuringResimulation: true)]
    private void OnYawPitchChanged(OnChangedData onChanged)
    {
      ApplyRotations(YawPitch, false);
    }

    public override void NetworkRender()
    {
      if (!IsInputSource)
        ApplyRotations(YawPitch, true);
    }

    private void ApplyRotations(Vector2 camAngles, bool isProxy)
    {
      if (isProxy)
        _renderTransform.rotation = Quaternion.Euler(0, camAngles.x, 0);
      else
        transform.rotation = Quaternion.Euler(0, camAngles.x, 0);

      _cameraParent.localEulerAngles = new Vector3(camAngles.y, 0, 0);
      _camAngles = camAngles;
    }

    private Vector2 ClampAngles(float yaw, float pitch)
    {
      return new Vector2(ClampAngle(yaw, -360, 360), ClampAngle(pitch, -80, 80));
    }

    private float ClampAngle(float angle, float min, float max)
    {
      if (angle < -360F) angle += 360F;
      if (angle > 360F) angle -= 360F;
      return Mathf.Clamp(angle, min, max);
    }
    #endregion

  }
}

