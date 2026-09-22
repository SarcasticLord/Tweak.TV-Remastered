using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

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

    private float _verticalVelocity;
    private bool _isGrounded;
    
    //Coyote timer variables
    private float _coyoteTimer;

    //Player Inputs
    private Vector2 _moveInput;
    private bool _slideInput;

    [Header("Player Base Stats")]
    public float playerHealth = 100f;
    public float playerSpeed = 5f;
    public float playerSlideSpeed = 150f;
    public float playerRotationSpeed = 1.0f;

    public float playerJumpForce = 9f;
    public float playerGravity = -12f;
    public float initialFallVelocity = -3f;
    public float coyoteTime = 0.3f;

    public float acceleration = 10.0f;


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

                if(_slideInput == true)
                    _currentState = PlayerState.SLIDE;
                

                break;
            case PlayerState.SLIDE:
                
                targetSpeed = playerSlideSpeed;

                //Switch to RUN state
                if(_slideInput == false)
                    _currentState = PlayerState.RUN;
                break;
        }

        Vector3 finalMove = inputDirection * _speed;
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
            if (!_isGrounded && _verticalVelocity > 0)
            {
                _verticalVelocity = 0;
            }
        }
        else
        {
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
