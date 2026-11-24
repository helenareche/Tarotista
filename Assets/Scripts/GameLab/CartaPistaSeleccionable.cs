
using UnityEngine;
using UnityEngine.EventSystems;

public class CartaPistaSeleccionable : MonoBehaviour, IPointerClickHandler
{
    private RepartoBoss repartoBoss;
    private int indice;

    public void Configurar(RepartoBoss reparto, int indiceCarta)
    {
        repartoBoss = reparto;
        indice = indiceCarta;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (repartoBoss == null)
            return;

        repartoBoss.SeleccionarSegundaPista(indice);
    }
}
