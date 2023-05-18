using FixingIt.Funcs;
using FixingIt.RoomObjects;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FixingIt.UI.Minigame
{
    public class UIManualPanel : MonoBehaviour
    {
        [SerializeField] private GameObject _manualItemPrefab;
        [SerializeField] private Transform[] _recipesParents;

        [SerializeField] private Button _previousPageButton;
        [SerializeField] private Button _nextPageButton;

        [Header("Invoking Func")]
        [SerializeField]
        private ToolRecipeManagerFuncSO _getLevelToolRecipeManagerSOFunc;

        private ToolRecipeManagerSO _toolRecipeManagerSO;
        private int _currentGroupIndex;  // for manage pages

        public GameObject FirstSelected => _previousPageButton.gameObject;

        private void Awake()
        {
            _previousPageButton.onClick.AddListener(() => {
                _currentGroupIndex--;
                ShowCurrentGroup();
            });

            _nextPageButton.onClick.AddListener(() => {
                _currentGroupIndex++;
                ShowCurrentGroup();
            });
        }

        private void OnEnable()
        {
            _currentGroupIndex = 0;
        }

        private void Start()
        {
            _toolRecipeManagerSO = _getLevelToolRecipeManagerSOFunc.RaiseFunc();
        }

        private void ShowCurrentGroup()
        {
            // clear previous transforms
            foreach (Transform parent in _recipesParents) {
                for (int i = parent.childCount - 1; i >= 0; i--) {
                    // Get the child transform
                    Transform childTransform = parent.GetChild(i);

                    // Destroy the child transform
                    DestroyImmediate(childTransform.gameObject);
                }
            }

            _previousPageButton.gameObject.SetActive(true);
            _nextPageButton.gameObject.SetActive(true);

            int numberOfGroups = _toolRecipeManagerSO.Recipes.Length / _recipesParents.Length + 1;

            // avoid overflow just in case
            _currentGroupIndex %= numberOfGroups;

            // hide buttons
            if (_currentGroupIndex == 0)
                _previousPageButton.gameObject.SetActive(false);

            if (_currentGroupIndex >= numberOfGroups - 1)
                _nextPageButton.gameObject.SetActive(false);

            // set images per group
            int loopIndex = 0;
            int realIndex = 0 + (_currentGroupIndex * _recipesParents.Length);
            //for (int i = 0; i < _recipesParents.Length && realIndex < _toolRecipeManagerSO.Recipes.Length; i++) {
            //    UIManualItem manualItem = Instantiate(_manualItemPrefab, _recipesParents[i]).GetComponent<UIManualItem>();
            //    manualItem.InitManualItem(_toolRecipeManagerSO.Recipes[realIndex]);

            //    realIndex = i + (_currentGroupIndex * _recipesParents.Length);
            //}
            while (loopIndex < _recipesParents.Length && realIndex < _toolRecipeManagerSO.Recipes.Length) {
                UIManualItem manualItem = Instantiate(_manualItemPrefab, _recipesParents[loopIndex]).GetComponent<UIManualItem>();
                manualItem.InitManualItem(_toolRecipeManagerSO.Recipes[realIndex]);

                loopIndex++;
                realIndex = loopIndex + (_currentGroupIndex * _recipesParents.Length);
            }

            // set focus
            EventSystem.current.SetSelectedGameObject(_previousPageButton.gameObject.activeSelf ? 
                _previousPageButton.gameObject : _nextPageButton.gameObject // if nextpageButton also deactivated it doesnt matter
            );
        }

        public void Show()
        {
            ShowCurrentGroup();

            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
