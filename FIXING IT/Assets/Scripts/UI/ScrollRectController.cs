using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScrollRectController : MonoBehaviour
{
    private ScrollRect _scrollRect;
    private RectTransform _selectedRectTransform;
    public float scrollSpeed = 10f;

    private void Start()
    {
        _scrollRect = GetComponent<ScrollRect>();
    }

    public void ScrollToSelected(RectTransform selectedRT)
    {
        _selectedRectTransform = selectedRT;

        if (_scrollRect == null || _selectedRectTransform == null)
            return;

        Debug.Log("pase el return");

        // Obtenemos la posición del elemento seleccionado en el ScrollRect
        Vector3[] corners = new Vector3[4];
        _selectedRectTransform.GetWorldCorners(corners);
        Vector3 selectedPos = _scrollRect.viewport.InverseTransformPoint(corners[0]);

        // Obtenemos la posición actual del contenido del ScrollRect
        Vector2 contentPos = _scrollRect.content.anchoredPosition;

        // Calculamos la diferencia entre la posición actual y la del elemento seleccionado
        Vector2 diff = new Vector2(0f, selectedPos.y) - contentPos;

        // Si la diferencia es menor que 0, significa que el elemento está arriba del ScrollRect
        if (diff.y < 0)
        {
            // Hacemos scroll hacia arriba
            contentPos += Vector2.up * scrollSpeed * Time.deltaTime;

            // Si nos hemos pasado del elemento seleccionado, lo fijamos en la posición correcta
            if (contentPos.y <= selectedPos.y)
                contentPos.y = selectedPos.y;
        }
        // Si la diferencia es mayor que la altura del ScrollRect, significa que el elemento está debajo
        else if (diff.y > _scrollRect.viewport.rect.height)
        {
            // Hacemos scroll hacia abajo
            contentPos += Vector2.down * scrollSpeed * Time.deltaTime;

            // Si nos hemos pasado del elemento seleccionado, lo fijamos en la posición correcta
            if (contentPos.y >= selectedPos.y)
                contentPos.y = selectedPos.y;
        }

        // Actualizamos la posición del contenido del ScrollRect
        _scrollRect.content.anchoredPosition = contentPos;
    }
}
