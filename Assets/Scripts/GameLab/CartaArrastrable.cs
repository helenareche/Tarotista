
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CanvasGroup))]
public class CartaArrastrable : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler,
    IPointerClickHandler
{
    private CanvasGroup grupo;
    private RectTransform rect;
    private RectTransform rectCanvas;

    private Transform padreInicial;
    private int indiceInicial;
    private SlotPosicion slotOrigen;
    private bool soltadaEnSlot;

    public bool PuedeArrastrarse { get; set; } = true;

    private void Awake()
    {
        grupo = GetComponent<CanvasGroup>();
        rect = GetComponent<RectTransform>();
    }

    // NUEVO: retirar una carta colocada haciendo clic
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!PuedeArrastrarse)
            return;

        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        SlotPosicion slot =
            transform.parent.GetComponent<SlotPosicion>();

        if (slot != null && TableroUI.Instancia != null)
        {
            TableroUI.Instancia.DevolverCartaAMano(this);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!PuedeArrastrarse)
        {
            eventData.pointerDrag = null;
            return;
        }

        soltadaEnSlot = false;
        padreInicial = transform.parent;
        indiceInicial = transform.GetSiblingIndex();

        slotOrigen = padreInicial.GetComponent<SlotPosicion>();

        if (slotOrigen != null)
        {
            slotOrigen.Liberar();
        }

        Canvas canvas = GetComponentInParent<Canvas>().rootCanvas;
        rectCanvas = canvas.GetComponent<RectTransform>();

        transform.SetParent(canvas.transform, true);
        transform.SetAsLastSibling();

        grupo.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!PuedeArrastrarse || rectCanvas == null)
            return;

        Vector3 mundo;

        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(
            rectCanvas,
            eventData.position,
            eventData.pressEventCamera,
            out mundo))
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

    public void MarcarColocada()
    {
        soltadaEnSlot = true;
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
