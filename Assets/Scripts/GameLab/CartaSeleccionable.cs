using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CartaVisual))]
public class CartaSeleccionable : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        CartaVisual carta = GetComponent<CartaVisual>();

        Debug.Log("Carta seleccionada: " +
                  carta.Carta.NombreValor + " de " +
                  carta.Carta.Palo);

        if (TableroUI.Instancia != null)
            TableroUI.Instancia.Seleccionar(carta);
    }
}