using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class ChatManager : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] protected GameObject messagePrefab;
    public Transform messageContainer;
    [SerializeField] protected int maxMessages = 3;
    [SerializeField] protected TextMeshProUGUI chatMode;


    [Header("Super Chat")]
    [SerializeField] protected bool isSuperChat;
    [SerializeField] protected string modName;

    [Header("Data")]
    [SerializeField] protected ChatUsernames usernames;
    [SerializeField] protected ChatMessage[] messagePools;


    private static readonly string[] colors = 
        { "#DB0909", "#DB8E09", "#B1D709", "#2FD709", "#09B1D7", "#8443E5", "#E543E5" };  //Red, Orange, Yellow, Green, Blue, Purple, Pink


    private class PooledMessage
    {
        public GameObject go;
        public TextMeshProUGUI text;
    }
    private readonly List<PooledMessage> freeMessages = new List<PooledMessage>();
    private readonly Queue<PooledMessage> activeMessages = new Queue<PooledMessage>();

    private int poolIndex = -1;
    private Coroutine chatRoutine;
    private Coroutine revertRoutine;
    private int basePoolIndex = -1; // pool to return to after a temporary reaction



    void Awake()
    {
        // Visible messages plus one spare
        for (int i = 0; i < maxMessages + 1; i++)
            freeMessages.Add(CreatePooledMessage());
    }

    void OnEnable()
    {
        // Coroutines stop when the object is disabled, so restart the loop if we were already running
        if (poolIndex >= 0)
            SetPool(poolIndex);
    }

    void OnDisable()
    {
        chatRoutine = null;
    }

    void Start()
    {
        SetPool(0);
    }

    // ---------- Pool handling ----------

    private PooledMessage CreatePooledMessage()
    {
        GameObject go = Instantiate(messagePrefab);
        go.transform.SetParent(messageContainer, false); // keep prefab's local scale/position
        go.SetActive(false);

        return new PooledMessage
        {
            go = go,
            text = go.GetComponentInChildren<TextMeshProUGUI>(true)
        };
    }

    private PooledMessage GetMessageSlot()
    {
        // Full: recycle the oldest visible message
        if (activeMessages.Count >= maxMessages)
            return activeMessages.Dequeue();

        // Otherwise take a spare
        if (freeMessages.Count > 0)
        {
            int last = freeMessages.Count - 1;
            PooledMessage msg = freeMessages[last];
            freeMessages.RemoveAt(last);
            return msg;
        }

        // Only reached if maxMessages was raised at runtime
        return CreatePooledMessage();
    }

    public void AddMessage(string newMessage)
    {
        if (messagePrefab == null) return;

        string finalText = isSuperChat ? $"{modName}: {newMessage}" : newMessage;

        PooledMessage msg = GetMessageSlot();
        msg.text.text = finalText;
        msg.go.transform.SetAsLastSibling(); // newest at the bottom of the layout group
        msg.go.SetActive(true);

        activeMessages.Enqueue(msg);

        // If maxMessages was lowered at runtime, retire the extras
        while (activeMessages.Count > maxMessages)
        {
            PooledMessage old = activeMessages.Dequeue();
            old.go.SetActive(false);
            freeMessages.Add(old);
        }
    }

    // ---------- Mode handling ----------

    public void CycleChatSource()
    {
        if (messagePools == null || messagePools.Length == 0) return;
        SetPool((poolIndex + 1) % messagePools.Length);
    }


    // ----- Public API for other scripts -----

    // Permanently switch to a pool (cancels any temporary reaction in progress)
    public void SetPool(int index)
    {
        CancelReaction();
        ApplyPool(index);
    }

    public void SetPool(ChatMessage pool)
    {
        int index = FindPoolIndex(pool);
        if (index >= 0) SetPool(index);
        else Debug.LogWarning("ChatController: that pool isn't in the Message Pools array.", this);
    }

    public bool SetPoolByName(string modeName)
    {
        int index = FindPoolIndex(modeName);
        if (index < 0)
        {
            Debug.LogWarning($"ChatController: no pool with mode name \"{modeName}\".", this);
            return false;
        }

        SetPool(index);
        return true;
    }

    // Temporarily switch to a pool, then return to whatever was active before
    public bool PlayReaction(string modeName, float duration)
    {
        int reactionIndex = FindPoolIndex(modeName);
        if (reactionIndex < 0)
        {
            Debug.LogWarning($"ChatController: no pool with mode name \"{modeName}\".", this);
            return false;
        }

        // Remember the pool to return to, but only if a reaction isn't already running
        if (basePoolIndex < 0) basePoolIndex = poolIndex;
        if (revertRoutine != null) StopCoroutine(revertRoutine);

        ApplyPool(reactionIndex);
        revertRoutine = StartCoroutine(RevertAfter(duration));
        return true;
    }

    // ----- Internals -----

    private void ApplyPool(int index)
    {
        if (messagePools == null || messagePools.Length == 0) return;

        poolIndex = ((index % messagePools.Length) + messagePools.Length) % messagePools.Length;
        ChatMessage pool = messagePools[poolIndex];
        if (pool == null) return;

        if (chatMode != null)
            chatMode.text = $"Chat mode: {pool.modeName}";

        Debug.Log($"Chat mode: {pool.modeName}");

        // Stop only the chat loop, not every coroutine on this object
        if (chatRoutine != null)
            StopCoroutine(chatRoutine);

        chatRoutine = StartCoroutine(ChatLoop(pool));
    }

    private IEnumerator RevertAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        int back = basePoolIndex;
        revertRoutine = null;
        basePoolIndex = -1;

        if (back >= 0) ApplyPool(back);
    }

    private void CancelReaction()
    {
        if (revertRoutine != null) StopCoroutine(revertRoutine);
        revertRoutine = null;
        basePoolIndex = -1;
    }

    private int FindPoolIndex(ChatMessage pool)
    {
        return messagePools == null ? -1 : System.Array.IndexOf(messagePools, pool);
    }

    private int FindPoolIndex(string modeName)
    {
        if (messagePools == null) return -1;

        for (int i = 0; i < messagePools.Length; i++)
        {
            if (messagePools[i] != null &&
                string.Equals(messagePools[i].modeName, modeName, System.StringComparison.OrdinalIgnoreCase))
                return i;
        }
        return -1;
    }

    // ---------- Fake chat loop ----------

    private IEnumerator ChatLoop(ChatMessage pool)
    {
        while (true)
        {
            string username = usernames != null ? usernames.GetRandomName().Trim() : "anon";
            string message = pool.GetRandomMessage().Trim();

            if (!string.IsNullOrEmpty(message))
                AddMessage(FormatMessage(username, message));

            yield return new WaitForSeconds(pool.GetRandomDelay());
        }
    }

    private string FormatMessage(string username, string message)
    {
        // Stable color per username, like real chats
        int colorIndex = (username.GetHashCode() & 0x7FFFFFFF) % colors.Length;
        return $"<color={colors[colorIndex]}>{username}</color>: {message}";
    }
}