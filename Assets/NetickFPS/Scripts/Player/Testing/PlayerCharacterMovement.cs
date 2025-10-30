using Netick;
using Netick.Unity;
using UnityEngine;

public class PlayerCharacterMovement : NetworkBehaviour
{

    [Header("Movement")]
    [SerializeField] private CharacterController _cc;
    [SerializeField] private float MoveSpeed = 5f;
    [SerializeField] private float GravityMultiplier = 2;
    [SerializeField] private float JumpPower = 5;
    [Header("Smoothed Visuals")]
    //[SerializeField] private Transform RenderTransform;

    // Networked...
    //[Networked, Smooth] private Vector3 Position { get; set; }
    //[Networked, Smooth] private Quaternion Rotation { get; set; }
    [Networked] private float _yVelocity { get; set; }

    private float _gravity = -9.81f;
    //[Networked, Smooth] private float 

    public override void NetworkUpdate()
    {
        if (!IsInputSource || !Sandbox.InputEnabled)
            return;

        var networkInput = Sandbox.GetInput<PlayerCharacterInput>();

        networkInput.Movement = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        networkInput.Jump |= Input.GetKeyDown(KeyCode.Space);

        Sandbox.SetInput(networkInput);
    }

    public override void NetworkFixedUpdate()
    {
        if (FetchInput(out PlayerCharacterInput input))
        {
            Vector3 movement = new Vector3(input.Movement.x, 0, input.Movement.y);
            //transform.position += movement * Sandbox.FixedDeltaTime * MoveSpeed;

            movement *= MoveSpeed;

            if (!_cc.isGrounded)
            {
                //movement.y = -9.61f;
                _yVelocity += _gravity * GravityMultiplier * Sandbox.FixedDeltaTime;
            }
            else
            {
                _yVelocity = -1;
            }

            if (_cc.isGrounded && input.Jump)
            {
                _yVelocity = JumpPower;
            }

            movement.y = _yVelocity;

            _cc.Move(movement * Sandbox.FixedDeltaTime);
        }
    }

    //public override void NetworkRender()
    //{
    //    RenderTransform.transform.position = Position;
    //    RenderTransform.transform.rotation = Rotation;
    //}

    //public override void NetcodeIntoGameEngine()
    //{
    //    transform.position = Position;
    //    transform.rotation = Rotation;
    //}

    //public override void GameEngineIntoNetcode()
    //{
    //    Position = transform.position;
    //    Rotation = transform.rotation;
    //}
}