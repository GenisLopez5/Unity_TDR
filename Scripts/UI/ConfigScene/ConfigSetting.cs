using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

public class ConfigSetting : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI SettingNameText;
    [SerializeField]
    private string[] TextValuesENG, TextValuesESP, TextValuesCAT;
    [SerializeField]
    private int valueAmount;
    public int selectedConfig = 0;
    int previousconfig;
    int language;

    private void OnEnable()
    {
        ConfigEvents.current.OnLanguageChanged += LanguageChanged;
        ConfigEvents.current.SaveCopies += SaveCopy;

        UpdateConfigText();
    }

    public void NextConfig()
    {
        if (selectedConfig + 1 > valueAmount - 1)
            selectedConfig = 0;
        else
            selectedConfig += 1;

        UpdateConfigText();
        ConfigEvents.current.SomethingChanged();
    }

    public void PreviousConfig()
    {
        if (selectedConfig - 1 < 0)
            selectedConfig = valueAmount - 1;
        else
            selectedConfig -= 1;

        UpdateConfigText();
        ConfigEvents.current.SomethingChanged();
    }

    public void UpdateConfigText()
    {
        switch (language)
        {
            case 0:
                SettingNameText.text = TextValuesENG[selectedConfig];
                break;
            case 1:
                SettingNameText.text = TextValuesESP[selectedConfig];
                break;
            case 2:
                SettingNameText.text = TextValuesCAT[selectedConfig];
                break;
        }
    }

    void LanguageChanged(int newLanguage)
    {
        language = newLanguage;
        UpdateConfigText();
    }

    public void SaveCopy()
    {
        previousconfig = selectedConfig;
    }

    public void RestoreChanges()
    {
        selectedConfig = previousconfig;
        UpdateConfigText();
    }
}
