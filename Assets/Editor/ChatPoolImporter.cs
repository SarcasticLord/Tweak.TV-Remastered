#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

// Put this file in a folder named "Editor" (e.g. Assets/Editor/ChatPoolImporter.cs)
public static class ChatPoolImporter
{
    private const string MenuPath = "Assets/Create/Chat Pools From Selected Text Files";

    [MenuItem(MenuPath)]
    private static void CreatePools()
    {
        int created = 0;

        foreach (Object obj in Selection.objects)
        {
            TextAsset text = obj as TextAsset;
            if (text == null) continue;

            string txtPath = AssetDatabase.GetAssetPath(text);
            string folder = Path.GetDirectoryName(txtPath);
            string assetPath = AssetDatabase.GenerateUniqueAssetPath(
                Path.Combine(folder, text.name + "Pool.asset").Replace('\\', '/'));

            ChatMessage pool = ScriptableObject.CreateInstance<ChatMessage>();
            pool.modeName = text.name;   // rename in the inspector if you like
            pool.importFile = text;
            pool.ImportFromTextFile();

            AssetDatabase.CreateAsset(pool, assetPath);
            created++;
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"Created {created} chat pool asset(s).");
    }

    [MenuItem(MenuPath, true)]
    private static bool ValidateCreatePools()
    {
        foreach (Object obj in Selection.objects)
            if (obj is TextAsset) return true;
        return false;
    }
}
#endif
