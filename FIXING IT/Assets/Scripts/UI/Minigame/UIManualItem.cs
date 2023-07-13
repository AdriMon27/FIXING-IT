using FixingIt.RoomObjects.SO;
using UnityEngine;
using UnityEngine.UI;

namespace FixingIt.UI.Minigame
{
    public class UIManualItem : MonoBehaviour
    {
        [SerializeField] Sprite _emptyPNG;
        [SerializeField] Image _toolImage;
        [SerializeField] Image[] _piecesImages;

        public void InitManualItem(ToolRecipeSO toolRecipeSO)
        {
            // clear all images
            _toolImage.sprite = _emptyPNG;

            foreach (Image image in _piecesImages) {
                image.sprite = _emptyPNG;
            }

            // set all images
            _toolImage.sprite = toolRecipeSO.Output.Sprite;

            for (int i = 0; i < toolRecipeSO.Pieces.Length; i++) {
                _piecesImages[i].sprite = toolRecipeSO.Pieces[i].Sprite;
            }
        }
    }
}
