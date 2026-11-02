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
        if (Ocupado || eventData.pointerDrag == null)
        {
            return;
        }

        CartaArrastrable carta =
            eventData.pointerDrag.GetComponent<CartaArrastrable>();

        if (carta == null)
        {
            return;
        }

        Colocar(carta);
        carta.MarcarColocada();
    }

    public void Colocar(CartaArrastrable carta)
    {
        carta.transform.SetParent(transform, false);

        RectTransform rect = carta.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;

        Ocupante = carta.GetComponent<CartaVisual>();
    }

    public void Liberar()
    {
        Ocupante = null;
    }
}