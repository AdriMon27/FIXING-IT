using System;
using System.Collections;
using UnityEngine;

public class UIMainMenuManager : MonoBehaviour
{
    private class PanelLTSequences
    {
        public LTSeq PanelInSeq;
        public LTSeq PanelOutSeq;
        public String Name { get; }

        /// <summary>
        /// Create null InSeq and OutSeq
        /// with Name {name}
        /// </summary>
        public PanelLTSequences(String name)
        {
            PanelInSeq = null;
            PanelOutSeq = null;

            Name = name;
        }

        public bool CancelAll()
        {
            if (PanelInSeq == null) {   //not sequences assigned yet
                Debug.Log($"{Name} PanelInSeq not assigned");
                return false;
            }
            if (PanelOutSeq == null) {
                Debug.Log($"{Name} PanelOutSeq not assigned yet");
                return false;
            }

            LeanTween.cancel(PanelInSeq.id);
            LeanTween.cancel(PanelOutSeq.id);

            return true;
        }
    }

    [Min(1000f)]
    [SerializeField] private float _speedTweens = 1000f;

    private float _offset = 100f;

    // store sequences to cancel them whenever I want
    private PanelLTSequences _mainMenuSeqs;
    private PanelLTSequences _optionsSeqs;
    private PanelLTSequences _screenSettingsSeqs;
    private PanelLTSequences _audioSettingsSeqs;

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
    [SerializeField]
    private VoidEventChannelSO _onCompletedLTSeqEvent;

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

        // create sequences
        _mainMenuSeqs = new PanelLTSequences("MainMenuSeqs");
        _optionsSeqs = new PanelLTSequences("OptionsSeqs");
        _screenSettingsSeqs = new PanelLTSequences("SSSeqs");
        _audioSettingsSeqs = new PanelLTSequences("ASSeqs");
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
        //_optionsSeqs.CancelAll();
        //_mainMenuSeqs.CancelAll();

        RectTransform optionsPanelRT = _optionsPanel.GetComponent<RectTransform>();
        RectTransform mainmenuPanelRT = _menuPanel.GetComponent<RectTransform>();

        // show optionsPanel
        _optionsSeqs.PanelInSeq = CreateOPInSeq(optionsPanelRT);

        // hide mainPanel
        _mainMenuSeqs.PanelOutSeq = CreateMMOutSeq(mainmenuPanelRT);
    }
    #endregion

    #region OptionsPanel events
    private void ShowScreenSettingsPanel()
    {
        //_screenSettingsSeqs.CancelAll();

        RectTransform screenSettingsRT = _screenSettingsPanel.GetComponent<RectTransform>();
        
        HideAudioSettingsPanel();

        _screenSettingsSeqs.PanelInSeq = CreateSSPInSeq(screenSettingsRT);
    }

    private void ShowAudioSettingsPanel()
    {
        //_audioSettingsSeqs.CancelAll();

        RectTransform audioSettingsRT = _audioSettingsPanel.GetComponent<RectTransform>();

        HideScreenSettingsPanel();

        _audioSettingsSeqs.PanelInSeq = CreateASPInSeq(audioSettingsRT);
    }

    private void ShowMainMenuPanel()
    {
        //_mainMenuSeqs.CancelAll();
        //_optionsSeqs.CancelAll();

        DisableSubMenus();

        RectTransform optionsPanelRT = _optionsPanel.GetComponent<RectTransform>();
        RectTransform mainmenuPanelRT = _menuPanel.GetComponent<RectTransform>();

        // show mainPanel
        _mainMenuSeqs.PanelInSeq = CreateMMInSeq(mainmenuPanelRT);

        // hide optionsPanel
        _optionsSeqs.PanelOutSeq = CreateOPOutSeq(optionsPanelRT);
    }
    #endregion

    private void HideAudioSettingsPanel()
    {
        //_audioSettingsSeqs.CancelAll();

        RectTransform audioSettingsRT = _audioSettingsPanel.GetComponent<RectTransform>();

        _audioSettingsSeqs.PanelOutSeq = CreateASPOutSeq(audioSettingsRT);
    }

    private void HideScreenSettingsPanel()
    {;
        //_screenSettingsSeqs.CancelAll();

        RectTransform screenSettingsRT = _screenSettingsPanel.GetComponent<RectTransform>();

        _screenSettingsSeqs.PanelOutSeq = CreateSSPOutSeq(screenSettingsRT);
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
                    _onCompletedLTSeqEvent.RaiseEvent();
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
                _onCompletedLTSeqEvent.RaiseEvent();
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
                _onCompletedLTSeqEvent.RaiseEvent();
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
                _onCompletedLTSeqEvent.RaiseEvent();
            }
        );

        return sequence;
    }
    #endregion
}
