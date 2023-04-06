using System;
using System.Collections;
using UnityEngine;

public class UIMainMenuManager : MonoBehaviour
{
    private enum PanelShowing
    {
        MainPanel,
        OptionsPanel,
        ScreenSettingsPanel,
        AudioSettingsPanel
    }

    [SerializeField] private InputReaderSO _inputReaderSO;
    private PanelShowing _panelShowing;

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
    private GameObjectEventChannelSO _onCompletedLTSeqEvent;

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
        _inputReaderSO.MenuCancelEvent += GoBackAPanel;

        _optionsPanelEvent.OnEventRaised += ShowOptionsPanel;

        _screenSettingsPanelEvent.OnEventRaised += ShowScreenSettingsPanel;
        _audioSettingsPanelEvent.OnEventRaised += ShowAudioSettingsPanel;
        _mainMenuPanelEvent.OnEventRaised += ShowMainMenuPanel;
    }

    private void OnDisable()
    {
        _inputReaderSO.MenuCancelEvent -= GoBackAPanel;

        _optionsPanelEvent.OnEventRaised -= ShowOptionsPanel;

        _screenSettingsPanelEvent.OnEventRaised -= ShowScreenSettingsPanel;
        _audioSettingsPanelEvent.OnEventRaised -= ShowAudioSettingsPanel;
        _mainMenuPanelEvent.OnEventRaised -= ShowMainMenuPanel;
    }

    private void Start()
    {
        _panelShowing = PanelShowing.MainPanel;

        // put hidden menus outside canvas, instant
        RectTransform optionsPanelRT = _optionsPanel.GetComponent<RectTransform>();
        RectTransform screenSettingsRT = _screenSettingsPanel.GetComponent<RectTransform>();
        RectTransform audioSettingsRT = _audioSettingsPanel.GetComponent<RectTransform>();

        LeanTween.move(optionsPanelRT, GetOptionsPanelHiddenPos(optionsPanelRT), 0f);
        LeanTween.move(screenSettingsRT, GetScreenSettingsPanelHiddenPos(screenSettingsRT), 0f);
        LeanTween.move(audioSettingsRT, GetAudioSettingsPanelHiddenPos(audioSettingsRT), 0f);

        StartCoroutine(WaitOneFramesAndHide());
    }

    private void GoBackAPanel()
    {
        switch (_panelShowing)
        {
            case PanelShowing.OptionsPanel:
                ShowMainMenuPanel();
                break;
            case PanelShowing.ScreenSettingsPanel:
                HideScreenSettingsPanel(true);
                break;
            case PanelShowing.AudioSettingsPanel:
                HideAudioSettingsPanel(true);
                break;
            default:
                Debug.LogWarning($"We should be in {PanelShowing.MainPanel} and the Exit is managed by the scene loader" +
                    $"\n PanelShowing: {_panelShowing}");
                break;
        }
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

        _panelShowing = PanelShowing.OptionsPanel;
    }
    #endregion

    #region OptionsPanel events
    private void ShowScreenSettingsPanel()
    {
        RectTransform screenSettingsRT = _screenSettingsPanel.GetComponent<RectTransform>();
        
        HideAudioSettingsPanel();

        CreateSSPInSeq(screenSettingsRT);

        _onStartedLTSeqEvent.RaiseEvent();

        _panelShowing = PanelShowing.ScreenSettingsPanel;
    }

    private void ShowAudioSettingsPanel()
    {
        RectTransform audioSettingsRT = _audioSettingsPanel.GetComponent<RectTransform>();

        HideScreenSettingsPanel();

        CreateASPInSeq(audioSettingsRT);

        _onStartedLTSeqEvent.RaiseEvent();

        _panelShowing = PanelShowing.AudioSettingsPanel;
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

        _panelShowing = PanelShowing.MainPanel;
    }
    #endregion

    private void HideAudioSettingsPanel(bool onlyHiding = false)
    {
        RectTransform audioSettingsRT = _audioSettingsPanel.GetComponent<RectTransform>();

        CreateASPOutSeq(audioSettingsRT, onlyHiding);

        _panelShowing = PanelShowing.OptionsPanel;
    }

    private void HideScreenSettingsPanel(bool onlyHiding = false)
    {
        RectTransform screenSettingsRT = _screenSettingsPanel.GetComponent<RectTransform>();

        CreateSSPOutSeq(screenSettingsRT, onlyHiding);

        _panelShowing = PanelShowing.OptionsPanel;
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
            .setOnComplete(() => _onCompletedLTSeqEvent.RaiseEvent(_menuPanel.FirstSelected))
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
            .setOnComplete(() => _onCompletedLTSeqEvent.RaiseEvent(_optionsPanel.FirstSelected))
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
            .setOnComplete(() => _onCompletedLTSeqEvent.RaiseEvent(_screenSettingsPanel.FirstSelected))
        );

        return sequence;
    }
    private LTSeq CreateSSPOutSeq(RectTransform rectTransform, bool onlyHiding = false)
    {
        if (onlyHiding) {
            _onStartedLTSeqEvent.RaiseEvent();
        }

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
                if (onlyHiding) {
                    _onCompletedLTSeqEvent.RaiseEvent(_optionsPanel.FirstSelected);
                }
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
            .setOnComplete(() => _onCompletedLTSeqEvent.RaiseEvent(_audioSettingsPanel.FirstSelected))
        );

        return sequence;
    }
    private LTSeq CreateASPOutSeq(RectTransform rectTransform, bool onlyHiding = false)
    {
        if (onlyHiding) {
            _onStartedLTSeqEvent.RaiseEvent();
        }

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
                if (onlyHiding) {
                    _onCompletedLTSeqEvent.RaiseEvent(_optionsPanel.FirstSelected);
                }
            }
        );

        return sequence;
    }
}
