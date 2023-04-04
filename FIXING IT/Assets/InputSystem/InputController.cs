using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputController : MonoBehaviour
{
    [SerializeField] private InputReaderSO _inputReaderSO;
    [SerializeField, Range(0f, 0.3f)]
    [Tooltip("0.3f at minimum TweenSpeed (1000) \nInverse Linear relation")]
    private float _secondsToWaitInTweens = 0.3f;

    // reference to enable it again
    private EventSystem _currentEventSystem;

    [Header("Listening To")]
    [SerializeField]
    private VoidEventChannelSO _onStartedLTSeqEvent;
    [SerializeField]
    private VoidEventChannelSO _onCompletedLTSeqEvent;

    private void OnEnable()
    {
        _onStartedLTSeqEvent.OnEventRaised += DisableAllInput;
        _onCompletedLTSeqEvent.OnEventRaised += WaitAndEnableMenuInput;
    }

    private void OnDisable()
    {
        _onStartedLTSeqEvent.OnEventRaised -= DisableAllInput;
        _onCompletedLTSeqEvent.OnEventRaised -= WaitAndEnableMenuInput;
    }

    private void DisableAllInput()
    {
        _currentEventSystem = EventSystem.current;
        _currentEventSystem.enabled = false;

        _inputReaderSO.DisableAllInput();
    }

    private void WaitAndEnableMenuInput()
    {
        StartCoroutine(EnableMenuInput());
    }

    private IEnumerator EnableMenuInput()
    {
        yield return new WaitForSeconds(_secondsToWaitInTweens);  //necessary wait at minimum speed of tweens (1000)

        _currentEventSystem.enabled = true;

        _inputReaderSO.EnableMenuInput();
    }
}
