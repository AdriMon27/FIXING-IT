using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIMainMenuPanel : MonoBehaviour
{
    [SerializeField] private Button _playButton;
    [SerializeField] private Button _optionsButton;
    [SerializeField] private Button _exitButton;

    [Header("Broadcasting To")]
    [SerializeField]
    private VoidEventChannelSO _playGameEvent;
    [SerializeField]
    private VoidEventChannelSO _optionsPanelEvent;
    [SerializeField]
    private VoidEventChannelSO _exitGameEvent;

    private void Start()
    {
        _playButton.onClick.AddListener(PlayButtonAcion);
        _optionsButton.onClick.AddListener(OptionsButtonAction);
        _exitButton.onClick.AddListener(ExitGameAction);
    }

    private void PlayButtonAcion()
    {
        _playGameEvent.RaiseEvent();
    }

    private void OptionsButtonAction()
    {
        _optionsPanelEvent.RaiseEvent();
    }

    private void ExitGameAction()
    {
        _exitGameEvent.RaiseEvent();
    }
}
