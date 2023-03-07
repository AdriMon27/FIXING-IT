using System;
using UnityEngine;
using UnityEngine.UI;

public class OptionsPanel : MonoBehaviour
{
    [SerializeField] private RectTransform _mainMenuPanel;

    [Header("OptionsSubMenus")]
    [SerializeField] private RectTransform _screenSettingsPanel;
    [SerializeField] private RectTransform _volumeSettingsPanel;

    [Header("OptionsButtons")]
    [SerializeField] private Button _screenSettingsButton;
    [SerializeField] private Button _volumeSettingsButton;
    [SerializeField] private Button _backButton;

    private void Start()
    {
        _screenSettingsButton.onClick.AddListener( () => ShowSubMenu(_screenSettingsPanel) );
        _volumeSettingsButton.onClick.AddListener( () => ShowSubMenu(_volumeSettingsPanel) );
        _backButton.onClick.AddListener(BackToMainMenuPanel);
    }

    private void ShowSubMenu(RectTransform subMenu)
    {
        DisableSubMenus();

        subMenu.gameObject.SetActive(true);
        //possible Lean Tween
    }

    private void BackToMainMenuPanel()
    {
        DisableSubMenus();

        _mainMenuPanel.gameObject.SetActive(true);
        LeanTween.move(_mainMenuPanel, Vector2.zero, 2f);
        gameObject.SetActive(false);
    }

    private void DisableSubMenus()
    {
        //possible leantween

        //disable submenus
        _screenSettingsPanel.gameObject.SetActive(false);
        _volumeSettingsPanel.gameObject.SetActive(false);
    }
}
