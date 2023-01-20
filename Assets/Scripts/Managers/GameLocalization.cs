using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class GameLocalization
{
    private static readonly Dictionary<string, Dictionary<string, string>> LanguageDictionaries = new Dictionary<string, Dictionary<string, string>>();

    public static string selectedLanguage = "English";

    public static void LoadLanguage()
    {
        LoadLanguage(selectedLanguage);
    }
    public static void LoadLanguage(string language)
    {
        Debug.Log($"Loading localization for language: {language}...");

        if (LanguageDictionaries.ContainsKey(language))
        {
            Debug.LogError($"{language} was arleady loaded!");
            return;
        }
        LanguageDictionaries.Add(language, new Dictionary<string, string>());

        string path = $"{Application.dataPath}/Resources/Localization/{language}";

        string[] allfiles = Directory.GetFiles(path, "*.csv", SearchOption.AllDirectories);


        foreach (string file in allfiles)
        {
            using(StreamReader sr = new StreamReader(file))
            {
                string csv = sr.ReadToEnd();
                var csvItems = csv.Split('\n');
                var dictionary = new Dictionary<string, string>();

                foreach (var item in csvItems)
                {
                    if (!string.IsNullOrWhiteSpace(item))
                    {
                        var itemParts = item.Split(';');

                        string id = SimplifyString( itemParts[0] );
                        string value = itemParts[1];

                        if (LanguageDictionaries[language].ContainsKey(id))
                        {
                            Debug.LogError($"The localization id has been duplicated: {id}");
                            continue;
                        }

                        if(!string.IsNullOrWhiteSpace(itemParts[0]))
                            LanguageDictionaries[language].Add(id, value);
                    }
                }
            }
        }
    }

    public static string GetText(string text)
    {
        string simplifiedText = SimplifyString(text);

        if (LanguageDictionaries[selectedLanguage].TryGetValue(simplifiedText, out string localizedText))
        {
            return localizedText;
        }
        return text;
    }

#if UNITY_EDITOR
    /// <summary>
    /// Returns localized text, and also loads the localization files. 
    /// Should be ONLY used for development
    /// </summary>
    public static string LoadAndGetText(string text)
    {
        LanguageDictionaries.Clear();
        //if (!LanguageDictionaries.ContainsKey("English"))
        {
            LoadLanguage("English");
        }
        return GetText(text);
    }
#endif


    private static string SimplifyString(string s)
    {
        return s.Trim().ToLower();
    }
}
