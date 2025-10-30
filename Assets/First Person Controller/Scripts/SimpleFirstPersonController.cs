using UnityEngine;
using Netick;
using Netick.Unity;
using UnityEngine.UI;



//public struct InterpData
//{
//    public int RemoteInterpFrom;
//    public int RemoteInterpTo;
//    public float RemoteInterpAlpha;
//}

public class SimpleFirstPersonController : NetworkedCharacterController
{
    
    // Networked properties
    [Networked(relevancy: Relevancy.InputSource)] public Vector3 Velocity { get; set; }
    [Networked][Smooth] public float Pitch { get; set; }

    [SerializeField] private bool _useAutoMover = false;
    [SerializeField] private GameObject _autoMoverUI;
    [SerializeField] private Toggle _autoMove;
    [SerializeField] private Toggle _directionAMove;


    [Header("Stable Movement")]
    public float WalkingSpeed = 2.5f;
    public float SprintMultiplier = 2f;
    public float AccelerationRate = 25f;

    [Header("Air Movement")]
    public float JumpStrength = 5;
    public float GravityAcceleration = -9.81f;

    [Header("Other")]
    //[SerializeField] private float _movementSpeed = 10;
    [SerializeField] private float _sensitivityX = 1.6f;
    [SerializeField] private float _sensitivityY = -1f;
    [SerializeField] private Transform _cameraParent;
    [SerializeField] private Transform _renderTransform;

    private Vector2 _camAngles;
    private bool _cursorLocked;

    void UpdateCursorLock()
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

    private void Update()
    {
        if (!IsInputSource)
            return;
        if (Input.GetKeyDown(KeyCode.Escape))
            _cursorLocked = !_cursorLocked;
        if (Sandbox.InputEnabled)
            UpdateCursorLock();
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

    public override void OnInputSourceLeft()
    {
        // destroy the player object when its input source (controller player) leaves the game
        Sandbox.Destroy(Object);
    }

    public override void NetworkRender()
    {
        if (IsProxy)    //on local client, we apply the camera rotation in update. on proxies, we use network render
        {
            _camAngles.x = transform.eulerAngles.y;
            _camAngles.y = Pitch;
            ApplyRotations(_camAngles, true);
        }
    }

    public override void NetworkUpdate()
    {
        if (!IsInputSource || !Sandbox.InputEnabled)
            return;

        var networkInput = Sandbox.GetInput<PlayerInput>();


        _autoMoverUI.SetActive(_useAutoMover);
        //if (_useAutoMover)
        //{
        //    //_autoMove.gameObject.SetActive(true);
        //    //_directionAMove.gameObject.SetActive(true);
        //    _autoMoverUI.SetActive(;
        //}
        //else
        //{
        //    _autoMove.gameObject.SetActive(false);
        //    _directionAMove.gameObject.SetActive(false);
        //}

        if (_autoMove.isOn && _useAutoMover)
        {
            networkInput.Movement = new Vector2(_directionAMove.isOn ? 1 : -1, 0);
        }
        else
        {
            networkInput.Movement = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        }





        Vector2 mouseInput = new Vector2(Input.GetAxisRaw("Mouse X") * _sensitivityX, Input.GetAxisRaw("Mouse Y") * _sensitivityY);
        if (!_cursorLocked)
            mouseInput = Vector2.zero;

        networkInput.Sprinting = Input.GetKey(KeyCode.LeftShift);
        networkInput.JumpInput |= Input.GetKeyDown(KeyCode.Space);

        //networkInput.YawPitchDelta += mouseInput;
        Sandbox.SetInput<PlayerInput>(networkInput);

        // we apply the rotation in update on the client to prevent look delay
        _camAngles += mouseInput;
        _camAngles.y = Mathf.Clamp(_camAngles.y, -90, 90);
        ApplyRotations(_camAngles, true);
    }

    public override void NetworkFixedUpdate()
    {
        Vector3 targetVelocity = Vector3.zero;
        bool didJump = false;

        float yaw = transform.eulerAngles.y;
        if (FetchInput(out PlayerInput input))
        {
            //Pitch = Mathf.Clamp(Pitch + input.YawPitchDelta.y, -90, 90);
            //yaw += input.YawPitchDelta.x;

            float sprintMultiplier = input.Sprinting ? SprintMultiplier : 1;

            if (input.JumpInput)
                didJump = true;

            // desired movement direction
            Vector2 movementInput = Vector2.ClampMagnitude(input.Movement, 1);
            targetVelocity = transform.TransformVector(Vector3.right * movementInput.x + Vector3.forward * movementInput.y) * WalkingSpeed * sprintMultiplier;
        }

        ApplyRotations(new Vector2(yaw, Pitch));

        if (Sandbox.IsServer || IsPredicted)
        {
            bool groundedPreMove = IsGrounded();
            Vector3 _velocity = Velocity;
            _velocity.y = 0;

            _velocity = Vector3.MoveTowards(_velocity, targetVelocity, AccelerationRate * Sandbox.FixedDeltaTime);

            _velocity.y = Velocity.y;
            if (groundedPreMove && didJump)
                _velocity.y = JumpStrength;
            _velocity.y += GravityAcceleration * Sandbox.FixedDeltaTime;

            // move
            _CC.Move((_velocity) * Sandbox.FixedDeltaTime);

            bool groundedPostMove = IsGrounded();

            if (groundedPostMove)
                _velocity.y = 0;

            Velocity = _velocity;
        }
    }

    private void ApplyRotations(Vector2 camAngles, bool render = false)
    {
        // on the player transform, we apply yaw
        if (!render)
            transform.rotation = Quaternion.Euler(new Vector3(0, camAngles.x, 0));
        _renderTransform.rotation = Quaternion.Euler(new Vector3(0, camAngles.x, 0));

        // on the weapon/camera holder, we apply the pitch angle
        _cameraParent.localEulerAngles = new Vector3(camAngles.y, 0, 0);

        _camAngles = camAngles;
    }
}
