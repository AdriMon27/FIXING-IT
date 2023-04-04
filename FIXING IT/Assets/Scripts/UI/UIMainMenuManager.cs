using System;
using System.Collections;
using UnityEngine;

public class UIMainMenuManager : MonoBehaviour
{
    [Min(1000f)]
    [SerializeField] private float _speedTweens = 1000f;

    private float _offset = 100f;

    [Header("Panels")]
    [SerializeField] private UIMainMenuPanel _menuPanel;
    [SerializeField] private UIOptionsPanel _optionsPanel;
    [SerializeField] private UIScreenSettings _screenSettingsPanel;
    [SerializeField] private UIAudioSettings _audioSettingsPanel;

    [Header("Broadcasting To")]
    [SerializeField]
    private VoidEventChannelSO _onStartedLTSeqEvent;
    [SerializeField]
    private VoidEventChannelSO _onCompletedLTSeqEvent;

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

        // show optionsPanel
        CreateOPInSeq(optionsPanelRT);

        // hide mainPanel
        CreateMMOutSeq(mainmenuPanelRT);

        _onStartedLTSeqEvent.RaiseEvent();
    }
    #endregion

    #region OptionsPanel events
    private void ShowScreenSettingsPanel()
    {
        RectTransform screenSettingsRT = _screenSettingsPanel.GetComponent<RectTransform>();
        
        HideAudioSettingsPanel();

        CreateSSPInSeq(screenSettingsRT);

        _onStartedLTSeqEvent.RaiseEvent();
    }

    private void ShowAudioSettingsPanel()
    {
        RectTransform audioSettingsRT = _audioSettingsPanel.GetComponent<RectTransform>();

        HideScreenSettingsPanel();

        CreateASPInSeq(audioSettingsRT);

        _onStartedLTSeqEvent.RaiseEvent();
    }

    private void ShowMainMenuPanel()
    {
        DisableSubMenus();

        RectTransform optionsPanelRT = _optionsPanel.GetComponent<RectTransform>();
        RectTransform mainmenuPanelRT = _menuPanel.GetComponent<RectTransform>();

        // show mainPanel
        CreateMMInSeq(mainmenuPanelRT);

        // hide optionsPanel
        CreateOPOutSeq(optionsPanelRT);

        _onStartedLTSeqEvent.RaiseEvent();
    }
    #endregion

    private void HideAudioSettingsPanel()
    {
        RectTransform audioSettingsRT = _audioSettingsPanel.GetComponent<RectTransform>();

        CreateASPOutSeq(audioSettingsRT);
    }

    private void HideScreenSettingsPanel()
    {
        RectTransform screenSettingsRT = _screenSettingsPanel.GetComponent<RectTransform>();

        CreateSSPOutSeq(screenSettingsRT);
    }

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
    #endregion

    // TODO: Refactor all of these into two general functions
    private LTSeq CreateMMInSeq(RectTransform rectTransform)
    {
        LTSeq sequence = LeanTween.sequence();
        sequence.append(
            () => rectTransform.gameObject.SetActive(true)
        );
        sequence.append(
            LeanTween.move(
                rectTransform,
                Vector2.zero,
                CalculateTime(rectTransform.rect.width)
            )
            .setOnComplete(_onCompletedLTSeqEvent.RaiseEvent)
        );

        return sequence;
    }
    private LTSeq CreateMMOutSeq(RectTransform rectTransform)
    {
        LTSeq sequence = LeanTween.sequence();
        sequence.append(
            LeanTween.move(
                rectTransform,
                GetMainPanelHiddenPos(rectTransform),
                CalculateTime(rectTransform.rect.width)
            )
        );
        sequence.append(
            () =>
                {
                    rectTransform.gameObject.SetActive(false);
                    //_onCompletedLTSeqEvent.RaiseEvent();
                }
        );

        return sequence;
    }
    private LTSeq CreateOPInSeq(RectTransform rectTransform)
    {
        LTSeq sequence = LeanTween.sequence();
        sequence.append(
            () => rectTransform.gameObject.SetActive(true)
        );
        sequence.append(
            LeanTween.move(
                rectTransform,
                Vector2.zero,
                CalculateTime(rectTransform.rect.height)
            )
            .setOnComplete(_onCompletedLTSeqEvent.RaiseEvent)
        );

        return sequence;
    }
    private LTSeq CreateOPOutSeq(RectTransform rectTransform)
    {
        LTSeq sequence = LeanTween.sequence();
        sequence.append(
            LeanTween.move(
                rectTransform,
                GetOptionsPanelHiddenPos(rectTransform),
                CalculateTime(rectTransform.rect.height)
            )
        );
        sequence.append(
            () =>
            {
                rectTransform.gameObject.SetActive(false);
                //_onCompletedLTSeqEvent.RaiseEvent();
            }
        );

        return sequence;
    }
    private LTSeq CreateSSPInSeq(RectTransform rectTransform)
    {
        LTSeq sequence = LeanTween.sequence();
        sequence.append(
            () => rectTransform.gameObject.SetActive(true)
        );
        sequence.append(
            LeanTween.move(
                rectTransform,
                Vector2.zero,
                CalculateTime(rectTransform.rect.height + _offset)
            )
            .setOnComplete(_onCompletedLTSeqEvent.RaiseEvent)
        );

        return sequence;
    }
    private LTSeq CreateSSPOutSeq(RectTransform rectTransform)
    {
        LTSeq sequence = LeanTween.sequence();
        sequence.append(
             LeanTween.move(
                rectTransform,
                GetScreenSettingsPanelHiddenPos(rectTransform),
                CalculateTime(rectTransform.rect.height + _offset)
            )
        );
        sequence.append(
            () =>
            {
                rectTransform.gameObject.SetActive(false);
                //_onCompletedLTSeqEvent.RaiseEvent();
            }
        );

        return sequence;
    }
    private LTSeq CreateASPInSeq(RectTransform rectTransform)
    {
        LTSeq sequence = LeanTween.sequence();
        sequence.append(
            () => rectTransform.gameObject.SetActive(true)
        );
        sequence.append(
            LeanTween.move(
                rectTransform,
                Vector2.zero,
                CalculateTime(rectTransform.rect.height + _offset)
            )
            .setOnComplete(_onCompletedLTSeqEvent.RaiseEvent)
        );

        return sequence;
    }
    private LTSeq CreateASPOutSeq(RectTransform rectTransform)
    {
        LTSeq sequence = LeanTween.sequence();
        sequence.append(
            LeanTween.move(
                rectTransform,
                GetAudioSettingsPanelHiddenPos(rectTransform),
                CalculateTime(rectTransform.rect.height + _offset)
            )
        );
        sequence.append(
            () =>
            {
                rectTransform.gameObject.SetActive(false);
                //_onCompletedLTSeqEvent.RaiseEvent();
            }
        );

        return sequence;
    }
}
