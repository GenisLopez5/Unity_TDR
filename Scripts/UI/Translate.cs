using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

public class Translate : MonoBehaviour
{
    [SerializeField]
    private string[] text;
    [SerializeField]
    private TextMeshProUGUI TMPtext;
    int currentLanguage;

    private async void Awake()
    {
        while (!FindObjectOfType<ConfigEvents>())
        {
            await Task.Delay(100);
        }

        currentLanguage = ConfigurationUIManager.current.language;
        ConfigEvents.current.OnLanguageChanged += LanguageChanged;
        ChangeLanguage();
    }

    void LanguageChanged(int newLanguage)
    {
        currentLanguage = newLanguage;
        ChangeLanguage();
    }

    private async void OnEnable()
    {
        while (!FindObjectOfType<ConfigEvents>())
        {
            await Task.Delay(100);
        }

        currentLanguage = ConfigurationUIManager.current.language;
        ConfigEvents.current.OnLanguageChanged += LanguageChanged;
        ChangeLanguage();
    }

    void ChangeLanguage()
    {
        if (text[currentLanguage] == null)
            Debug.LogWarning(("L'objecte {0} no te una traducció per a l'idioma {1}", gameObject, currentLanguage));
        else
            TMPtext.text = text[currentLanguage];
    }
}
