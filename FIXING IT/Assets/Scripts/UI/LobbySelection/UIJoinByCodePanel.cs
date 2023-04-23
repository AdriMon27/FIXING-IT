using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIJoinByCodePanel : MonoBehaviour
{
    [SerializeField] private TMP_InputField _joinCodeInputField;
    [SerializeField] private Button _joinButton;
    [SerializeField] private Button _cancelButton;

    public GameObject FirstSelected => _joinCodeInputField.gameObject;

    [Header("Broadcasting To")]
    [SerializeField]
    private StringEventChannelSO _joinByCodeEvent;
    [SerializeField]
    private VoidEventChannelSO _cancelJoinByCodeEvent;

    private void Start()
    {
        _joinButton.onClick.AddListener(JoinByCode);
        _cancelButton.onClick.AddListener(CancelJoinByCode);
    }

    private void JoinByCode()
    {
        string joinCode = _joinCodeInputField.text;

        _joinByCodeEvent.RaiseEvent(joinCode);
    }

    private void CancelJoinByCode()
    {
        _cancelJoinByCodeEvent.RaiseEvent();
    }
}
