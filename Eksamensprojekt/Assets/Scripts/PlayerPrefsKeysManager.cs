using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerPrefsKeysManager
{
    private static List<string> allKeys = new List<string>();

    private static void SaveKeys()
    {
        string json = JsonUtility.ToJson(new ListContainer { Keys = allKeys });
        PlayerPrefs.SetString("AllKeys", json);
        PlayerPrefs.Save();
    }

    private static void LoadKeys()
    {
        string json = PlayerPrefs.GetString("AllKeys", "");
        if (!string.IsNullOrEmpty(json))
        {
            ListContainer container = JsonUtility.FromJson<ListContainer>(json);
            allKeys = container.Keys ?? new List<string>();
        }
    }

    public static void RegisterKey(string key)
    {
        if (!allKeys.Contains(key))
        {
            allKeys.Add(key);
            SaveKeys();  // Save keys whenever a new one is added
        }
    }

    public static void UnregisterKey(string key)
    {
        if (allKeys.Contains(key))
        {
            allKeys.Remove(key);
            SaveKeys();  // Save keys whenever one is removed
        }
    }

    public static List<string> GetAllKeys()
    {
        return new List<string>(allKeys);
    }

    [System.Serializable]
    private class ListContainer
    {
        public List<string> Keys;
    }

    // Call this at the start of your application
    public static void Initialize()
    {
        LoadKeys();
    }
}