using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class InGameUIController : MonoBehaviour
{
    [SerializeField] private InputActionReference pauseAction;

    [Header("UI elements")]
    [SerializeField] private GameObject pauseScreen;
    [SerializeField] private TextMeshProUGUI viewCountText;
    [SerializeField] private TextMeshProUGUI timerText;

    public static float gameTime = 0f;

    public static bool isPaused { get; private set; } = false;

    private void OnEnable()
    {
        pauseAction.action.performed += onPausePressed;
        pauseAction.action.Enable();
        GameController.OnViewCountChanged += UpdateViewCount;
    }

    private void OnDisable()
    {
        pauseAction.action.performed -= onPausePressed;
        pauseAction.action.Disable();
        GameController.OnViewCountChanged -= UpdateViewCount;
    }

    private void FixedUpdate()
    {
        gameTime += Time.deltaTime;
        TimeSpan timespan = TimeSpan.FromSeconds(gameTime);
        UpdateTimer(timespan);
    }

    private void Start()
    {
        pauseScreen.SetActive(false);
        gameTime = 0;
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

    private void UpdateViewCount(int currentViewCount)
    {
        viewCountText.text = $"Viewers: {currentViewCount}";
    }

    private void UpdateTimer(TimeSpan time)
    {
        int hours = time.Hours;
        int minutes = time.Minutes; 
        int seconds = time.Seconds;

        timerText.text = string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
    }
}
