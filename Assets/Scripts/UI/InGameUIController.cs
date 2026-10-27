using UnityEngine;
using UnityEngine.InputSystem;

public class InGameUIController : MonoBehaviour
{

    [SerializeField] private GameObject pauseScreen;

    [SerializeField] private InputActionReference pauseAction;

    public static bool isPaused { get; private set; } = false;

    private void OnEnable()
    {
        pauseAction.action.performed += onPausePressed;
        pauseAction.action.Enable();
    }

    private void OnDisable()
    {
        pauseAction.action.performed -= onPausePressed;
        pauseAction.action.Disable();
    }

    private void Start()
    {
        pauseScreen.SetActive(false);
    }

    void onPausePressed(InputAction.CallbackContext pause)
    {
        if (pause.ReadValueAsButton())
        {
            if (isPaused == false)
            {
                //Freeze game and activate pause overlay
                pauseScreen.SetActive(true);
                Time.timeScale = 0f;
                isPaused = true;
            }
            else
            {
                //Unpause
                pauseScreen.SetActive(false);
                Time.timeScale = 1f;
                isPaused = false;
            }
        }
    }

}
