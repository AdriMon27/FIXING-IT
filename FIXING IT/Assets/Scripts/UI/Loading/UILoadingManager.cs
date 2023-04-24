using UnityEngine;

public class UILoadingManager : MonoBehaviour
{
    [SerializeField] private GameObject _loadingPanel;

    [Header("Listening to")]
    [SerializeField]
    private VoidEventChannelSO _startSceneLoadingEvent;
    [SerializeField]
    private VoidEventChannelSO _sceneLoadedEvent;

    private void Start()
    {
        _loadingPanel.SetActive(false);
    }

    private void OnEnable()
    {
        _startSceneLoadingEvent.OnEventRaised += ToggleLoadingPanel;
        _sceneLoadedEvent.OnEventRaised += ToggleLoadingPanel;
    }

    private void OnDisable()
    {
        _startSceneLoadingEvent.OnEventRaised -= ToggleLoadingPanel;
        _sceneLoadedEvent.OnEventRaised -= ToggleLoadingPanel;
    }

    private void ToggleLoadingPanel()
    {
        _loadingPanel.SetActive(!_loadingPanel.activeSelf);
    }
}
