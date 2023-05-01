using FixingIt.Funcs;
using ProgramadorCastellano.Events;
using ProgramadorCastellano.MyFuncs;
using UnityEngine;
using UnityEngine.UI;

namespace FixingIt.UI.CharacterSelection
{
    [RequireComponent(typeof(Button))]
    public class UICharacterColorItem : MonoBehaviour
    {
        [SerializeField] private int _colorId;
        [SerializeField] private Image image;
        [SerializeField] private GameObject _selectedGameObject;

        [Header("Broadcasting To")]
        [SerializeField]
        private IntEventChannelSO _changePlayerColorId;

        [Header("Listening To")]
        [SerializeField]
        private VoidEventChannelSO _playerDataNetworkListChangedEvent;

        [Header("Invoking Func")]
        [SerializeField]
        private IntColorFuncSO _getPlayerColorFunc;
        [SerializeField]
        private PlayerdataFuncSO _getClientPlayerDataFunc;

        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(ChangePlayerColorId);
        }

        private void OnEnable()
        {
            _playerDataNetworkListChangedEvent.OnEventRaised += UpdateIsSelected;
        }

        private void OnDisable()
        {
            _playerDataNetworkListChangedEvent.OnEventRaised -= UpdateIsSelected;
        }

        private void Start()
        {
            image.color = _getPlayerColorFunc.RaiseFunc(_colorId);

            UpdateIsSelected();
        }

        private void ChangePlayerColorId()
        {
            _changePlayerColorId.RaiseEvent(_colorId);
        }

        private void UpdateIsSelected()
        {
            if (_getClientPlayerDataFunc.RaiseFunc().ColorId == _colorId) {
                _selectedGameObject.SetActive(true);
            }
            else {
                _selectedGameObject.SetActive(false);
            }
        }
    }
}
