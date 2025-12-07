using UnityEngine;
using UnityEngine.InputSystem;

namespace FpsAnimatonDev
{
    public class FpsPlayer : MonoBehaviour
    {
        [System.Serializable]
        public class InputReferences
        {
            public InputActionReference MoveInput;
            public InputActionReference LookInput;
            public InputActionReference ShootInput;
            public InputActionReference ReloadInput;
            public InputActionReference SprintInput;
            public InputActionReference CrouchInput;
        }

        [Header("Input")]
        [SerializeField] private UnityEngine.InputSystem.PlayerInput _playerInput;
        [SerializeField] private InputReferences _inputReferences;

        [Header("Movement Settings")]
        [SerializeField] private float walkSpeed = 5f;
        [SerializeField] private float sprintSpeed = 8f;
        [SerializeField] private float crouchSpeed = 2.5f;
        [SerializeField] private float gravity = -9.81f;
        [SerializeField] private float crouchHeight = 1f;
        [SerializeField] private float standingHeight = 2f;
        [SerializeField] private float crouchTransitionSpeed = 5f;

        [Header("Look Settings")]
        // [SerializeField] private Transform playerCamera;
        [SerializeField] private float _sensitivityX = 1f;
        [SerializeField] private float _sensitivityY = 1f;
        // [SerializeField] private float gamepadSensitivity = 50f;
        [SerializeField] private float minPitch = -90f;
        [SerializeField] private float maxPitch = 90f;

        private InputAction _moveInput;
        private InputAction _lookInput;
        private InputAction _shootInput;
        private InputAction _reloadInput;
        private InputAction _sprintInput;
        private InputAction _crouchInput;

        private CharacterController _controller;
        private Vector3 _velocity;
        private bool _isSprinting;
        private bool _isCrouching;

        private float _pitch;

        public float Pitch {get {return _pitch;} }

        void Start()
        {
            _controller = GetComponent<CharacterController>();
            InitializeInputs();

            // if (!playerCamera)
            //     Debug.LogError("Player Camera not assigned in FpsPlayer script.");
        }

        private void InitializeInputs()
        {
            _moveInput = _playerInput.actions[_inputReferences.MoveInput.action.name];
            _lookInput = _playerInput.actions[_inputReferences.LookInput.action.name];
            _shootInput = _playerInput.actions[_inputReferences.ShootInput.action.name];
            _reloadInput = _playerInput.actions[_inputReferences.ReloadInput.action.name];
            _sprintInput = _playerInput.actions[_inputReferences.SprintInput.action.name];
            _crouchInput = _playerInput.actions[_inputReferences.CrouchInput.action.name];
        }

        void Update()
        {
            Move();
            HandleLook();
            HandleShooting();
            HandleReload();
        }

        private void Move()
        {
            Vector2 input = _moveInput.ReadValue<Vector2>();
            Vector3 move = transform.right * input.x + transform.forward * input.y;

            _isSprinting = _sprintInput.IsPressed() && !_isCrouching;
            _isCrouching = _crouchInput.IsPressed();

            float speed = walkSpeed;
            if (_isSprinting) speed = sprintSpeed;
            if (_isCrouching) speed = crouchSpeed;

            _controller.Move(move * speed * Time.deltaTime);

            // Gravity
            if (!_controller.isGrounded)
                _velocity.y += gravity * Time.deltaTime;
            else if (_velocity.y < 0)
                _velocity.y = 0;

            _controller.Move(_velocity * Time.deltaTime);

            // Smooth crouch transition
            float targetHeight = _isCrouching ? crouchHeight : standingHeight;
            _controller.height = Mathf.Lerp(_controller.height, targetHeight, crouchTransitionSpeed * Time.deltaTime);
            _controller.center = new Vector3(0, _controller.height / 2f, 0);
        }

        private void HandleLook()
        {
            Vector2 lookDelta = _lookInput.ReadValue<Vector2>();

            // Detect if using mouse or gamepad
            // float deltaX = lookDelta.x * (Mouse.current != null ? mouseSensitivity : gamepadSensitivity) * Time.deltaTime;
            // float deltaY = lookDelta.y * (Mouse.current != null ? mouseSensitivity : gamepadSensitivity) * Time.deltaTime;
            bool isCurrentMouse = _lookInput.activeControl?.device is Mouse;

            float deltaMultiplier = isCurrentMouse ? 1.0f : Time.deltaTime;
            float deltaX = lookDelta.x * _sensitivityX * deltaMultiplier;
            float deltaY = lookDelta.y * _sensitivityY * deltaMultiplier;

            // Horizontal rotation (yaw)
            transform.Rotate(Vector3.up, deltaX);

            // Vertical rotation (pitch)
            _pitch -= deltaY;
            _pitch = Mathf.Clamp(_pitch, minPitch, maxPitch);
            // playerCamera.localEulerAngles = new Vector3(pitch, 0, 0);
        }

        private void HandleShooting()
        {
            if (_shootInput.WasPressedThisFrame())
            {
                // Fire weapon logic
            }
        }

        private void HandleReload()
        {
            if (_reloadInput.WasPressedThisFrame())
            {
                // Reload weapon logic
            }
        }
    }
}
