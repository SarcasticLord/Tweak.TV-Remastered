using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class PlayerMovementScript : MonoBehaviour
{
    //Components
    private PlayerInput _playerInput;
    private CharacterController _controller;

    [Header("Inputs")]
    [SerializeField] private InputActionReference jumpAction;

    //States
    private enum PlayerState {IDLE, RUN, SLIDE, DASH, JUMP};
    private PlayerState _currentState = PlayerState.IDLE;

    //Player Ingame Stats
    private float _speed;

    private float _verticalVelocity;
    private bool _isGrounded;


    //Player Inputs
    private Vector2 _moveInput;

    [Header("Player Base Stats")]
    public float playerHealth = 100f;
    public float playerSpeed = 5f;
    public float playerSlideSpeed = 150f;
    public float playerRotationSpeed = 1.0f;

    public float playerJumpForce = 10f;
    public float playerGravity = -12f;
    public float initialFallVelocity = -3f;

    public float acceleration = 10.0f;


    private void OnEnable()
    {
        jumpAction.action.performed += OnJumpPressed;
        jumpAction.action.canceled += OnJumpPressed;
    }

    private void OnDisable()
    {
        jumpAction.action.performed -= OnJumpPressed;
        jumpAction.action.canceled -= OnJumpPressed;
    }


    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _playerInput = GetComponent<PlayerInput>();

    }

    private void Update()
    {
        //Ground Check
        _isGrounded = _controller.isGrounded;

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

    //Retrieve jump input
    void OnJumpPressed(InputAction.CallbackContext jumpValue)
    {
        //Variable jump height
        if(jumpValue.ReadValue<float>() == 0)
        {
            if (!_isGrounded && _verticalVelocity > 0)
            {
                _verticalVelocity = 0;
            }
        }
        else
        {
            if (_isGrounded)
            {
                _verticalVelocity = playerJumpForce;
            }
        }

    }
}
