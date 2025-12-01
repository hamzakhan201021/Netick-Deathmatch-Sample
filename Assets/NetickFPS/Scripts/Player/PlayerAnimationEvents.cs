using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour
{

    [Header("Foot steps")]
    [SerializeField] private AudioClip _footClip;
    [SerializeField, Range(0, 1)] private float _volume = 1;
    [SerializeField] private float _minDelayBetweenFootStep = 0.2f;
    [SerializeField] private float _minVelocity = 0.5f;
    [SerializeField] private float _minWeightForEvent = 0.5f;

    // TODO assign in inspector
    [SerializeField] private PlayerMovementController _playerMovementController;

    private float _footTimer;

    // Animation Event
    public void PlayFootSound(AnimationEvent animationEvent)
    {
        TryPlayFootSound(animationEvent);
    }

    private void Update()
    {
        if (_footTimer > 0)
        {
            _footTimer -= Time.deltaTime;
        }
        else if (_footTimer < 0)
        {
            _footTimer = 0;
        }
    }

    private void TryPlayFootSound(AnimationEvent animationEvent)
    {
        // if (_footTimer <= 0 && _playerMovementController.Velocity.magnitude >= _minVelocity)
        if (CanPlayFootSound(animationEvent.animatorClipInfo.weight))
        {
            _footTimer = _minDelayBetweenFootStep;

            AudioSource.PlayClipAtPoint(_footClip, transform.position, _volume);
        }
    }

    private bool CanPlayFootSound(float weight)
    {
        return _footTimer <= 0 && _playerMovementController.Velocity.magnitude >= _minVelocity && weight >= _minWeightForEvent;
    }
}