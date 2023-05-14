using FixingIt.InputSystem;
using ProgramadorCastellano.Funcs;
using UnityEngine;

namespace FixingIt.Minigame
{
    public class FixingGameManager : MonoBehaviour
    {
        [SerializeField] InputReaderSO _inputReaderSO;

        [SerializeField]
        private Transform[] _customerCounters;
        [SerializeField]
        private int TestIndex;

        [Header("Customers")]
        [SerializeField] GameObject _customerPrefab;
        [SerializeField] Transform _customerStartPosition;

        [Header("Setting Func")]
        [SerializeField] private TransformFuncSO _getCustomerStartPosition;
        [SerializeField] private TransformFuncSO _getFreeCounterPosition;

        private void Awake()
        {
            _inputReaderSO.EnableGameplayInput();

            _getCustomerStartPosition.ClearOnFuncRaised();
            _getCustomerStartPosition.TrySetOnFuncRaised(() => _customerStartPosition);

            _getFreeCounterPosition.ClearOnFuncRaised();
            _getFreeCounterPosition.TrySetOnFuncRaised(() => _customerCounters[TestIndex]);
        }

        private void Start()
        {
            Instantiate(_customerPrefab, _customerStartPosition.position, Quaternion.identity);
        }
    }
}
