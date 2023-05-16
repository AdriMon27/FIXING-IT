using FixingIt.Counters;
using FixingIt.Customer;
using FixingIt.InputSystem;
using FixingIt.RoomObjects;
using ProgramadorCastellano.Funcs;
using UnityEngine;

namespace FixingIt.Minigame
{
    public class FixingGameManager : MonoBehaviour
    {
        [SerializeField] InputReaderSO _inputReaderSO;

        [SerializeField]
        private CustomerCounter[] _customerCounters;
        [SerializeField]
        private RoomObjectSO[] _objectsToFixSO;
        public int TestIndex;

        [Header("Customers")]
        [SerializeField] GameObject _customerPrefab;
        [SerializeField] Transform _customerStartPosition;

        private void Awake()
        {
            _inputReaderSO.EnableGameplayInput();
        }

        private void Start()
        {
            GameObject customerGO = Instantiate(_customerPrefab, _customerStartPosition.position, Quaternion.identity);

            CustomerController customerController = customerGO.GetComponent<CustomerController>();

            if (customerController == null) {
                Debug.LogError($"The prefab {_customerPrefab} is not a Customer Controller");
                return;
            }

            customerController.InitCustomer(_customerStartPosition, _customerCounters[TestIndex], _objectsToFixSO[TestIndex]);
        }
    }
}
