using System.Collections.Generic;
using Netick;
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
    [SerializeField] private float _sensX = 2;
    [SerializeField] private float _sensY = 2;
    [SerializeField] private float _inputSmoothSpeed = 5;

    [Header("Player References")]
    [SerializeField] private Transform _cameraParent;
    [SerializeField] private Transform _renderTransform;

    [Header("For Testing Use")]
    [SerializeField] private bool _useAutoMover = false;
    [SerializeField] private GameObject _autoMoverUI;
    [SerializeField] private Toggle _autoMove;
    [SerializeField] private Toggle _directionAMove;
    [Header("For Lag Comp Tests")]
    [SerializeField] private Toggle _autoAimOnTarget;

    [Networked(relevancy: Relevancy.InputSource)] public Vector3 Velocity { get; set; }
    [Networked][Smooth] public Vector2 YawPitch { get; set; }
    [Networked] public NetworkBool IsCrouching { get; set; }

    private Vector2 _camAngles;
    private bool _cursorLocked;

    // Testing
    private Transform _target;

    private Vector2 mouseInputsYP;
    private Vector2 mouseInputSmooth;

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

        var networkInput = Sandbox.GetInput<PlayerInput>();


        if (_autoMoverUI != null) _autoMoverUI.SetActive(_useAutoMover);

        if (_autoMove != null && _autoMove.isOn && _useAutoMover)
        {
            networkInput.Movement = new Vector2(_directionAMove.isOn ? 1 : -1, 0);
        }
        else
        {
            networkInput.Movement = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        }


        mouseInputsYP = Vector2.Lerp(mouseInputsYP, new Vector2(Input.GetAxisRaw("Mouse X") * _sensX, -Input.GetAxisRaw("Mouse Y") * _sensY), _inputSmoothSpeed * Time.deltaTime);

        mouseInputSmooth = Vector2.Lerp(mouseInputSmooth, new Vector2(Input.GetAxisRaw("Mouse X"), -Input.GetAxisRaw("Mouse Y")), _inputSmoothSpeed * Time.deltaTime);

        //networkInput.MouseInput = Vector2.Lerp(, ,);
        networkInput.MouseInput = mouseInputSmooth;

        if (!_cursorLocked) mouseInputsYP = Vector2.zero;

        if (_autoAimOnTarget != null && _autoAimOnTarget.isOn)
        {
            // TODO improve targetting etc. (it's for testing anyways)

            if (_target == null)
            {
                List<PlayerInputProvider> playerObjects = Sandbox.FindObjectsOfType<PlayerInputProvider>();

                for (int i = 0; i < playerObjects.Count; i++)
                {
                    if (playerObjects[i] != GetComponent<PlayerInputProvider>())
                    {
                        // this surely isn't our own player so just select it as target
                        _target = playerObjects[i].transform;
                    }
                }
            }

            if (_target != null)
            {
                Vector3 dir = (_target.position + new Vector3(0, 1, 0)) - _cameraParent.position;
                dir.Normalize();

                float yaw = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
                float pitch = -Mathf.Asin(dir.y) * Mathf.Rad2Deg;

                networkInput.YawPitch = new Vector2(yaw - 2, pitch);
            }
            else
            {
                // Target is null.
#if UNITY_EDITOR
                Debug.LogWarning("Auto aim couldn't find a target XD");
#endif
            }
        }
        else
        {
            networkInput.YawPitch += mouseInputsYP;
        }

        //networkInput.YawPitch += mouseInputs;

        networkInput.CrouchInput |= Input.GetKeyDown(KeyCode.C);
        networkInput.Sprinting = Input.GetKey(KeyCode.LeftShift);
        networkInput.JumpInput |= Input.GetKeyDown(KeyCode.Space);

        Sandbox.SetInput(networkInput);

        // we apply the rotation in update on the client to prevent look delay
        _camAngles = ClampAngles(_camAngles.x + mouseInputsYP.x, _camAngles.y + mouseInputsYP.y);
        ApplyRotations(_camAngles, false);
    }



    public override void NetworkFixedUpdate()
    {
        Vector3 targetVelocity = Vector3.zero;
        bool didJump = false;

        if (FetchInput(out PlayerInput input))
        {
            if (_autoAimOnTarget.isOn)
            {
                YawPitch = ClampAngles(input.YawPitch.x, input.YawPitch.y);
            }
            else
            {
                YawPitch = ClampAngles(YawPitch.x + input.YawPitch.x, YawPitch.y + input.YawPitch.y);
            }

            ApplyRotations(YawPitch, false);

            float sprintMultiplier = input.Sprinting ? SprintMultiplier : 1;

            if (input.CrouchInput)
            {
                IsCrouching = !IsCrouching;
            }

            if (input.JumpInput)
            {
                didJump = true;
            }

            // desired movement direction
            Vector2 movementInput = Vector2.ClampMagnitude(input.Movement, 1);
            targetVelocity = transform.TransformVector(Vector3.right * movementInput.x + Vector3.forward * movementInput.y) * WalkingSpeed * sprintMultiplier;
        }

        if (Sandbox.IsServer || IsPredicted)
        {
            bool groundedPreMove = IsGrounded();
            Vector3 _velocity = Velocity;
            _velocity.y = 0;

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
                _velocity.y = 0;

            if (!groundedPreMove && groundedPostMove)
            {
                // this means we just landed
                // play effect
                _weaponEffects.AddBump(_landEffectIntensity);
            }

            Velocity = _velocity;
        }

        // ManageMovement();
    }

    private void ManageMovement()
    {
        Vector3 targetVelocity = Vector3.zero;
        bool didJump = false;

        if (FetchInput(out PlayerInput input))
        {
            // Apply yaw rotation (without touching pitch)
            if (_autoAimOnTarget != null && _autoAimOnTarget.isOn)
            {
                YawPitch = ClampAngles(input.YawPitch.x, input.YawPitch.y);
            }
            else
            {
                YawPitch = ClampAngles(YawPitch.x + input.YawPitch.x, YawPitch.y + input.YawPitch.y);
            }

            float sprintMultiplier = input.Sprinting ? SprintMultiplier : 1f;
            if (input.JumpInput)
                didJump = true;

            // Local move direction in world space
            Vector2 moveInput = Vector2.ClampMagnitude(input.Movement, 1f);
            Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);
            targetVelocity = transform.TransformDirection(move) * WalkingSpeed * sprintMultiplier;
        }

        if (Sandbox.IsServer || IsPredicted)
        {
            bool groundedBefore = IsGrounded();

            Vector3 vel = Velocity;
            vel.y = 0;
            vel = Vector3.MoveTowards(vel, targetVelocity, AccelerationRate * Sandbox.FixedDeltaTime);
            vel.y = Velocity.y;

            if (groundedBefore && didJump)
                vel.y = JumpStrength;

            vel.y += GravityAcceleration * Sandbox.FixedDeltaTime;

            _CC.Move(vel * Sandbox.FixedDeltaTime);

            bool groundedAfter = IsGrounded();
            if (groundedAfter)
                vel.y = 0;

            Velocity = vel;
        }
    }

    public override void NetworkRender()
    {
        if (IsProxy)
            ApplyRotations(YawPitch, true);
    }

    [OnChanged(nameof(YawPitch), invokeDuringResimulation: true)]
    private void OnYawPitchChanged(OnChangedData onChanged)
    {
        ApplyRotations(YawPitch, false);
    }

    private void ApplyRotations(Vector2 camAngles, bool isProxy)
    {
        // TODO remove this junk
        //if (_autoAimOnTarget.isOn && !isProxy)
        //{
        //    // TODO improve targetting etc. (it's for testing anyways)

        //    List<PlayerInputProvider> playerObjects = Sandbox.FindObjectsOfType<PlayerInputProvider>();

        //    Transform target = null;

        //    for (int i = 0; i < playerObjects.Count; i++)
        //    {
        //        if (playerObjects[i] != GetComponent<PlayerInputProvider>())
        //        {
        //            // this surely isn't our own player so just select it as target
        //            target = playerObjects[i].transform;
        //        }
        //    }

        //    if (target != null)
        //    {
        //        Vector3 dir = (target.position + new Vector3(0, 1, 0)) - _cameraParent.position;
        //        dir.Normalize();

        //        float yaw = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
        //        float pitch = -Mathf.Asin(dir.y) * Mathf.Rad2Deg;

        //        camAngles = new Vector2(yaw, pitch);
        //    }
        //}

        // THIS WAS COMMENTED

        // on the player transform, we apply yaw.
        if (isProxy)
        {
            _renderTransform.rotation = Quaternion.Euler(new Vector3(0, camAngles.x, 0));
        }
        else
        {
            transform.rotation = Quaternion.Euler(new Vector3(0, camAngles.x, 0));
        }

        // on the weapon/camera holder, we apply the pitch angle.
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
}