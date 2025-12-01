using System.Collections.Generic;
using Netick;
using Netick.Unity;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovementController : NetworkedCharacterController
{

    [Header("Weapon")]
    [SerializeField] private WeaponEffects _weaponEffects;
    [SerializeField] private float _jumpEffectIntensity = 0.1f;
    [SerializeField] private float _landEffectIntensity = -0.1f;

    [Header("Stable Movement")]
    [SerializeField] private float WalkingSpeed = 2.5f;
    [SerializeField] private float SprintMultiplier = 2f;
    [SerializeField] private float AccelerationRate = 25f;

    [Header("Air Movement")]
    [SerializeField] private float JumpStrength = 10;
    [SerializeField] private float GravityAcceleration = -9.81f;
    //[SerializeField] private float GravityMultiplier = 2;

    [Header("Look")]
    [SerializeField] private float _sensX = 1;
    [SerializeField] private float _sensY = 1;
    // [SerializeField] private float _inputSmoothSpeed = 5;
    [SerializeField] private float _lookSmoothTime = 0.05f;
    [SerializeField] private bool _useAcceleration = false;
    [SerializeField] private AnimationCurve _accelerationCurve = AnimationCurve.Linear(0, 1, 20, 3);

    [Header("Player References")]
    [SerializeField] private Transform _cameraParent;
    [SerializeField] private Transform _renderTransform;

    [Header("For Testing Use")]
    [SerializeField] private bool _useAutoMover = false;
    [SerializeField] private GameObject _autoMoverUI;
    [SerializeField] private Toggle _autoMove;
    [SerializeField] private Toggle _directionAMove;
    // [Header("For Lag Comp Tests")]
    // [SerializeField] private Toggle _autoAimOnTarget;

    [Networked(relevancy: Relevancy.InputSource)] public Vector3 Velocity { get; set; }
    [Networked] public NetworkBool IsCrouching { get; set; }

    // Cursor
    private bool _cursorLocked;


    // Look
    private Vector2 _camAngles;
    private Vector2 _smoothedLook;
    private Vector2 _lookVel;
    [Networked][Smooth] public Vector2 YawPitch { get; set; }

    private PlayerInput _lastInput;

    private void UpdateCursorLock()
    {
        if (!Sandbox.InputEnabled || !IsInputSource)
            return;

        if (_cursorLocked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public override void OnInputSourceLeft()
    {
        // destroy the player object when its input source (controller player) leaves the game
        Sandbox.Destroy(Object);
    }

    private void Update()
    {
        if (!IsInputSource) return;

        if (Input.GetKeyDown(KeyCode.Escape)) _cursorLocked = !_cursorLocked;

        if (Sandbox.InputEnabled) UpdateCursorLock();
    }

    public override void NetworkStart()
    {
        InitializeComponent();

        if (IsInputSource)
        {
            var cam = Sandbox.FindObjectOfType<Camera>();
            cam.transform.parent = _cameraParent;
            cam.transform.localPosition = Vector3.zero;
            cam.transform.localRotation = Quaternion.identity;

            _cursorLocked = true;
            UpdateCursorLock();
        }
    }

    public override void NetworkUpdate()
    {
        if (!IsInputSource || !Sandbox.InputEnabled)
            return;



        #region OLD non organised


        //         if (_autoMoverUI != null) _autoMoverUI.SetActive(_useAutoMover);

        //         if (_autoMove != null && _autoMove.isOn && _useAutoMover)
        //         {
        //             networkInput.Movement = new Vector2(_directionAMove.isOn ? 1 : -1, 0);
        //         }
        //         else
        //         {
        //             networkInput.Movement = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        //         }


        //         mouseInputsYP = Vector2.Lerp(mouseInputsYP, new Vector2(Input.GetAxisRaw("Mouse X") * _sensX, -Input.GetAxisRaw("Mouse Y") * _sensY), _inputSmoothSpeed * Time.deltaTime);

        //         mouseInputSmooth = Vector2.Lerp(mouseInputSmooth, new Vector2(Input.GetAxisRaw("Mouse X"), -Input.GetAxisRaw("Mouse Y")), _inputSmoothSpeed * Time.deltaTime);

        //         //networkInput.MouseInput = Vector2.Lerp(, ,);
        //         networkInput.MouseInput = mouseInputSmooth;

        //         if (!_cursorLocked) mouseInputsYP = Vector2.zero;

        //         if (_autoAimOnTarget != null && _autoAimOnTarget.isOn)
        //         {
        //             // TODO improve targetting etc. (it's for testing anyways)

        //             if (_target == null)
        //             {
        //                 List<PlayerInputProvider> playerObjects = Sandbox.FindObjectsOfType<PlayerInputProvider>();

        //                 for (int i = 0; i < playerObjects.Count; i++)
        //                 {
        //                     if (playerObjects[i] != GetComponent<PlayerInputProvider>())
        //                     {
        //                         // this surely isn't our own player so just select it as target
        //                         _target = playerObjects[i].transform;
        //                     }
        //                 }
        //             }

        //             if (_target != null)
        //             {
        //                 Vector3 dir = (_target.position + new Vector3(0, 1, 0)) - _cameraParent.position;
        //                 dir.Normalize();

        //                 float yaw = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
        //                 float pitch = -Mathf.Asin(dir.y) * Mathf.Rad2Deg;

        //                 networkInput.YawPitch = new Vector2(yaw - 2, pitch);
        //             }
        //             else
        //             {
        //                 // Target is null.
        // #if UNITY_EDITOR
        //                 Debug.LogWarning("Auto aim couldn't find a target XD");
        // #endif
        //             }
        //         }
        //         else
        //         {
        //             networkInput.YawPitch += mouseInputsYP;
        //         }

        //         //networkInput.YawPitch += mouseInputs;

        //         networkInput.CrouchInput |= Input.GetKeyDown(KeyCode.C);
        //         networkInput.Sprinting = Input.GetKey(KeyCode.LeftShift);
        //         networkInput.JumpInput |= Input.GetKeyDown(KeyCode.Space);

        #endregion

        var networkInput = Sandbox.GetInput<PlayerInput>();

        networkInput = UpdateInputs(networkInput);
        networkInput = UpdateLookInput(networkInput);

        Sandbox.SetInput(networkInput);

        // we apply the rotation in update on the client to prevent look delay
        // _camAngles = ClampAngles(_camAngles.x + mouseInputsYP.x, _camAngles.y + mouseInputsYP.y);
        // ApplyRotations(_camAngles, false);

    }

    private PlayerInput UpdateInputs(PlayerInput networkInput)
    {
        if (_autoMoverUI != null) _autoMoverUI.SetActive(_useAutoMover);

        if (_autoMove != null && _autoMove.isOn && _useAutoMover)
        {
            networkInput.Movement = new Vector2(_directionAMove.isOn ? 1 : -1, 0);
        }
        else
        {
            networkInput.Movement = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        }

        networkInput.CrouchInput |= Input.GetKeyDown(KeyCode.C);
        networkInput.Sprinting = Input.GetKey(KeyCode.LeftShift);
        networkInput.JumpInput |= Input.GetKeyDown(KeyCode.Space);

        return networkInput;
    }

    // private PlayerInput UpdateLookInput(PlayerInput networkInput)
    // {



    //     // mouseInputsYP = Vector2.Lerp(mouseInputsYP, new Vector2(Input.GetAxisRaw("Mouse X") * _sensX, -Input.GetAxisRaw("Mouse Y") * _sensY), _inputSmoothSpeed * Time.deltaTime);

    //     // mouseInputSmooth = Vector2.Lerp(mouseInputSmooth, new Vector2(Input.GetAxisRaw("Mouse X"), -Input.GetAxisRaw("Mouse Y")), _inputSmoothSpeed * Time.deltaTime);

    //     //networkInput.MouseInput = Vector2.Lerp(, ,);
    //     // networkInput.MouseInput = mouseInputSmooth;

    //     // Calculate mouse input and apply rotation instantly...

    //     // Set the mouse input
    //     networkInput.MouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

    //     if (!_cursorLocked)
    //     {
    //         networkInput.MouseInput = Vector2.zero;
    //     }

    //     // Use mouse input to set the rotation
    //     // _xRotation += networkInput.MouseInput.y * Sandbox.DeltaTime;
    //     // _yRotation += networkInput.MouseInput.x * Sandbox.DeltaTime;

    //     // ApplyRotations(false);

    //     return networkInput;
    // }
    private PlayerInput UpdateLookInput(PlayerInput networkInput)
    {
        // bool currentMouse = _inputActions.Player.Look.activeControl?.device is UnityEngine.InputSystem.Mouse;

        // float deltaMultiplier = currentMouse ? 1 : Sandbox.DeltaTime;

        // Vector2 rawDelta = _cursorLocked ? _inputActions.Player.Look.ReadValue<Vector2>() * deltaMultiplier : Vector2.zero;
        // Vector2 raw = new Vector2(rawDelta.x * _sensitivityX, rawDelta.y * _sensitivityY);

        // if (useAcceleration)
        // {
        //     float speed = raw.magnitude;
        //     float scale = accelerationCurve.Evaluate(speed);
        //     raw *= scale;
        // }
        // bool currentMouse = _inputActions.Player.Look.activeControl?.device is UnityEngine.InputSystem.Mouse;

        // float deltaMultiplier = currentMouse ? 1 : Sandbox.DeltaTime;

        Vector2 rawDelta = _cursorLocked ? new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) : Vector2.zero;
        Vector2 raw = new Vector2(rawDelta.x * _sensX, rawDelta.y * _sensY);

        if (_useAcceleration)
        {
            float speed = raw.magnitude;
            float scale = _accelerationCurve.Evaluate(speed);
            raw *= scale;
        }

        _smoothedLook = Vector2.SmoothDamp(_smoothedLook, raw, ref _lookVel, _lookSmoothTime);

        networkInput.YawPitch += _smoothedLook;

        _camAngles = ClampAngles(_camAngles.x + _smoothedLook.x, _camAngles.y + _smoothedLook.y);
        ApplyRotations(_camAngles, false);

        return networkInput;
    }


    public override void NetworkFixedUpdate()
    {
        #region Movement has been moved to Handle Movement
        // Vector3 targetVelocity = Vector3.zero;
        // bool didJump = false;

        // if (FetchInput(out PlayerInput input))
        // {
        //     if (_autoAimOnTarget.isOn)
        //     {
        //         YawPitch = ClampAngles(input.YawPitch.x, input.YawPitch.y);
        //     }
        //     else
        //     {
        //         YawPitch = ClampAngles(YawPitch.x + input.YawPitch.x, YawPitch.y + input.YawPitch.y);
        //     }

        //     ApplyRotations(YawPitch, false);

        //     // Get sprint multiplier
        //     float sprintMultiplier = input.Sprinting ? SprintMultiplier : 1;

        //     // by default use crouch input to toggle crouching
        //     if (input.CrouchInput)
        //     {
        //         IsCrouching = !IsCrouching;
        //     }

        //     if (input.Sprinting)
        //     {
        //         // Ensure we ain't crouching if we want to run
        //         IsCrouching = false;
        //     }

        //     if (input.JumpInput)
        //     {
        //         didJump = true;
        //     }

        //     // desired movement direction
        //     Vector2 movementInput = Vector2.ClampMagnitude(input.Movement, 1);
        //     targetVelocity = transform.TransformVector(Vector3.right * movementInput.x + Vector3.forward * movementInput.y) * WalkingSpeed * sprintMultiplier;
        // }

        // if (Sandbox.IsServer || IsPredicted)
        // {
        //     bool groundedPreMove = IsGrounded();
        //     Vector3 _velocity = Velocity;
        //     _velocity.y = 0;

        //     _velocity = Vector3.MoveTowards(_velocity, targetVelocity, AccelerationRate * Sandbox.FixedDeltaTime);

        //     _velocity.y = Velocity.y;
        //     if (groundedPreMove && didJump)
        //     {
        //         _velocity.y = JumpStrength;

        //         // here we jump so add effect to weapon.
        //         _weaponEffects.AddBump(_jumpEffectIntensity);
        //     }

        //     _velocity.y += GravityAcceleration * Sandbox.FixedDeltaTime;

        //     // move
        //     _CC.Move((_velocity) * Sandbox.FixedDeltaTime);

        //     bool groundedPostMove = IsGrounded();

        //     if (groundedPostMove)
        //         _velocity.y = 0;

        //     if (!groundedPreMove && groundedPostMove)
        //     {
        //         // this means we just landed
        //         // play effect
        //         _weaponEffects.AddBump(_landEffectIntensity);
        //     }

        //     Velocity = _velocity;
        // }
        #endregion
        HandleMovementAndLook();
    }

    // private void HandleMovement()
    // {
    //     Vector3 targetVelocity = Vector3.zero;
    //     bool didJump = false;
    //     bool didCrouch = false;

    //     if (FetchInput(out PlayerInput input))
    //     {
    //         if (_autoAimOnTarget.isOn)
    //         {
    //             // YawPitch = ClampAngles(input.YawPitch.x, input.YawPitch.y);
    //         }
    //         else
    //         {
    //             // YawPitch = ClampAngles(YawPitch.x + input.YawPitch.x, YawPitch.y + input.YawPitch.y);
    //         }

    //         // ApplyRotations(YawPitch, false);
    //         // YawPitch += new Vector2(input.MouseInput.x * Sandbox.FixedDeltaTime, input.MouseInput.y * Sandbox.FixedDeltaTime);
    //         // _yRotation += input.MouseInput.x * Sandbox.DeltaTime;

    //         // float mouseX = lookInput.x * Time.deltaTime * _sensX;
    //         // float mouseY = lookInput.y * Time.deltaTime * _sensY;
    //         // _yRotation += mouseX;
    //         // _xRotation -= mouseY;

    //         // // Handle Max Look Amounts.
    //         // //HandleMaxLookAmounts(IsProning(), Input.Player.Move.ReadValue<Vector2>() != Vector2.zero, _airState == AirState.inAir, ref _lookUpLimit, ref _lookDownLimit, _lookMaxChangeSpeed);
    //         // HandleMaxLookAmounts(IsProning(), moveInput != Vector2.zero, _airState == AirState.inAir, ref _lookUpLimit, ref _lookDownLimit, _lookMaxChangeSpeed);

    //         // // Clamp X Rot.
    //         // _xRotation = Mathf.Clamp(_xRotation, -_lookUpLimit, _lookDownLimit);

    //         // float mouseX = input.MouseInput.x * Sandbox.DeltaTime * _sensX;
    //         // float mouseY = input.MouseInput.y * Sandbox.DeltaTime * _sensY;

    //         // YawPitch += new Vector2(mouseX, -mouseY);
    //         // float mx = input.MouseInput.x * _sensX;
    //         // float my = input.MouseInput.y * _sensY;

    //         // YawPitch += new Vector2(mx, -my);

    //         ApplyRotations();

    //         // YawPitch = ClampAngles(YawPitch.x, YawPitch.y);

    //         // ApplyRotations();

    //         // Get sprint multiplier
    //         float sprintMultiplier = input.Sprinting ? SprintMultiplier : 1;

    //         // by default use crouch input to toggle crouching
    //         if (input.CrouchInput)
    //         {
    //             didCrouch = true;
    //         }

    //         if (input.Sprinting)
    //         {
    //             // Ensure we ain't crouching if we want to run
    //             IsCrouching = false;
    //         }

    //         if (input.JumpInput)
    //         {
    //             didJump = true;
    //         }

    //         // desired movement direction
    //         Vector2 movementInput = Vector2.ClampMagnitude(input.Movement, 1);
    //         targetVelocity = transform.TransformVector(Vector3.right * movementInput.x + Vector3.forward * movementInput.y) * WalkingSpeed * sprintMultiplier;
    //     }

    //     if (Sandbox.IsServer || IsPredicted)
    //     {
    //         bool groundedPreMove = IsGrounded();
    //         Vector3 _velocity = Velocity;
    //         _velocity.y = -1;

    //         _velocity = Vector3.MoveTowards(_velocity, targetVelocity, AccelerationRate * Sandbox.FixedDeltaTime);

    //         _velocity.y = Velocity.y;
    //         if (groundedPreMove && didJump)
    //         {
    //             _velocity.y = JumpStrength;

    //             // here we jump so add effect to weapon.
    //             _weaponEffects.AddBump(_jumpEffectIntensity);
    //         }

    //         _velocity.y += GravityAcceleration * Sandbox.FixedDeltaTime;

    //         // move
    //         _CC.Move((_velocity) * Sandbox.FixedDeltaTime);

    //         bool groundedPostMove = IsGrounded();

    //         if (groundedPostMove)
    //             _velocity.y = -1;

    //         if (!groundedPreMove && groundedPostMove)
    //         {
    //             // this means we just landed
    //             // play effect
    //             _weaponEffects.AddBump(_landEffectIntensity);
    //         }

    //         Velocity = _velocity;

    //         if (didCrouch)
    //         {
    //             IsCrouching = !IsCrouching;
    //         }
    //     }
    // }
    private void HandleMovementAndLook()
    {
        Vector3 targetVelocity = Vector3.zero;
        bool didJump = false;
        bool didCrouch = false;

        // if (FetchInput(out PlayerInput input))
        FetchInput(out _lastInput, out bool isDuplicated);

        if (IsInputSource || IsServer)
        {
            // Get sprint multiplier
            float sprintMultiplier = _lastInput.Sprinting ? SprintMultiplier : 1;

            // by default use crouch input to toggle crouching
            if (_lastInput.CrouchInput)
            {
                didCrouch = true;
            }

            if (_lastInput.Sprinting)
            {
                // Ensure we ain't crouching if we want to run
                IsCrouching = false;
            }

            if (_lastInput.JumpInput)
            {
                didJump = true;
            }

            // desired movement direction
            Vector2 movementInput = Vector2.ClampMagnitude(_lastInput.Movement, 1);
            targetVelocity = transform.TransformVector(Vector3.right * movementInput.x + Vector3.forward * movementInput.y) * WalkingSpeed * sprintMultiplier;

            // Look
            if (!isDuplicated)
            {
                YawPitch = ClampAngles(YawPitch.x + _lastInput.YawPitch.x, YawPitch.y + _lastInput.YawPitch.y);
                ApplyRotations(YawPitch, false);
            }
        }

        if (Sandbox.IsServer || IsPredicted)
        {
            bool groundedPreMove = IsGrounded();
            Vector3 _velocity = Velocity;
            _velocity.y = -1;

            _velocity = Vector3.MoveTowards(_velocity, targetVelocity, AccelerationRate * Sandbox.FixedDeltaTime);

            _velocity.y = Velocity.y;
            if (groundedPreMove && didJump)
            {
                _velocity.y = JumpStrength;

                // here we jump so add effect to weapon.
                _weaponEffects.AddBump(_jumpEffectIntensity);
            }

            _velocity.y += GravityAcceleration * Sandbox.FixedDeltaTime;

            // move
            _CC.Move((_velocity) * Sandbox.FixedDeltaTime);

            bool groundedPostMove = IsGrounded();

            if (groundedPostMove)
                _velocity.y = -1;

            if (!groundedPreMove && groundedPostMove)
            {
                // this means we just landed
                // play effect
                _weaponEffects.AddBump(_landEffectIntensity);
            }

            Velocity = _velocity;

            if (didCrouch)
            {
                IsCrouching = !IsCrouching;
            }
        }
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

        // _cameraParent.localEulerAngles = new Vector3(camAngles.y, 0, 0);

        _camAngles = camAngles;
    }

    private Vector2 ClampAngles(float yaw, float pitch)
    {
        return new Vector2(ClampAngle(yaw, -360, 360), ClampAngle(pitch, -80, 80));
    }

    private float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
    public bool GetGrounded()
    {
        return IsGrounded();
    }

    public void AddRecoilRotation(float amount)
    {
        // YawPitch = new Vector2(YawPitch.x, YawPitch.y + amount);
    }

    public float GetPitch()
    {
        // return netVar ? YawPitch.x : _xRotation;
        // return YawPitch.y;
        return _camAngles.y;
    }
}