using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace FixingIt.InputSystem
{
//[CreateAssetMenu(fileName = "Input Reader", menuName = "Game/ Input Reader")]
    public class InputReaderSO : ScriptableObject, GameInput.IGameplayActions, GameInput.IMenuActions
    {
        private GameInput _gameInput;

        // Assign delegate{} to events to initialise them with an empty delegate
        // so we can skip the null check when we use them

        // Gameplay
        public event UnityAction<Vector2> MoveEvent = delegate { };
        public event UnityAction InteractEvent = delegate { };
        public event UnityAction AlternateInteractEvent = delegate { };

        // Menu
        public event UnityAction MenuConfirmEvent = delegate { };
        public event UnityAction MenuCancelEvent = delegate { };
        public event UnityAction<Vector2> MenuNavigationEvent = delegate { };

        /*
         * On Enable/Disable Functions
         */
        #region On Enable/Disable
        private void OnEnable()
        {
            if (_gameInput == null) {
                _gameInput = new GameInput();

                // set all callbacks
                _gameInput.Gameplay.SetCallbacks(this);
                _gameInput.Menu.SetCallbacks(this);
            }

            // EnableGameplayInput(); // TODO: se debe manejar de forma externa
        }

        private void OnDisable()
        {
            DisableAllInput();
        }
        #endregion

        /*
         * Turn On/Off Inputs
         */
        #region Turn On/Off Inputs
        public void EnableGameplayInput()
        {
            DisableAllInput();

            _gameInput.Gameplay.Enable();
        }

        public void EnableMenuInput()
        {
            DisableAllInput();

            _gameInput.Menu.Enable();
        }

        public void DisableAllInput()
        {
            _gameInput.Gameplay.Disable();
            _gameInput.Menu.Disable();
        }
        #endregion

        /*
         *  Gameplay Acions
         */
        #region Gameplay Actions
        public void OnMove(InputAction.CallbackContext context)
        {
            MoveEvent.Invoke(context.ReadValue<Vector2>());
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed) {
                InteractEvent.Invoke();
            }
        }

        public void OnAlternateInteract(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed) {
                AlternateInteractEvent.Invoke();
            }
        }
        #endregion

        /*
         *  Menu Actions
         */
        #region Menu Actions
        public void OnSubmit(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed) {
                MenuConfirmEvent.Invoke();
            }
        }

        public void OnCancel(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed) {
                MenuCancelEvent.Invoke();
            }
        }

        public void OnNavigation(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed) {
                MenuNavigationEvent.Invoke(context.ReadValue<Vector2>());
            }
        }
        #endregion
    }
}