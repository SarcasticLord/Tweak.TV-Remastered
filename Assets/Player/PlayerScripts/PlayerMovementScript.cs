using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementScript : MonoBehaviour
{
    //Components
    private PlayerInput _playerInput;
    private CharacterController _controller;

    //States
    private enum PlayerState {IDLE, RUN, SLIDE, DASH, JUMP};
    private PlayerState _currentState = PlayerState.IDLE;

    //Player Ingame Stats
    private float _speed;

    private float _verticalVelocity;
    private bool _isGrounded;


    //Player Inputs
    private Vector2 _moveInput;

    //Player base stats
    public float playerHealth = 100f;
    public float playerSpeed = 5f;
    public float playerSlideSpeed = 150f;
    public float playerRotationSpeed = 1.0f;

    public float playerJumpForce = 6f;
    public float playerGravity = -12f;
    public float initialFallVelocity = -2f;

    public float acceleration = 10.0f;
    


    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _playerInput = GetComponent<PlayerInput>();

    }

    private void Update()
    {
        ApplyGravity();

        //Ground Check
        _isGrounded = _controller.isGrounded;

        Vector3 inputDirection = (transform.right * _moveInput.x + transform.forward * _moveInput.y).normalized;

        float targetSpeed = playerSpeed;
        float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;
        float speedOffset = 0.1f;

        //Handle player acceleration 
        if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed, Time.deltaTime * acceleration);
        }
        else
        {
            _speed = targetSpeed;
        }

        //Player State Machine
        switch (_currentState)
        {
            case PlayerState.IDLE:

                targetSpeed = 0.0f;

                //Switch to RUN state
                if (_moveInput != Vector2.zero)
                    _currentState = PlayerState.RUN;

                break;
            case PlayerState.RUN:

                //Switch to IDLE state
                if (_moveInput == Vector2.zero)
                    _currentState = PlayerState.IDLE;
                
                break;
        }

        Vector3 finalMove = inputDirection * _speed;
        finalMove.y = _verticalVelocity;
        _controller.Move(finalMove * Time.deltaTime);
    }

    void ApplyGravity()
    {
        if(_isGrounded && _verticalVelocity < 0)
        {
            _verticalVelocity = initialFallVelocity;
        }

        _verticalVelocity += playerGravity * Time.deltaTime;
    }

    //Retrieve move input
    void OnMove(InputValue movementValue)
    {
        _moveInput = movementValue.Get<Vector2>();
    }

    //Retrieve jump input
    void OnJump(InputValue jumpValue)
    {
        if (_isGrounded)
        {
            _verticalVelocity = playerJumpForce;
        }
    }
}
