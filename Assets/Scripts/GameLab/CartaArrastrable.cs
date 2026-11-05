using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CanvasGroup))]
public class CartaArrastrable : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private CanvasGroup grupo;
    private RectTransform rect;
    private RectTransform rectCanvas;

    private Transform padreInicial;
    private int indiceInicial;
    private SlotPosicion slotOrigen;
    private bool soltadaEnSlot;

    // Poner a false cuando se resuelve la ronda para que no se puedan mover
    public bool PuedeArrastrarse { get; set; } = true;

    private void Awake()
    {
        grupo = GetComponent<CanvasGroup>();
        rect = GetComponent<RectTransform>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!PuedeArrastrarse)
        {
            eventData.pointerDrag = null; // cancela el arrastre
            return;
        }

        // Al arrastrar, la carta deja de estar seleccionada (y vuelve a su altura)
        if (TableroUI.Instancia != null)
            TableroUI.Instancia.Deseleccionar();

        soltadaEnSlot = false;
        padreInicial = transform.parent;
        indiceInicial = transform.GetSiblingIndex();

        slotOrigen = padreInicial.GetComponent<SlotPosicion>();
        if (slotOrigen != null)
        {
            slotOrigen.Liberar();
        }

        // Se saca al Canvas raíz para que se dibuje por encima de todo
        Canvas canvas = GetComponentInParent<Canvas>().rootCanvas;
        rectCanvas = canvas.GetComponent<RectTransform>();
        transform.SetParent(canvas.transform, true);
        transform.SetAsLastSibling();

        // Sin esto la propia carta bloquea el ratón y el hueco no detecta el soltado
        grupo.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 mundo;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(
            rectCanvas, eventData.position, eventData.pressEventCamera, out mundo))
        {
            transform.position = mundo;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        grupo.blocksRaycasts = true;

        if (!soltadaEnSlot)
        {
            VolverAlOrigen();
        }
    }

    // Hueco del que viene la carta (null si venía de la mano)
    public SlotPosicion SlotOrigen
    {
        get { return slotOrigen; }
    }

    // Lo llama el SlotPosicion cuando acepta la carta
    public void MarcarColocada()
    {
        soltadaEnSlot = true;
    }

    // Lo llama ZonaMano al soltar la carta sobre la mano
    public void SoltarEnMano(Transform mano)
    {
        transform.SetParent(mano, false);

        if (slotOrigen == null)
        {
            // Venía de la mano: recupera su sitio
            transform.SetSiblingIndex(Mathf.Min(indiceInicial, mano.childCount - 1));
        }
        else
        {
            transform.SetAsLastSibling();
        }

        RectTransform r = GetComponent<RectTransform>();
        r.localScale = Vector3.one;

        MarcarColocada();
    }

    private void VolverAlOrigen()
    {
        if (slotOrigen != null)
        {
            slotOrigen.Colocar(this);
        }
        else
        {
            transform.SetParent(padreInicial, false);
            transform.SetSiblingIndex(indiceInicial);
        }
    }
}