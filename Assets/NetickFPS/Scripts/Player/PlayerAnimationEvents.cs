using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour
{

    [Header("Foot steps")]
    [SerializeField] private AudioClip _footClip;
    [SerializeField, Range(0, 1)] private float _volume = 1;
    [SerializeField] private float _minDelayBetweenFootStep = 0.2f;
    [SerializeField] private float _minVelocity = 0.5f;

    // TODO assign in inspector
    [SerializeField] private PlayerMovementController _playerMovementController;

    private float _footTimer;

    // Animation Event
    public void PlayFootSound()
    {
        TryPlayFootSound();
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

    private void TryPlayFootSound()
    {
        if (_footTimer <= 0 && _playerMovementController.Velocity.magnitude >= _minVelocity)
        {
            _footTimer = _minDelayBetweenFootStep;

            AudioSource.PlayClipAtPoint(_footClip, transform.position, _volume);
        }
    }
}