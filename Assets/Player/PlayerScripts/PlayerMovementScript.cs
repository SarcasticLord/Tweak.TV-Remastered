using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementScript : MonoBehaviour
{
    //Components
    private PlayerInput _playerInput;
    private CharacterController _controller;
    private GameObject _mainCamera;

    private enum PlayerState {IDLE, RUN, SLIDE, DASH, JUMP};
    private PlayerState _currentState = PlayerState.IDLE;

    //Player ingame stats
    private float _speed;
    private float _rotationVelocity;
    private float _verticalVelocity;

    //Player input
    private Vector2 _moveInput;

    //Player base stats
    public float playerHealth = 100f;
    public float playerSpeed = 5f;
    public float playerSlideSpeed = 150f;
    public float acceleration = 10.0f;

    private void Awake()
    {
        if (_mainCamera == null)
        {
            _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        }
    }
    void Start()
    {
        _controller = GetComponent<CharacterController>();
        _playerInput = GetComponent<PlayerInput>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {

        float targetSpeed = playerSpeed;
        float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;
        //Vector2 stickInput = Gamepad.current.leftStick.ReadValue();

        float speedOffset = 0.1f;
        //float inputMagnitude = stickInput.magnitude >= 0.15f ? _moveInput.magnitude : 1f;

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

                if (_moveInput != Vector2.zero)
                    _currentState = PlayerState.RUN;

                break;
            case PlayerState.RUN:

                if (_moveInput == Vector2.zero)
                    _currentState = PlayerState.IDLE;

                Vector3 inputDirection = new Vector3(_moveInput.x, 0.0f, _moveInput.y).normalized;
                
                if(_moveInput != Vector2.zero)
                {
                    inputDirection = transform.right * _moveInput.x + transform.forward * _moveInput.y;
                }
                
                _controller.Move(inputDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
                break;
        }

    }

    //Retrieve input from Unity input system
    void OnMove(InputValue movementValue)
    {
        _moveInput = movementValue.Get<Vector2>();
        Debug.Log("Player Movement: " + _moveInput.x + "," + _moveInput.y);
    }

}
