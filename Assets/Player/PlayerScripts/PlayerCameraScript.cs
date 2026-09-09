using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCameraScript : MonoBehaviour
{
    [SerializeField] private GameObject _player;
    [SerializeField] private InputActionReference lookAction;

    public float sensitivity = 2f;
    private float xRotation = 0f;

    private Vector2 _lookInput;

    private void OnEnable()
    {
        lookAction.action.performed += StoreLookInput;
        lookAction.action.canceled += StoreLookInput;
    }

    private void OnDisable()
    {
        lookAction.action.performed -= StoreLookInput;
        lookAction.action.canceled -= StoreLookInput;
    }

    void Start()
    {
        //Lock cursor to center of screen and hide it
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }


    void Update()
    {
        float lookX = _lookInput.x * sensitivity;
        float lookY = _lookInput.y * sensitivity;

        xRotation -= lookY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        _player.transform.Rotate(Vector3.up * lookX);
        
    }

    private void StoreLookInput(InputAction.CallbackContext lookValue)
    {
        _lookInput = lookValue.ReadValue<Vector2>();
    }
}
