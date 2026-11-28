using UnityEngine;
using System.Collections.Generic;


[CreateAssetMenu(fileName = "ChatMessagePool", menuName = "Scriptable Objects/ChatMessagePool")]
public class ChatMessage : ScriptableObject
{


    [System.Serializable]
    public struct WeightedMessage
    {
        [TextArea(1,3)]
        public string messageText;

        [Range( 1, 100)]
        public int weight;
    }

    [Header("Chat Mode")]
    public string modeName = "chat";

    [Header("PoolPacing")]
    public Vector2 delayRange = new Vector2(0.5f, 2f);

    [Header("Dialogue Entries")]
    public List<WeightedMessage> messages = new List<WeightedMessage>();

    [Header("Text File Import")]
    [Tooltip("Optional: a .txt file with one message per line")]
    public TextAsset importFile;
    [Range(1, 100)]
    public int importWeight = 10;
    [Tooltip("Keep existing entries and add the file's lines after them")]
    public bool appendOnImport = false;

    [ContextMenu("Import From Text File")]
    public void ImportFromTextFile()
    {
        if (importFile == null)
        {
            Debug.LogWarning($"{name}: assign a text file to Import File first.", this);
            return;
        }

        if (messages == null) messages = new List<WeightedMessage>();
        if (!appendOnImport) messages.Clear();

        // Split on both \r and \n so Windows files work, and drop blank lines
        string[] lines = importFile.text.Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

        int added = 0;
        foreach (string line in lines)
        {
            string trimmed = line.Trim();
            if (trimmed.Length == 0) continue;

            messages.Add(new WeightedMessage { messageText = trimmed, weight = importWeight });
            added++;
        }

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
        Debug.Log($"{name}: imported {added} messages from {importFile.name}.", this);
    }


    public string GetRandomMessage()
    {
        if (messages == null || messages.Count == 0)
        {
            return "...";
        }

        // Ignore invalid weights 
        int totalWeight = 0;

        for(int i = 0; i< messages.Count; i++)
        {
            totalWeight += Mathf.Max(0, messages[i].weight);
        }

        if (totalWeight <= 0)
            return messages[Random.Range(0, messages.Count)].messageText;

        int roll = Random.Range(0, totalWeight);
        int cumulative= 0;
        
        for(int i = 0; i < messages.Count; i++)
        {
            cumulative += messages[i].weight;
            if(roll < cumulative)
            {
                return messages[i].messageText;
            }
        }

        return messages[0].messageText;
    }

    public float GetRandomDelay()
    {
        return Random.Range(delayRange.x, delayRange.y);
    }

    void OnValidate()
    {
        delayRange.x = Mathf.Max(0.05f, delayRange.x);
        delayRange.y = Mathf.Max(delayRange.x, delayRange.y);
    }

}
