using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Threading.Tasks;

public class ConfigurationUIManager : MonoBehaviour
{
    public static ConfigurationUIManager current;

    [SerializeField]
    ConfigSetting QualityConfig, BloomConfig, VigneteConfig, MotionblurConfig, LanguageSetting;
    [SerializeField]
    ConfigSettingSlider SensitivityConfig;
    [SerializeField]
    VolumeProfile volume;
    [SerializeField]
    Transform ConfigPanel;

    Bloom bloom;
    Vignette vignette;
    MotionBlur motion;

    bool changing = false;
    bool isOpen = false;

    public int language;

    private void Start()
    {
        volume.TryGet(out bloom);
        volume.TryGet(out vignette);
        volume.TryGet(out motion);

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

        ConfigEvents.current.OnSomethingChanged += SomethingChanged;
        ConfigEvents.current.OnGameLoaded += Apply;
    }

    private void Update()
    {
        if (isOpen && Input.GetKeyDown(KeyCode.Escape)) Hide();
    }

    public void Apply()
    {
        changing = false;

        QualitySettings.SetQualityLevel(QualityConfig.selectedConfig);

        switch (BloomConfig.selectedConfig)
        {
            case 0:
                bloom.active = false;
                break;
            case 1:
                bloom.active = true;
                bloom.highQualityFiltering.Override(false);
                break;
            case 2:
                bloom.active = true;
                bloom.highQualityFiltering.Override(true);
                break;
        }

        switch (VigneteConfig.selectedConfig)
        {
            case 0:
                vignette.active = false;
                break;
            case 1:
                vignette.active = true;
                break;
        }

        switch (MotionblurConfig.selectedConfig)
        {
            case 0:
                motion.active = false;
                break;
            case 1:
                motion.active = true;
                motion.quality.Override(MotionBlurQuality.Low);
                break;
            case 2:
                motion.active = true;
                motion.quality.Override(MotionBlurQuality.Medium);
                break;
            case 3:
                motion.active = true;
                motion.quality.Override(MotionBlurQuality.High);
                break;
        }

        ConfigEvents.current.LanguageChanged(LanguageSetting.selectedConfig);
        ConfigEvents.current.SensitivityChanged(SensitivityConfig.sliderValue);

        language = LanguageSetting.selectedConfig;
    }

    public async void Show()
    {
        if (isOpen) return;

        isOpen = true;

        float v = 0, nx = 1920;

        while (nx > 0.01f)
        {
            nx = Mathf.SmoothDamp(nx, 0, ref v, 0.25f);

            ConfigPanel.localPosition = new Vector3(nx, 0, 0);
            await Task.Yield();
        }
    }

    public async void Hide()
    {
        if (!isOpen) return;
        isOpen = false;

        ConfigEvents.current.ConfigClosed();

        if (changing == true)
        {
            ConfigEvents.current.RestoreAllSettings();
        }

        float v = 0, nx = 0;

        while (nx < 1919.9f)
        {
            nx = Mathf.SmoothDamp(nx, 1920, ref v, 0.25f);
            ConfigPanel.localPosition = new Vector3(nx, 0, 0);

            await Task.Yield();
        }
    }

    void SomethingChanged()
    {
        if (!changing)
        {
            changing = true;

            ConfigEvents.current.SaveAllCopies();
        }
    }
}
