using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGame : MonoBehaviour
{
    [SerializeField] private GameSceneSO _sceneToLoad;

    [Header("Broadcasting To")]
    [SerializeField]
    private LoadSceneChannelSO _loadSceneChannel;

    [Header("Listening To")]
    [SerializeField]
    private VoidEventChannelSO _playGameEvent;

    private void OnEnable()
    {
        _playGameEvent.OnEventRaised += LoadFirstScreen;
    }
    private void OnDisable()
    {
        _playGameEvent.OnEventRaised -= LoadFirstScreen;
    }

    private void LoadFirstScreen()
    {
        _loadSceneChannel.RaiseEvent(_sceneToLoad);
    }
}
