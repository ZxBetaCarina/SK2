using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using AstekUtility.ServiceLocatorTool;

public class LanguageChanger : MonoBehaviour, IGameService
{
    public TMP_Dropdown languageDropdown;
    public string selectedLanguage;

    private void Start()
    {
        ServiceLocator.Instance.Register(this);
        selectedLanguage = PlayerPrefs.GetString("LanguageChoosed");
        if (languageDropdown != null)
        {
            List<string> availableLanguages = new List<string>(LanguageManager.instance.GetAvailableLanguages());
            languageDropdown.ClearOptions();
            languageDropdown.AddOptions(availableLanguages);
            languageDropdown.onValueChanged.AddListener(ChangeLanguage);
        }
        languageDropdown.SetValueWithoutNotify(LanguageToIndex(selectedLanguage));
    }

    private void OnDestroy()
    {
        ServiceLocator.Instance.Unregister<LanguageChanger>();
    }

    public void ChangeLanguage(int index)
    {
        selectedLanguage = languageDropdown.options[index].text;
        LanguageManager.instance.ChangeLanguage(selectedLanguage);
        PlayerPrefs.SetString("LanguageChoosed", selectedLanguage);
    }

    public int LanguageToIndex(string language)
    {
        switch (language)
        {
            case "English":

                return 0;

            case "French":

                return 1;

            case "Russian":

                return 2;

            case "Germany":

                return 3;

            case "Spanish":

                return 4;

            default:
                return 0;
        }
    }
}
