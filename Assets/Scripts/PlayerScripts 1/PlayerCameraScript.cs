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
    public float movementFrequencyMultiplier = 1.5f;

    [Header("Proximity Settings")]
    [SerializeField] private float detectionRadius = 75f;
    [SerializeField] private float baseIntensity = 0f;
    [SerializeField] private float maxTrainIntensity = 2f;
    [SerializeField] private float smoothingSpeed = 10f;
    private float _currentTrainIntensity = 0f;

    private GameObject[] trains;

    [Header("Sensivity")]
    public float sensitivity = 2f;
    private float _xRotation = 0f;

    private Vector2 _lookInput;
    private Vector2 _moveInput;

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
        //Cache trains at the start of the scene
        trains = GameObject.FindGameObjectsWithTag("Train"); //Anything tagged Trains will increase head bobbing intensity 
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

        //Train camera shake
        float highestTargetIntensity = baseIntensity;

        if(trains != null && trains.Length > 0)
        {
            for(int i = 0; i < trains.Length; i++)
            {
                if (trains[i] == null) continue;

                float distance = Vector3.Distance(transform.position, trains[i].transform.position);
                if(distance < detectionRadius)
                {
                    float proximityPercentage = 1f - (distance/detectionRadius);
                    float calculatedIntensity = Mathf.Lerp(baseIntensity, maxTrainIntensity, proximityPercentage);
                    if (calculatedIntensity > highestTargetIntensity)
                    {
                        highestTargetIntensity = calculatedIntensity;
                    }
                }
            }
        }

        //Adjust headbob Intensity
        _currentTrainIntensity = Mathf.MoveTowards(_currentTrainIntensity, highestTargetIntensity, smoothingSpeed * Time.deltaTime);

        float moveMagnitude = _moveInput.magnitude;

        float movementFrequency = moveMagnitude * movementFrequencyMultiplier;
        float trainFrequencyBoost = _currentTrainIntensity;

        _cinemachineNoise.FrequencyGain = 1.0f + movementFrequency + trainFrequencyBoost;

    }

    private void StoreLookInput(InputAction.CallbackContext lookValue)
    {
        _lookInput = lookValue.ReadValue<Vector2>();
    }

    private void playerMoveHeadBob(InputAction.CallbackContext moveValue)
    {
        _moveInput = moveValue.ReadValue<Vector2>();
    }

    IEnumerator BufferCameraInputOnGameStart()
    {
        yield return new WaitForSeconds(0.5f);
        _isPlaying = true;
    }
}
