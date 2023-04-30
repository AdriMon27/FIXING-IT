using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FixingIt.UI.MainMenu
{
    public class UIScreenSettings : MonoBehaviour
    {
        private int MAX_INDEX_RESOLUTION => Screen.resolutions.Length - 1;   //is upsidedown

        [SerializeField] private TMP_Dropdown _resolutionsDropdown;
        [SerializeField] private Toggle _fullscreenToggle;
        [SerializeField] private Button _applyButton;
        [SerializeField] private Button _resetButton;

        public GameObject FirstSelected => _resolutionsDropdown.gameObject;

        [Header("Listening To")]
        [SerializeField]
        private ScreenSettingsChannelSO _screenSettingsChanged;

        [Header("Broadcasting To")]
        [SerializeField]
        private ScreenSettingsChannelSO _screenSettingsChannel;

        private void OnEnable()
        {
            _screenSettingsChanged.OnEventRaised += SetVisuals;
        }
        private void OnDisable()
        {
            _screenSettingsChanged.OnEventRaised -= SetVisuals;
        }

        private void Start()
        {
            GetDropdownResolutions();

            _applyButton.onClick.AddListener(ApplyButtonAction);
            _resetButton.onClick.AddListener(ResetButtonAction);
        }

        private void ApplyButtonAction()
        {
            int indexResolution = MAX_INDEX_RESOLUTION - _resolutionsDropdown.value;   //because the dropdown is upsidedown
            _screenSettingsChannel.RaiseEvent(indexResolution, _fullscreenToggle.isOn);
        }

        private void ResetButtonAction()
        {
            // maxResolution index
            // true is fullscreen enabled
            _screenSettingsChannel.RaiseEvent(MAX_INDEX_RESOLUTION, true);
        }

        private void GetDropdownResolutions()
        {
            _resolutionsDropdown.options.Clear();

            Resolution[] resolutions = Screen.resolutions;

            int currentResolutionIndex = 0;
            // Crear una lista de opciones para el dropdown
            List<string> options = new List<string>();
            for (int i = 0; i < resolutions.Length; i++)
            {
                string option = resolutions[i].width + " x " + resolutions[i].height;
                options.Add(option);

                if (resolutions[i].width == Screen.currentResolution.width
                    && resolutions[i].height == Screen.currentResolution.height)
                {

                    currentResolutionIndex = i;
                }
            }

            options.Reverse();  //para mostrar de mayor resolución a menor
            foreach (string option in options)
            {
                _resolutionsDropdown.options.Add(new TMP_Dropdown.OptionData(option));
            }
            //_resolutionsDropdown.value = MAX_INDEX_RESOLUTION - currentResolutionIndex;
            //_resolutionsDropdown.RefreshShownValue();
            SetVisuals(currentResolutionIndex, Screen.fullScreen);
        }

        private void SetVisuals(int indexResolution, bool fullScreen)
        {
            _resolutionsDropdown.value = MAX_INDEX_RESOLUTION - indexResolution;
            _resolutionsDropdown.RefreshShownValue();

            _fullscreenToggle.isOn = fullScreen;
        }
    }
}