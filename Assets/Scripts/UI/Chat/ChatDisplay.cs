
using System.IO;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
using static UnityEngine.Rendering.DebugUI;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEditor;

public class ChatDisplay : MonoBehaviour
{
    public Transform chatPanel;
    public GameObject messagePrefab;
    public int maxMessages = 3;
    public TextAsset usernamesFile;
    public TextAsset messageFile;
    public float messageDelay = 2f; // Delay in seconds
    private Queue<GameObject> messageQueue = new Queue<GameObject>();
    private int messageType = 0;
    public TextMeshProUGUI chatMode;
    private string[] usernames;
    private string[] messages;
    //Add list of textfiles here
    public bool isSuperChat;
    private GameStats player;
    private int lastMinute = -1;
    public string modName;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T) && !isSuperChat)
        {
            CycleChatSource();
        }
        if (isSuperChat)
        {
            int currentMinute = Mathf.FloorToInt(player.elapsedTime / 60f);
            if (currentMinute != lastMinute)
            {
                Debug.Log($"{player.elapsedTime} time.");
                AddMessage($"{currentMinute} minutes have passed...");
                lastMinute = currentMinute;
            }
        }
    }

    public List<string> Messages { get; private set; }


    void Start()
    {
        player = GameObject.FindGameObjectWithTag("WholePlayer").GetComponent<GameStats>();
        usernames = usernamesFile.text.Split('\n');
        messages = messageFile.text.Split('\n');
        if (!isSuperChat)
        {
            for (int i = 0; i < 6; i++)
            {
                AddMessage("");
            }
            StartCoroutine(DisplayMessagesWithDelay());
        }

    }


    public void LoadTextAssetFromPath(string relativePath)
    {
        TextAsset newTextAsset = Resources.Load(relativePath) as TextAsset;
        if (newTextAsset != null)
        {
            messageFile = newTextAsset;
            Debug.Log("Loaded TextAsset: " + messageFile.name);
            Debug.Log("Text content:\n" + messageFile.text);
        }
        else
        {
            Debug.LogWarning("TextAsset not found at path: " + relativePath);
        }
    }

   public void CycleChatSource()
   {
       string[] chatOptions = { "chat","combat", "death", "bored" };
       messageType += 1;
       if (messageType > chatOptions.Length-1)
       {
           messageType = 0;
       }
       string source = $"{chatOptions[messageType]}messages";
       LoadTextAssetFromPath (source);
       messages = messageFile.text.Split('\n');
       chatMode.text = $"Chat mode: {chatOptions[messageType]}";
       StopAllCoroutines();
       StartCoroutine(DisplayMessagesWithDelay());
    }
    public void ChangeChatSource(string type)
    {
        string source = $"{type}messages";
        LoadTextAssetFromPath(source);
        messages = messageFile.text.Split('\n');
        chatMode.text = $"Chat mode: {type}";
        StopAllCoroutines();
        StartCoroutine(DisplayMessagesWithDelay());
    }


    IEnumerator DisplayMessagesWithDelay()
    {
        
        string[] colors = { "#DB0909", "#DB8E09", "#B1D709", "#2FD709", "#09B1D7", "#8443E5", "#E543E5"};  //Red, Orange, Yellow, Green, Blue, Purple, Pink
        //string[] emojis = { "\U0001f60A", "\U0001f60B", "\U0001f60C", "\U0001f60D", };


        for (int i = 0; i < Mathf.Min(usernames.Length, messages.Length); i++)
        {
            int randomName = UnityEngine.Random.Range(0, usernames.Length);
            int randomMesssage = UnityEngine.Random.Range(0, messages.Length);
            string randomColor = colors[UnityEngine.Random.Range(0, colors.Length)];
            //string randomEmoji = emojis[UnityEngine.Random.Range(0,emojis.Length)];
            int randNum = UnityEngine.Random.Range(0,10);

            
            string username = usernames[randomName].Trim();
            string message = messages[randomMesssage].Trim();

            string formattedMessage = $"<color={randomColor}>{username}</color>: {message}";
            /*if (randNum >= 5)
            {
                formattedMessage = $"<color={randomColor}>{username}</color>: ";
                for (i = 0; i < randNum; i++)
                {
                    formattedMessage += "Emoji here.";//emojis[UnityEngine.Random.Range(0, 4)];
                }
            }*/
            AddMessage(formattedMessage);
            
            yield return new WaitForSeconds(messageDelay);
        }
    }

    public void AddMessage(string newMessage)
    {
        if (!isSuperChat)
        {
            GameObject msgObj = Instantiate(messagePrefab, chatPanel);
            msgObj.GetComponent<TextMeshProUGUI>().text = newMessage;
            messageQueue.Enqueue(msgObj);
        }
        if (isSuperChat)
        {
            GameObject msgObj = Instantiate(messagePrefab, chatPanel);
            msgObj.GetComponent<TextMeshProUGUI>().text = $"{modName}: {newMessage}";
            messageQueue.Enqueue(msgObj);
        }

        if (messageQueue.Count > maxMessages)
        {
            GameObject oldMsg = messageQueue.Dequeue();
            Destroy(oldMsg);
        }
    }
}
