using UnityEngine;
using Netick.Unity;
using Netick;

// TODO, Create Animations in anim controller, and sync from here.
// TODO, cleanup
public class PlayerAnimatorController : NetworkBehaviour
{

    [Networked, Smooth] public Vector2 Movement { get; set; }
    [Networked] public NetworkBool IsSprinting { get; set; }

    [Header("Debugging")]
    public bool EnableComponent = true;

    [Header("Player Movement Controller")]
    [SerializeField] private PlayerMovementController _playerMovementController;
    [SerializeField] private PlayerTickAnimController _playerTickAnimController;

    [Header("Animation Smoothing")]
    [SerializeField] private float _smoothMoveInputSpeed = 0.1f;
    [SerializeField] private float _animationLerpSpeed = 10;

    [Header("Animation Parameters")]
    [SerializeField] private string _moveX = "MoveX";
    [SerializeField] private string _moveZ = "MoveZ";
    [SerializeField] private string _standing = "Standing";
    [SerializeField] private string _grounded = "Grounded";
    [SerializeField] private string _walkOrRun = "WalkOrRun";

    // Animator Hashes
    private int _moveXHash;
    private int _moveZHash;
    private int _standingHash;
    private int _walkOrRunHash;
    private int _groundedHash;

    // Animator reference
    private Animator _animator;

    // Smoothing Input Vector (Adopted from HK FPS, See link in readme in case you want to XD)
    private Vector2 _currentInputVector;
    private Vector2 _smoothInputVelocity;

    private float _standingValue = 1;
    private float _walkOrRunValue = 0;

    [SerializeField] private CharacterController _controller;
    [SerializeField] private float _groundCheckRadius = 0.3f;

    public override void NetworkStart()
    {
        SetupForAnimation();
        if (!IsProxy) _playerTickAnimController.OnSetValues += SetValues;
    }

    public override void NetworkDestroy()
    {
        if (!IsProxy) _playerTickAnimController.OnSetValues -= SetValues;
    }

    private void SetupForAnimation()
    {
        // _controller = GetComponentInParent<CharacterController>();
        _animator = GetComponent<Animator>();

        _moveXHash = Animator.StringToHash(_moveX);
        _moveZHash = Animator.StringToHash(_moveZ);
        _standingHash = Animator.StringToHash(_standing);
        _groundedHash = Animator.StringToHash(_grounded);
        _walkOrRunHash = Animator.StringToHash(_walkOrRun);
    }

    public override void NetworkFixedUpdate()
    {
        if (FetchInput(out PlayerInput input))
        {
            // TODO set net vars using input.
            Movement = input.Movement;
            IsSprinting = input.Sprinting;

            //Movement = Vector2.Lerp(Movement, input.Movement, _animationLerpSpeed * Sandbox.FixedDeltaTime);
        }
    }

    // This is a registered function of tick anim controller to set the values needed.
    // It's registered manually in network start
    private void SetValues()
    {
        if (FetchInput(out PlayerInput input))
        {
            // This doesn't work well because the animation doesn't expect it to be normalized, only clamped...
            // Vector2 movementVal = Vector2.ClampMagnitude(input.Movement, 1);
            Vector2 movementVal = ClampVector2(input.Movement, -1, 1);

            _playerTickAnimController.MoveX = movementVal.x;
            _playerTickAnimController.MoveY = movementVal.y;

            // 0 for crouch, 0.5 for walk, 1 for sprint/run
            // Default to walk
            float stateValue = 0.5f;

            // set to 0 is crouching
            if (_playerMovementController.IsCrouching) stateValue = 0;
            // set to 1 if sprinting
            if (input.Sprinting) stateValue = 1;

            _playerTickAnimController.StateValue = stateValue;
            _playerTickAnimController.GroundedValueSmooth = Mathf.Lerp(_playerTickAnimController.GroundedValueSmooth, IsGrounded() ? 0 : 1, Sandbox.FixedDeltaTime * _playerTickAnimController.LerpSpeed);
        }
    }

    private bool IsGrounded()
    {
        Collider[] hits = Physics.OverlapSphere(_controller.transform.position, _groundCheckRadius);
        if (hits.Length > 0)
        {
            // Some hit is there
            // bool foundOtherThanController = false;

            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i] != _controller)
                {
                    // foundOtherThanController = true;

                    return true;
                }
            }
        }

        return false;

        // return Physics.CheckSphere(_controller.transform.position, _controller.radius);
    }

    public override void NetworkRender()
    {
        if (!EnableComponent) return;
        // TODO cleanup
        //Vector2 current = new Vector2(_animator.GetFloat(_moveXHash), _animator.GetFloat(_moveZHash));
        //Vector2 target = Vector2.Lerp(current, Movement, _animationLerpSpeed * Sandbox.DeltaTime);

        _currentInputVector = Vector2.SmoothDamp(_currentInputVector, Movement, ref _smoothInputVelocity, _smoothMoveInputSpeed);

        // 1 for standing, 0 for crouching blend tree value...
        // float sTarget = _playerMovementController.IsCrouching ? 0 : 1;
        float sTarget = 1;

        _standingValue = Mathf.Lerp(_standingValue, sTarget, _animationLerpSpeed * Sandbox.DeltaTime);

        // 1 for sprinting, 0 for walking.
        float wrTarget = IsSprinting ? 1 : 0;

        _walkOrRunValue = Mathf.Lerp(_walkOrRunValue, wrTarget, _animationLerpSpeed * Sandbox.DeltaTime);

        // Set animator values

        _animator.SetFloat(_moveXHash, _currentInputVector.x);
        _animator.SetFloat(_moveZHash, _currentInputVector.y);
        _animator.SetFloat(_standingHash, _standingValue);
        _animator.SetFloat(_walkOrRunHash, _walkOrRunValue);
        _animator.SetBool(_groundedHash, _playerMovementController.GetGrounded());
    }

    private Vector2 ClampVector2(Vector2 value, float min, float max)
    {
        value.x = Mathf.Clamp(value.x, min, max);
        value.y = Mathf.Clamp(value.y, min, max);

        return value;
    }

    void OnDrawGizmos()
    {
        if (_controller == null) return;

        Gizmos.DrawWireSphere(_controller.transform.position, _groundCheckRadius);
    }
}