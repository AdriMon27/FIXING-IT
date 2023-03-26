using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMainMenuManager : MonoBehaviour
{
    [SerializeField] private UIMainMenuPanel _menuPanel;
    [SerializeField] private UIOptionsPanel _optionsPanel;
    [SerializeField] private UIScreenSettings _screenSettingsPanel;
    [SerializeField] private UIAudioSettings _audioSettingsPanel;

    [Header("Listening To")]
    [SerializeField]
    private VoidEventChannelSO _optionsPanelEvent;
    [SerializeField]
    private VoidEventChannelSO _screenSettingsPanelEvent;
    [SerializeField]
    private VoidEventChannelSO _audioSettingsPanelEvent;
    [SerializeField]
    private VoidEventChannelSO _mainMenuPanelEvent;

    private void OnEnable()
    {
        _optionsPanelEvent.OnEventRaised += ShowOptionsPanel;

        _screenSettingsPanelEvent.OnEventRaised += ShowScreenSettingsPanel;
        _audioSettingsPanelEvent.OnEventRaised += ShowAudioSettingsPanel;
        _mainMenuPanelEvent.OnEventRaised += ShowMainMenuPanel;
    }

    private void OnDisable()
    {
        _optionsPanelEvent.OnEventRaised -= ShowOptionsPanel;

        _screenSettingsPanelEvent.OnEventRaised -= ShowScreenSettingsPanel;
        _audioSettingsPanelEvent.OnEventRaised -= ShowAudioSettingsPanel;
        _mainMenuPanelEvent.OnEventRaised -= ShowMainMenuPanel;
    }

    #region MainMenuPanel events
    private void ShowOptionsPanel()
    {
        _menuPanel.gameObject.SetActive(false);
        _optionsPanel.gameObject.SetActive(true);
    }
    #endregion

    #region OptionsPanel events
    private void ShowScreenSettingsPanel()
    {
        ShowSubMenu(_screenSettingsPanel.GetComponent<RectTransform>());
    }

    private void ShowAudioSettingsPanel()
    {
        ShowSubMenu(_audioSettingsPanel.GetComponent<RectTransform>());
    }
    private void ShowMainMenuPanel()
    {
        DisableSubMenus();

        _optionsPanel.gameObject.SetActive(false);
        _menuPanel.gameObject.SetActive(true);
    }
    #endregion

    private void ShowSubMenu(RectTransform subMenu)
    {
        DisableSubMenus();

        subMenu.gameObject.SetActive(true);
        //possible Lean Tween
    }

    private void DisableSubMenus()
    {
        //possible leantween

        //disable submenus
        _screenSettingsPanel.gameObject.SetActive(false);
        _audioSettingsPanel.gameObject.SetActive(false);
    }
}
