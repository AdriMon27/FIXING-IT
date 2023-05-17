using FixingIt.RoomObjects;
using UnityEngine;

namespace FixingIt.UI.Minigame
{
    public class UIManualPanel : MonoBehaviour
    {
        [SerializeField] private GameObject _manualItemPrefab;
        [SerializeField] private Transform[] _recipesParents;

        //[Header("Invoking Func")]
        [SerializeField] private ToolRecipeManagerSO _toolRecipeManagerSO;

        private int _currentGroup;  // for manage pages

        private void ShowCurrentGroup()
        {
            // avoid overflow

            // set images per group
            for (int i = 0; i < _recipesParents.Length && i < _toolRecipeManagerSO.Recipes.Length; i++) {
                UIManualItem manualItem = Instantiate(_manualItemPrefab, _recipesParents[i]).GetComponent<UIManualItem>();

                manualItem.InitManualItem(_toolRecipeManagerSO.Recipes[i]);
            }
        }

        public void Show()
        {
            _currentGroup = 0;

            ShowCurrentGroup();

            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
