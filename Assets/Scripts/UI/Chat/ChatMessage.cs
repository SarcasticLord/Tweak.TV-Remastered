using UnityEngine;
using System.Collections.Generic;


[CreateAssetMenu(fileName = "ChatMessagePool", menuName = "Scriptable Objects/ChatMessagePool")]
public class ChatMessagePool : ScriptableObject
{
    [System.Serializable]
    public struct WeightedMessage
    {
        [TextArea(1,3)]
        public string messageText;

        [Range( 1, 100)]
        public int weight;
    }

    [Header("PoolPacing")]
    [Tooltip("Min/Max seconds between messages when this pool is active")]
    public Vector2 spawnDelayRange = new Vector2(0.5f,2.0f);

    [Header("Dialogue Entries")]
    public List<WeightedMessage> messages = new List<WeightedMessage>();

    public WeightedMessage GetRandomMessage()
    {
        if (messages == null || messages.Count == 0)
        {
            return new WeightedMessage {messageText = "...", weight = 1};
        }

        int totalWeight = 0;
        for(int i = 0; i< messages.Count; i++)
        {
            totalWeight += messages[i].weight;
        }

        int randomRoll = Random.Range(0, totalWeight);
        int cumulativeWeight = 0;
        
        for(int i = 0; i < messages.Count; i++)
        {
            cumulativeWeight += messages[i].weight;
            if(randomRoll < cumulativeWeight)
            {
                return messages[i];
            }
        }

        return messages[0];
    }
}
