using TMPro;
using UnityEngine;

public class UsernameInput : MonoBehaviour
{
    public static UsernameInput Instance;
    public static string playerInput = "";

    public TMP_InputField inputField;
    [SerializeField] private TextMeshProUGUI username;    

    public void SaveInput()
    {
        username.text = " ";
        playerInput = inputField.text;
        username.text = playerInput.ToString();

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
