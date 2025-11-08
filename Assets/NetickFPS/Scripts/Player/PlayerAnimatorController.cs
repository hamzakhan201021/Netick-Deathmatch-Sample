using UnityEngine;
using Netick.Unity;
using Netick;

// TODO, Create Animations in anim controller, and sync from here.
public class PlayerAnimatorController : NetworkBehaviour
{

    [Networked, Smooth] public Vector2 Movement { get; set; }
    [Networked] public NetworkBool IsSprinting { get; set; }

    [Header("Debugging")]
    public bool EnableComponent = true;

    [Header("Player Movement Controller")]
    [SerializeField] private PlayerMovementController _playerMovementController;

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

    public override void NetworkStart()
    {
        SetupForAnimation();
    }

    private void SetupForAnimation()
    {
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

    public override void NetworkRender()
    {
        if (!EnableComponent) return;
        // TODO cleanup
        //Vector2 current = new Vector2(_animator.GetFloat(_moveXHash), _animator.GetFloat(_moveZHash));
        //Vector2 target = Vector2.Lerp(current, Movement, _animationLerpSpeed * Sandbox.DeltaTime);

        _currentInputVector = Vector2.SmoothDamp(_currentInputVector, Movement, ref _smoothInputVelocity, _smoothMoveInputSpeed);

        // 1 for standing, 0 for crouching blend tree value...
        float sTarget = _playerMovementController.IsCrouching ? 0 : 1;

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
}