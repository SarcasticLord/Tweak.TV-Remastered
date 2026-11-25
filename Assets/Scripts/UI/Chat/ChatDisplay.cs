
using System.IO;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
using static UnityEngine.Rendering.DebugUI;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;
using UnityEditor;
using UnityEngine.UI;

//by herman
public class ChatDisplay : MonoBehaviour
{
    public GameObject messagePrefab;
    public int maxMessages = 3;
    public TextAsset usernamesFile;
    public TextAsset[] messageFiles;
    public string[] chatOptions = { "chat","bored", "combat", "gaslight", "react bees", "react rats" }; //Currently for debugging, should be synced with messageFiles
    private TextAsset messageFile;
    public float messageDelay = 2f; // Delay in seconds
    private Queue<GameObject> messageQueue = new Queue<GameObject>();
    private int messageType = 0;
    public TextMeshProUGUI chatMode;
    private string[] usernames;
    private string[] messages;
    //Add list of textfiles here
    public bool isSuperChat;
    // private int lastMinute = -1;
    public string modName;
    private List<GameObject> messagePool;
    private int messagesToPool = 20;
    
    public List<string> Messages { get; private set; }

    void Start()
    {
        // player = GameObject.FindGameObjectWithTag("WholePlayer").GetComponent<>();
        //usernames = usernamesFile.text.Split('\n');

        //messages = messageFiles[0].text.Split('\n');
        //if (!isSuperChat)
        //{
        //    for (int i = 0; i < 6; i++)
        //    {
        //        AddMessage("");
        //    }
        //    StartCoroutine(DisplayMessagesWithDelay());
        //}
        messagePool = new List<GameObject>();
        GameObject tmp;
        for (int i = 0; i < messagesToPool; i++)
        {
            tmp = Instantiate(messagePrefab);
            tmp.SetActive(false);
            tmp.transform.SetParent(this.transform);
            messagePool.Add(tmp);
        }
        CycleChatSource();
    }

    // void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.T) && !isSuperChat)
    //     {
    //         CycleChatSource();
    //     }
    //     // if (isSuperChat)
    //     // {
    //         // int currentMinute = Mathf.FloorToInt(player.elapsedTime / 60f);
    //         // if (currentMinute != lastMinute)
    //         // {
    //         //     Debug.Log($"{player.elapsedTime} time.");
    //         //     AddMessage($"{currentMinute} minutes have passed...");
    //         //     lastMinute = currentMinute;
    //         // }
    //     // }
    // }




    // public void LoadTextAssetFromPath(string relativePath)
    // {
    //     TextAsset newTextAsset = Resources.Load(relativePath) as TextAsset;
    //     if (newTextAsset != null)
    //     {
    //         messageFile = newTextAsset;
    //         Debug.Log("Loaded TextAsset: " + messageFile.name);
    //         Debug.Log("Text content:\n" + messageFile.text);
    //     }
    //     else
    //     {
    //         Debug.LogWarning("TextAsset not found at path: " + relativePath);
    //     }
    // }

   public void CycleChatSource()
   {
       
       messageType += 1;
       if (messageType > messageFiles.Length-1)
       {
           messageType = 0;
       }
    //    string source = $"{chatOptions[messageType]}messages";
    //    LoadTextAssetFromPath 

       messages = messageFiles[messageType].text.Split('\n');
       Debug.Log( $"Chat mode: {chatOptions[messageType]}");
       StopAllCoroutines();
       StartCoroutine(DisplayMessagesWithDelay());
    }
    public void ChangeChatSource(string type)
    {
        string source = $"{type}messages";
        // LoadTextAssetFromPath(source);
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
            //int randNum = UnityEngine.Random.Range(0,10);
            
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
            GameObject msgObj = getPooledMessages();
            msgObj.GetComponent<TextMeshProUGUI>().text = newMessage;
            msgObj.SetActive(true);
            messageQueue.Enqueue(msgObj);
        }
        if (isSuperChat)
        {
            GameObject msgObj = getPooledMessages();
            msgObj.GetComponent<TextMeshProUGUI>().text = $"{modName}: {newMessage}";
            msgObj.SetActive(true);
            messageQueue.Enqueue(msgObj);
        }

        if (messageQueue.Count > maxMessages)
        {
            GameObject oldMsg = messageQueue.Dequeue();
            oldMsg.SetActive(false);
        }
    }

    private GameObject getPooledMessages()
    {
        for(int i = 0;i < messagesToPool; i++)
        {
            if (!messagePool[i].activeInHierarchy)
            {
                return messagePool[i];
            }
        }
        return null;
    }
}
