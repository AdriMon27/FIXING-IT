using FixingIt.RoomObjects.SO;
using UnityEngine;
using UnityEngine.UI;

namespace FixingIt.RoomObjects.Logic
{
    public class ToFixRoomObjectVisualComp : MonoBehaviour
    {
        [SerializeField] private Transform _imagesContainer;
        [SerializeField] private Transform _templateImage;

        private void Start()
        {
            _templateImage.gameObject.SetActive(false);
        }

        public void UpdateTFROVisual(RoomObjectSO[] toolsToBeUsedSO, bool[] toolsUsed)
        {
            // Destroy all childs except template
            foreach (Transform child in _imagesContainer)
            {
                if (child == _templateImage)
                    continue;

                Destroy(child.gameObject);
            }

            // create new child for each tool not used
            for (int i = 0; i < toolsToBeUsedSO.Length; i++)
            {
                if (toolsUsed[i])
                    continue;

                Sprite imageSprite = toolsToBeUsedSO[i].Sprite;

                Transform imageTransform = Instantiate(_templateImage, _imagesContainer);
                imageTransform.gameObject.SetActive(true);
                imageTransform.GetChild(0).GetComponent<Image>().sprite = imageSprite;
            }
        }
    }
}
