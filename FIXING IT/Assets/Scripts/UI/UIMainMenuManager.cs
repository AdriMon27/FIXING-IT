using System;
using System.Collections;
using UnityEngine;

public class UIMainMenuManager : MonoBehaviour
{
    [Range(1000f, 2000f)]
    [SerializeField] private float _speedTweens = 1000f;

    private float _offset = 100f;

    [Header("Panels")]
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

    private void Start()
    {
        // put hidden menus outside canvas, instant
        RectTransform optionsPanelRT = _optionsPanel.GetComponent<RectTransform>();
        RectTransform screenSettingsRT = _screenSettingsPanel.GetComponent<RectTransform>();
        RectTransform audioSettingsRT = _audioSettingsPanel.GetComponent<RectTransform>();

        LeanTween.move(optionsPanelRT, GetOptionsPanelHiddenPos(optionsPanelRT), 0f);
        LeanTween.move(screenSettingsRT, GetScreenSettingsPanelHiddenPos(screenSettingsRT), 0f);
        LeanTween.move(audioSettingsRT, GetAudioSettingsPanelHiddenPos(audioSettingsRT), 0f);

        StartCoroutine(WaitOneFramesAndHide());
    }

    private IEnumerator WaitOneFramesAndHide()
    {
        yield return null;
        
        _optionsPanel.gameObject.SetActive(false);
        _screenSettingsPanel.gameObject.SetActive(false);
        _audioSettingsPanel.gameObject.SetActive(false);
    }

    #region MainMenuPanel events
    private void ShowOptionsPanel()
    {
        RectTransform optionsPanelRT = _optionsPanel.GetComponent<RectTransform>();
        RectTransform mainmenuPanelRT = _menuPanel.GetComponent<RectTransform>();

        // just in case
        LeanTween.cancel(optionsPanelRT);
        LeanTween.cancel(mainmenuPanelRT);

        // show optionsPanel
        var optionsInSequence = LeanTween.sequence();
        optionsInSequence.append(
            () => _optionsPanel.gameObject.SetActive(true)    
        );
        optionsInSequence.append(
            LeanTween.move(
                optionsPanelRT,
                Vector2.zero,
                CalculateTime(optionsPanelRT.rect.height)
            )
        );

        // hide mainPanel
        var menuOutSequence = LeanTween.sequence();
        menuOutSequence.append(
            LeanTween.move(
                mainmenuPanelRT,
                GetMainPanelHiddenPos(mainmenuPanelRT),
                CalculateTime(mainmenuPanelRT.rect.width)
            )
        );
        menuOutSequence.append(
            () => _menuPanel.gameObject.SetActive(false)
        );
    }
    #endregion

    #region OptionsPanel events
    private void ShowScreenSettingsPanel()
    {
        RectTransform screenSettingsRT = _screenSettingsPanel.GetComponent<RectTransform>();
        
        HideAudioSettingsPanel();

        LeanTween.cancel(screenSettingsRT); // try to cancel just in case

        var screenSettingsInSequence = LeanTween.sequence();
        screenSettingsInSequence.append(
            () => screenSettingsRT.gameObject.SetActive(true)
        );
        screenSettingsInSequence.append(
            LeanTween.move(
                screenSettingsRT,
                Vector2.zero,
                CalculateTime(screenSettingsRT.rect.height + _offset)
            )
        );
    }

    private void ShowAudioSettingsPanel()
    {
        RectTransform audioSettingsRT = _audioSettingsPanel.GetComponent<RectTransform>();

        HideScreenSettingsPanel();

        LeanTween.cancel(audioSettingsRT);  // try to cancel just in case

        var audioSettingsInSequence = LeanTween.sequence();
        audioSettingsInSequence.append(
            () => audioSettingsRT.gameObject.SetActive(true)
        );
        audioSettingsInSequence.append(
            LeanTween.move(
                audioSettingsRT,
                Vector2.zero,
                CalculateTime(audioSettingsRT.rect.height + _offset)
            )
        );
    }

    private void ShowMainMenuPanel()
    {
        DisableSubMenus();

        RectTransform optionsPanelRT = _optionsPanel.GetComponent<RectTransform>();
        RectTransform mainmenuPanelRT = _menuPanel.GetComponent<RectTransform>();

        // just in case
        LeanTween.cancel(optionsPanelRT);
        LeanTween.cancel(mainmenuPanelRT);

        // show mainPanel
        var mainMenuInSequence = LeanTween.sequence();
        mainMenuInSequence.append(
            () => _menuPanel.gameObject.SetActive(true)
        );
        mainMenuInSequence.append(
            LeanTween.move(
                mainmenuPanelRT,
                Vector2.zero,
                CalculateTime(mainmenuPanelRT.rect.width)
            )
        );

        // hide optionsPanel
        var optionsOutSequence = LeanTween.sequence();
        optionsOutSequence.append(
            LeanTween.move(
                optionsPanelRT,
                GetOptionsPanelHiddenPos(optionsPanelRT),
                CalculateTime(optionsPanelRT.rect.height)
            )
        );
        optionsOutSequence.append(
            () => _optionsPanel.gameObject.SetActive(false)
        );
    }
    #endregion

    private void DisableSubMenus()
    {
        HideScreenSettingsPanel();

        HideAudioSettingsPanel();
    }

    #region Tween Calculations
    private Vector2 GetMainPanelHiddenPos(RectTransform mainMenuPanelRT)
    {
        return new Vector2(-mainMenuPanelRT.rect.width, 0f);
    }
    private Vector2 GetOptionsPanelHiddenPos(RectTransform optionsPanelRT)
    {
        return new Vector2(0f, -optionsPanelRT.rect.height);
    }
    private Vector2 GetScreenSettingsPanelHiddenPos(RectTransform screenSettingsRT)
    {
        return new Vector2(0f, (screenSettingsRT.rect.height + _offset));
    }
    private Vector2 GetAudioSettingsPanelHiddenPos(RectTransform audioSettingsRT)
    {
        return new Vector2(0f, -(audioSettingsRT.rect.height + _offset));
    }
    
    private float CalculateTime(float distance)
    {
        // speed = distance / time
        return distance / _speedTweens;
    }

    private void HideAudioSettingsPanel()
    {
        RectTransform audioSettingsRT = _audioSettingsPanel.GetComponent<RectTransform>();

        LeanTween.cancel(audioSettingsRT);  //cancel just in case
        LeanTween.cancel(audioSettingsRT);  //twice for the movement tween and the setactive tween

        var audioSettingsOutSequence = LeanTween.sequence();
        audioSettingsOutSequence.append(
            LeanTween.move(
                audioSettingsRT,
                GetAudioSettingsPanelHiddenPos(audioSettingsRT),
                CalculateTime(audioSettingsRT.rect.height + _offset)
            )
        );
        audioSettingsOutSequence.append(
            () => audioSettingsRT.gameObject.SetActive(false)
        );
    }

    private void HideScreenSettingsPanel()
    {
        RectTransform screenSettingsRT = _screenSettingsPanel.GetComponent<RectTransform>();

        LeanTween.cancel(screenSettingsRT); //cancel just in case

        var screenSettingsOutSequence = LeanTween.sequence();
        screenSettingsOutSequence.append(
            LeanTween.move(
                screenSettingsRT,
                GetScreenSettingsPanelHiddenPos(screenSettingsRT),
                CalculateTime(screenSettingsRT.rect.height + _offset)
            )
        );
        screenSettingsOutSequence.append(
            () => screenSettingsRT.gameObject.SetActive(false)
        );
    }
    #endregion
}
