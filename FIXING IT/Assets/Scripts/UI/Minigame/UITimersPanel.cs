using ProgramadorCastellano.Events;
using System;
using TMPro;
using UnityEngine;

namespace FixingIt.UI.Minigame
{
    public class UITimersPanel : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _waitingToStartText;

        [Header("Listening To")]
        [SerializeField] private FloatEventChannelSO _waitingToStartTimerEvent;

        private void OnEnable()
        {
            _waitingToStartTimerEvent.OnEventRaised += ShowWaitingToStartTimer;
        }

        private void OnDisable()
        {
            _waitingToStartTimerEvent.OnEventRaised -= ShowWaitingToStartTimer;
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
    }
}
