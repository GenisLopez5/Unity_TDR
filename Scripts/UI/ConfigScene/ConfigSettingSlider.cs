using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ConfigSettingSlider : MonoBehaviour
{
    public float sliderValue;
    [SerializeField]
    private TextMeshProUGUI amountText;
    [SerializeField]
    private Slider slider;
    private float sliderPrevoiusValue;

    private void Awake()
    {
        sliderValue = slider.value;
        amountText.text = sliderValue.ToString();
    }

    private void OnEnable()
    {
        ConfigEvents.current.SaveCopies += SaveCopy;
    }

    public void ValueChanged()
    {
        sliderValue = slider.value;
        amountText.text = sliderValue.ToString();
        ConfigEvents.current.SomethingChanged();
    }

    public void SaveCopy()
    {
        sliderPrevoiusValue = sliderValue;
    }

    public void RestoreChanges()
    {
        slider.value = sliderPrevoiusValue;
    }
}
