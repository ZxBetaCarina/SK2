using System.Collections.Generic;
using UnityEngine;
using System.IO;
using AstekUtility.ServiceLocatorTool;

public class LanguageManager : MonoBehaviour
{
    public static LanguageManager instance;

    public Dictionary<string, Dictionary<string, string>> languages = new Dictionary<string, Dictionary<string, string>>();
    private string currentLanguage;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        ChangeLanguage(PlayerPrefs.GetString("LanguageChoosed"));
        // Load language files
        LoadLanguage("English");
        LoadLanguage("French");
        LoadLanguage("Russian");
        LoadLanguage("Germany");
        LoadLanguage("Spanish");

        if (LanguageManagerUI.instance != null)
        {
            LanguageManagerUI.instance.UpdateUITexts();
        }
        if (ChangeLanguageScript.instance != null)
        {
            ChangeLanguageScript.instance.UpdateUITextChange();
        }

        // Add more languages as needed
    }


    private void Start()
    {

        if (ServiceLocator.Instance.Get<LanguageChanger>() != null)
            ServiceLocator.Instance.Get<LanguageChanger>().ChangeLanguage(ServiceLocator.Instance.Get<LanguageChanger>().LanguageToIndex(PlayerPrefs.GetString("LanguageChoosed")));
        else
        {
            ChangeLanguage(PlayerPrefs.GetString("LanguageChoosed"));
        }
    }
    public void LoadLanguage(string language)
    {
        string path = Path.Combine(Application.streamingAssetsPath, language + ".txt");

        if (File.Exists(path))
        {
            string[] lines = File.ReadAllLines(path);

            Dictionary<string, string> languageDict = new Dictionary<string, string>();

            foreach (string line in lines)
            {
                string[] parts = line.Split('=');
                if (parts.Length == 2)
                {
                    languageDict.Add(parts[0].Trim(), parts[1].Trim());
                }
            }

            languages[language] = languageDict;

            if (currentLanguage == null)
            {
                currentLanguage = language;
                Debug.Log(language);
            }
        }
        else
        {
            Debug.LogError("Language file not found: " + path);
        }
    }

    public string GetLocalizedString(string key)
    {
        if (languages.ContainsKey(currentLanguage) && languages[currentLanguage].ContainsKey(key))
        {
            return languages[currentLanguage][key];
        }
        else
        {
            Debug.LogWarning("Localization key not found: " + key);
            return key;
        }
    }

    public void ChangeLanguage(string newLanguage)
    {
        if (languages.ContainsKey(newLanguage))
        {
            currentLanguage = newLanguage;
            UpdateUITexts();
        }
        else
        {
            Debug.LogWarning("Language not found: " + newLanguage);
        }
    }

    private void UpdateUITexts()
    {
        if (LanguageManagerUI.instance != null)
        {

            LanguageManagerUI.instance.UpdateUITexts();
        }
        if (ChangeLanguageScript.instance != null)
        {
            ChangeLanguageScript.instance.UpdateUITextChange();
        }
    }

    public IEnumerable<string> GetAvailableLanguages()
    {
        return languages.Keys;
    }
}
