using ProgramadorCastellano.Events;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FixingIt.UI.Minigame
{
    public class UITimersPanel : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _waitingToStartText;
        [SerializeField] private Image _clockGameplayTimerImage;

        [Header("Listening To")]
        [SerializeField]
        private FloatEventChannelSO _waitingToStartTimerEvent;
        [SerializeField]
        private FloatEventChannelSO _gameplayTimerNormalizedEvent;

        private void OnEnable()
        {
            _waitingToStartTimerEvent.OnEventRaised += ShowWaitingToStartTimer;
            _gameplayTimerNormalizedEvent.OnEventRaised += UpdateClockGameplayTimer;
        }

        private void OnDisable()
        {
            _waitingToStartTimerEvent.OnEventRaised -= ShowWaitingToStartTimer;
            _gameplayTimerNormalizedEvent.OnEventRaised -= UpdateClockGameplayTimer;
        }

        private void Start()
        {
            _waitingToStartText.gameObject.SetActive(false);
        }

        private void ShowWaitingToStartTimer(float timer)
        {
            _waitingToStartText.gameObject.SetActive(true);
            _waitingToStartText.text = Math.Ceiling(timer).ToString();

            if (timer < 0f) {
                _waitingToStartText.gameObject.SetActive(false);
            }
        }

        private void UpdateClockGameplayTimer(float timerNormalized)
        {
            _clockGameplayTimerImage.fillAmount = timerNormalized;
        }
    }
}
