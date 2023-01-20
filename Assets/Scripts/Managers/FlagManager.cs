using System.Collections.Generic;
using UnityEngine;

public static class FlagManager
{
    public static FlagDictionary Flags { get => flags; set => flags = value; }

    private static FlagDictionary flags = new FlagDictionary();


    public static bool CheckFlag(string flag)
    {
        flag = SimplifyText(flag);

        if (!flags.TryGetValue(flag, out bool flagValue))
        {
            return false;
        }
        return flagValue;
    }
    public static void SetFlag(string flag, bool value = true)
    {
        flag = SimplifyText(flag);

        if (flags.ContainsKey(flag))
            flags[flag] = value;
        else
            flags.Add(flag, value);

        Debug.Log($"Flag added: (flag: {flag}, value: {value})");
    }
    private static string SimplifyText(string s)
    {
        return s.Trim().ToLower();
    }


    [System.Serializable]
    public class FlagDictionary : SerializableDictionary<string, bool> { }

}