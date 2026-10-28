using UnityEngine;
using TMPro;

public class UsernameURL : MonoBehaviour
{
    public TextMeshProUGUI textElement;

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AppendText();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void AppendText()
    {
        textElement.text += UsernameInput.playerInput;
    }
}
