using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CartaVisual))]
public class CartaSeleccionable : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        if (TableroUI.Instancia != null)
            TableroUI.Instancia.Seleccionar(GetComponent<CartaVisual>());
    }
}