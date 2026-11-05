using UnityEngine;
using UnityEngine.EventSystems;

public class SlotPosicion : MonoBehaviour, IDropHandler
{
    [SerializeField] private TipoPosicion tipo;

    public TipoPosicion Tipo { get { return tipo; } }
    public CartaVisual Ocupante { get; private set; }
    public bool Ocupado { get { return Ocupante != null; } }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null)
        {
            return;
        }

        CartaArrastrable carta =
            eventData.pointerDrag.GetComponent<CartaArrastrable>();

        if (carta == null)
        {
            return;
        }

        if (Ocupado)
        {
            IntercambiarCon(carta);
            return;
        }

        Colocar(carta);
        carta.MarcarColocada();
    }

    // Soltar sobre un hueco ocupado:
    // - si la carta viene de otro hueco, las dos cartas se intercambian
    // - si viene de la mano, la que estaba en el hueco vuelve a la mano
    private void IntercambiarCon(CartaArrastrable entrante)
    {
        CartaArrastrable ocupanteArr = Ocupante.GetComponent<CartaArrastrable>();

        // Durante la resolución las cartas colocadas están bloqueadas
        if (ocupanteArr == null || !ocupanteArr.PuedeArrastrarse)
        {
            return;
        }

        SlotPosicion origen = entrante.SlotOrigen;

        Liberar();

        if (origen != null)
        {
            origen.Colocar(ocupanteArr);
        }
        else if (TableroUI.Instancia != null && TableroUI.Instancia.Mano != null)
        {
            ocupanteArr.transform.SetParent(TableroUI.Instancia.Mano, false);
            ocupanteArr.transform.SetAsLastSibling();
        }
        else
        {
            // Sin mano a la que mandarla, no se hace nada
            Colocar(ocupanteArr);
            return;
        }

        Colocar(entrante);
        entrante.MarcarColocada();
    }

    public void Colocar(CartaArrastrable carta)
    {
        carta.transform.SetParent(transform, false);

        RectTransform rect = carta.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        rect.localScale = Vector3.one;

        Ocupante = carta.GetComponent<CartaVisual>();
    }

    public void Liberar()
    {
        Ocupante = null;
    }
}