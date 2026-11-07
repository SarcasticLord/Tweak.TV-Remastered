using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCameraScript : MonoBehaviour
{
    [SerializeField] private GameObject _player;
    [SerializeField] private InputActionReference lookAction;

    [Header("Head Bobbing Noise")]
    [SerializeField] private InputActionReference moveAction;
    private CinemachineBasicMultiChannelPerlin _cinemachineNoise;
    public float headBobIntensity = 1f;

    [Header("Sensivity")]
    public float sensitivity = 2f;
    private float _xRotation = 0f;

    private Vector2 _lookInput;

    private bool _isPlaying = false;

    private void OnEnable()
    {
        //Subscribe to functions
        lookAction.action.performed += StoreLookInput;
        lookAction.action.canceled += StoreLookInput;
        moveAction.action.performed += playerMoveHeadBob;
        moveAction.action.canceled += playerMoveHeadBob;
    }

    private void OnDisable()
    {
        //Unsubscribe to function
        lookAction.action.performed -= StoreLookInput;
        lookAction.action.canceled -= StoreLookInput;
        moveAction.action.performed -= playerMoveHeadBob;
        moveAction.action.canceled -= playerMoveHeadBob;
    }

    private void Awake()
    {
        _cinemachineNoise = GetComponent<CinemachineBasicMultiChannelPerlin>();
        StartCoroutine(nameof(BufferCameraInputOnGameStart));

    }
    void Start()
    {
        //Lock cursor to center of screen and hide it
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }


    void Update()
    {
        //Stop camera movement if game is paused
        if(InGameUIController.isPaused || !_isPlaying) return;

        float lookX = _lookInput.x * sensitivity;
        float lookY = _lookInput.y * sensitivity;

        _xRotation -= lookY;
        _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
        _player.transform.Rotate(Vector3.up * lookX);
        
    }

    private void StoreLookInput(InputAction.CallbackContext lookValue)
    {
        _lookInput = lookValue.ReadValue<Vector2>();
    }

    private void playerMoveHeadBob(InputAction.CallbackContext moveValue)
    {
        //Apply noise to emulate head bobbing when player is moving
        float moveMagnitude = moveValue.ReadValue<Vector2>().magnitude;

        if (moveMagnitude > 0)
        {
            _cinemachineNoise.FrequencyGain = (moveMagnitude) * headBobIntensity;
        }
        else
        {
            _cinemachineNoise.FrequencyGain = 0.5f * headBobIntensity;
        }
    }

    IEnumerator BufferCameraInputOnGameStart()
    {
        yield return new WaitForSeconds(0.5f);
        _isPlaying = true;
    }
}
