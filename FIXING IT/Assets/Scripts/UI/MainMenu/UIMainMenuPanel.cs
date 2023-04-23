using UnityEngine;
using UnityEngine.UI;

public class UIMainMenuPanel : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button _playButton;
    [SerializeField] private Button _optionsButton;
    [SerializeField] private Button _exitButton;

    public GameObject FirstSelected => _playButton.gameObject;

    [Header("Broadcasting To")]
    [SerializeField]
    private VoidEventChannelSO _toLobbySelectionEvent;
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
        _toLobbySelectionEvent.RaiseEvent();
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
