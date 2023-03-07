using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScreenSettings : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown resolutionsDropdown;
    [SerializeField] private Toggle fullscreenToggle;

    private void Start()
    {
        resolutionsDropdown.onValueChanged.AddListener((resolutionIndex) => SetScreenResolution(resolutionIndex));
        fullscreenToggle.onValueChanged.AddListener((fullscreenSwitch) => SetFullScreen(fullscreenSwitch));

        SetDropdownResolutions();
        fullscreenToggle.isOn = Screen.fullScreen;
    }

    private void SetFullScreen(bool fullscreenSwitch)
    {
        Screen.fullScreen = fullscreenSwitch;
    }

    private void SetScreenResolution(int resolutionIndex)
    {
        Resolution resolution = Screen.resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    private void SetDropdownResolutions()
    {
        resolutionsDropdown.options.Clear();

        Resolution[] resolutions = Screen.resolutions;

        int currentResolutionIndex = 0;
        // Crear una lista de opciones para el dropdown
        List<string> options = new List<string>();
        for (int i = 0; i < resolutions.Length; i++) {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width
                && resolutions[i].height == Screen.currentResolution.height) {
                
                currentResolutionIndex = i;
            }
        }

        options.Reverse();  //para mostrar de mayor resolución a menor
        foreach (string option in options)
        {
            resolutionsDropdown.options.Add(new TMP_Dropdown.OptionData(option));
        }
        resolutionsDropdown.value = currentResolutionIndex;
        resolutionsDropdown.RefreshShownValue();
    }
}
