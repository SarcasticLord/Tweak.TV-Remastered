using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementScript : MonoBehaviour
{
    //Components
    private CharacterController _controller;

    [Header("Inputs")]
    [SerializeField] private InputActionReference jumpAction;
    [SerializeField] private InputActionReference slideAction;

    //States
    private enum PlayerState {IDLE, RUN, SLIDE, DASH};
    private PlayerState _currentState = PlayerState.IDLE;

    //Player Ingame Stats
    private float _speed;
    private bool _isJumping;
    private bool _canMove = true;
    private float _playerHeight = 2.0f;
    private float _playerSlideHeight = 0.0f;

    private float _verticalVelocity;
    private bool _isGrounded;
    
    //Coyote timer variables
    private float _coyoteTimer;

    //Player Inputs
    private Vector3 _inputDirection;
    private Vector2 _moveInput;
    private bool _slideInput;

    [Header("Player Base Stats")]
    public float playerHealth = 100f;
    public float playerSpeed = 10f;
    public float playerSlideSpeed = 20f;
    public float playerRotationSpeed = 1.0f;

    public float playerJumpForce = 9f;
    public float playerFastFall = -20f;
    public float playerGravity = -12f;
    public float initialFallVelocity = -3f;
    public float coyoteTime = 0.3f;

    public float acceleration = 7.0f;


    private void OnEnable()
    {
        jumpAction.action.performed += OnJumpPressed;
        jumpAction.action.canceled += OnJumpPressed;
        slideAction.action.performed += OnSlidePressed;
        slideAction.action.canceled += OnSlidePressed;
    }

    private void OnDisable()
    {
        jumpAction.action.performed -= OnJumpPressed;
        jumpAction.action.canceled -= OnJumpPressed;
        slideAction.action.performed -= OnSlidePressed;
        slideAction.action.canceled -= OnSlidePressed;
    }


    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _coyoteTimer = coyoteTime;
    }

    private void FixedUpdate()
    {

        Debug.Log(_currentState);

        //Ground Check
        _isGrounded = _controller.isGrounded;

        //Allow player the ability to jump a little bit after falling off a ledge
        if(_coyoteTimer > 0)
        {
            _coyoteTimer -= Time.deltaTime; 
        }
        if(_isGrounded)
        {
            _coyoteTimer = coyoteTime;
        }

        ApplyGravity();

        if(_canMove)
        {
            _inputDirection = (transform.right * _moveInput.x + transform.forward * _moveInput.y).normalized;
        }
        else
        {
            //TODO: Add slight movement in a direction while sliding
        }

            float targetSpeed = playerSpeed;
        float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;
        float speedOffset = 0.1f;

        //Player State Machine
        switch (_currentState)
        {
            case PlayerState.IDLE:

                targetSpeed = 0.0f;

                //Switch to RUN state
                if (_moveInput != Vector2.zero)
                    _currentState = PlayerState.RUN;
                //Switch to SLIDE state
                if (_slideInput == true)
                    _currentState = PlayerState.SLIDE;

                break;
            case PlayerState.RUN:

                targetSpeed = playerSpeed;

                //Switch to IDLE state
                if (_moveInput == Vector2.zero)
                    _currentState = PlayerState.IDLE;

                //Switch to SLIDE state
                if (_slideInput == true)
                    
                    _currentState = PlayerState.SLIDE;
                

                break;
            case PlayerState.SLIDE:

                _canMove = false;
                _controller.height = _playerSlideHeight;

                if (_isGrounded)
                {
                    targetSpeed = playerSlideSpeed;
                }
                else
                {
                    if (!_isJumping)
                    {
                        _verticalVelocity = playerFastFall;
                    }
                }

                //Switch to RUN state
                if (_slideInput == false)
                {
                    _canMove = true;
                    _controller.height = _playerHeight;
                    _currentState = PlayerState.RUN;
                }
                break;
        }

        //Handle player acceleration 
        if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed, Time.deltaTime * acceleration);
        }
        else
        {
            _speed = targetSpeed;
        }

        Vector3 finalMove = _inputDirection * _speed;
        finalMove.y = _verticalVelocity;
        _controller.Move(finalMove * Time.deltaTime);
    }

    void ApplyGravity()
    {
        //Keep gravity to an initial value while grounded
        if (_isGrounded && _verticalVelocity < 0)
        {
            _verticalVelocity = initialFallVelocity;
        }

        //Exponentially increase gravity as you fall
        _verticalVelocity += 2 * playerGravity * Time.deltaTime;

    }

    //Retrieve move input
    void OnMove(InputValue movementValue)
    {
        _moveInput = movementValue.Get<Vector2>();
    }

    void OnSlidePressed(InputAction.CallbackContext slideValue)
    {
        if (slideValue.ReadValue<float>() == 0)
        {
            _slideInput = false;
        }
        else
        {
            _slideInput = true;
        }
    }

    //Retrieve jump input
    void OnJumpPressed(InputAction.CallbackContext jumpValue)
    {
        //Variable jump height
        if (jumpValue.ReadValue<float>() == 0)
        {
            //Cut jump velocity
            _isJumping = false;
            if (!_isGrounded && _verticalVelocity > 0)
            {
                _verticalVelocity = 0;
            }
        }
        else
        {
            _isJumping = true;
            //On Jump pressed
            if (_isGrounded || _coyoteTimer > 0)
            {   
                _verticalVelocity = playerJumpForce;   
            }
        }
        //Prevent double jump
        _coyoteTimer = 0;
    }
}
