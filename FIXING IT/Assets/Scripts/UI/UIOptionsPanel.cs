using System;
using UnityEngine;
using UnityEngine.UI;

public class UIOptionsPanel : MonoBehaviour
{
    [SerializeField] private Button _screenSettingsButton;
    [SerializeField] private Button _volumeSettingsButton;
    [SerializeField] private Button _backButton;

    public GameObject FirstSelected => _screenSettingsButton.gameObject;

    [Header("Broadcasting To")]
    [SerializeField]
    private VoidEventChannelSO _screenSettingsPanelEvent;
    [SerializeField]
    private VoidEventChannelSO _audioSettingsPanelEvent;
    [SerializeField]
    private VoidEventChannelSO _mainMenuPanelEvent;

    private void Start()
    {
        _screenSettingsButton.onClick.AddListener(ScreenSettingsButtonAction);
        _volumeSettingsButton.onClick.AddListener(AudioSettingsButtonAction);
        _backButton.onClick.AddListener(BackButtonAction);
    }

    private void ScreenSettingsButtonAction()
    {
        _screenSettingsPanelEvent.RaiseEvent();
    }

    private void AudioSettingsButtonAction()
    {
        _audioSettingsPanelEvent.RaiseEvent();
    }

    private void BackButtonAction()
    {
        _mainMenuPanelEvent.RaiseEvent();
    }

    //private void ShowSubMenu(RectTransform subMenu)
    //{
    //    DisableSubMenus();

    //    subMenu.gameObject.SetActive(true);
    //    //possible Lean Tween
    //}

    //private void BackToMainMenuPanel()
    //{
    //    DisableSubMenus();

    //    _mainMenuPanel.gameObject.SetActive(true);
    //    LeanTween.move(_mainMenuPanel, Vector2.zero, 2f);
    //    gameObject.SetActive(false);
    //}

    //private void DisableSubMenus()
    //{
    //    //possible leantween

    //    //disable submenus
    //    _screenSettingsPanel.gameObject.SetActive(false);
    //    _volumeSettingsPanel.gameObject.SetActive(false);
    //}
}
