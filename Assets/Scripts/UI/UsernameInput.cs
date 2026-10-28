using JetBrains.Annotations;
using TMPro;
using UnityEngine;

public class UsernameInput : MonoBehaviour
{
    public static UsernameInput Instance;
    public static string playerInput = "";

    public TMP_InputField inputField;

    public void SaveInput()
    {
        playerInput = inputField.text;
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

        } else
        {
            Destroy(gameObject);
        }
    }

}
