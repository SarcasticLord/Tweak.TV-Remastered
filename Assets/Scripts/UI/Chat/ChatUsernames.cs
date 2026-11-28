using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "UsernamePool", menuName = "Scriptable Objects/UsernamePool")]
public class ChatUsernames : ScriptableObject
{
    public List<string> usernames = new List<string>();

    [Header("Text File Import")]
    [Tooltip("Optional: a .txt file with one username per line")]
    public TextAsset importFile;

    // Right-click the asset or component header > "Import From Text File"
    [ContextMenu("Import From Text File")]
    public void ImportFromTextFile()
    {
        if (importFile == null)
        {
            Debug.LogWarning($"{name}: assign a text file to Import File first.", this);
            return;
        }

        usernames.Clear();

        string[] lines = importFile.text.Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
        foreach (string line in lines)
        {
            string trimmed = line.Trim();
            if (trimmed.Length > 0)
                usernames.Add(trimmed);
        }

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
        Debug.Log($"{name}: imported {usernames.Count} usernames from {importFile.name}.", this);
    }

    public string GetRandomName()
    {
        if (usernames == null || usernames.Count == 0)
            return "Anonymous";

        return usernames[Random.Range(0, usernames.Count)];
    }
}
