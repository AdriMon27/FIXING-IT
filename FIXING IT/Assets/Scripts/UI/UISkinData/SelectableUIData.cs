using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Selectable))]
public class SelectableUIData : MonoBehaviour
{
    [SerializeField] private SelectableUISkinDataSO _skinDataSO;

    private Selectable _selectable;

    private void Awake()
    {
        //_selectable = GetComponent<Selectable>();

        OnSkinUI();
    }

    private void OnSkinUI()
    {
        _selectable = GetComponent<Selectable>();
        Debug.Log(_selectable.gameObject.name);

        _selectable.colors = _skinDataSO.Colors;
    }


    // COMMENT THE UPDATE AFTER DESGIN
    private void Update()
    {
        if (Application.isEditor)
        {
            OnSkinUI();
        }
    }
}
