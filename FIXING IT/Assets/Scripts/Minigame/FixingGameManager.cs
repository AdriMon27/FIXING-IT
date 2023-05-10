using FixingIt.InputSystem;
using UnityEngine;

namespace FixingIt
{
    public class FixingGameManager : MonoBehaviour
    {
        [SerializeField] InputReaderSO _inputReaderSO;

        private void Awake()
        {
            _inputReaderSO.EnableGameplayInput();
        }
    }
}
