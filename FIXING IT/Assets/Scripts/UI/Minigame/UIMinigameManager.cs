using ProgramadorCastellano.Events;
using UnityEngine;

namespace FixingIt.UI.Minigame
{
    public class UIMinigameManager : MonoBehaviour
    {
        [Header("Listening To")]
        [SerializeField] VoidEventChannelSO _manualCounterUsedEvent;

        private void OnEnable()
        {
            _manualCounterUsedEvent.OnEventRaised += ShowManualPanel;
        }

        private void OnDisable()
        {
            _manualCounterUsedEvent.OnEventRaised -= ShowManualPanel;
        }

        private void ShowManualPanel()
        {
            Debug.Log("UI manual panel jeje");
        }
    }
}
