using UnityEngine;
using UnityEngine.UI;

namespace ProgramadorCastellano.UI
{
    [CreateAssetMenu(menuName = "UI/Selectable UI Skin Data")]
    public class SelectableUISkinDataSO : ScriptableObject
    {
        [SerializeField] private ColorBlock _colors;
        public ColorBlock Colors => _colors;
    }
}