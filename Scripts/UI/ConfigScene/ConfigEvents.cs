using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ConfigEvents : MonoBehaviour
{
    public static ConfigEvents current { get; private set; }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Awake()
    {
        if (current != null && current != this)
        {
            Destroy(this);
            return;
        }

        current = this;
    }

    public event Action OnGameLoaded;
    public void GameLoaded()
    {
        OnGameLoaded?.Invoke();
    }

    public event Action<int> OnLanguageChanged;
    public void LanguageChanged(int newLanguage)
    {
        OnLanguageChanged?.Invoke(newLanguage);
    }

    public event Action<float> OnSensitivityChanged;
    public void SensitivityChanged(float newSensitivity)
    {
        OnSensitivityChanged?.Invoke(newSensitivity);
    }

    public event Action OnSomethingChanged;
    public void SomethingChanged()
    {
        OnSomethingChanged?.Invoke();
    }

    public event Action SaveCopies;
    public void SaveAllCopies()
    {
        SaveCopies?.Invoke();
    }

    public event Action RestoreSettings;
    public void RestoreAllSettings()
    {
       RestoreSettings?.Invoke();
    }

    public event Action OnConfigClosed;
    public void ConfigClosed()
    {
        OnConfigClosed?.Invoke();
    }
}
