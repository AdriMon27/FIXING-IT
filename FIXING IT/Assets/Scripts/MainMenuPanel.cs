using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuPanel : MonoBehaviour
{
    [SerializeField] private RectTransform _optionsPanel;

    [SerializeField] private Button _playButton;
    [SerializeField] private Button _optionsButton;
    [SerializeField] private Button _exitButton;

    private void Start()
    {
        _playButton.onClick.AddListener(ToLobbySelection);
        _optionsButton.onClick.AddListener(ShowOptionsPanel);
        _exitButton.onClick.AddListener(ExitGame);
    }

    private void ToLobbySelection()
    {
        SceneManager.LoadScene(1);
        Debug.Log("To Lobby Selection");
    }

    private void ShowOptionsPanel()
    {
        _optionsPanel.gameObject.SetActive(true);
        LeanTween.move(_optionsPanel, Vector2.zero, 2f);
        gameObject.SetActive(false);
    }

    private void ExitGame()
    {
        Application.Quit();
        Debug.Log("Exit!");
    }
}
