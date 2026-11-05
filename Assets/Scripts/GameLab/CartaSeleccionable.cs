using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CartaVisual))]
public class CartaSeleccionable : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private float alturaElevada = 40f;
    [SerializeField] private float escalaElevada = 1.1f;

    private RectTransform rect;
    private bool elevada;      // la carta está seleccionada
    private bool aplicada;     // el desplazamiento ya está aplicado
    private Vector2 posAplicada;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (TableroUI.Instancia != null)
            TableroUI.Instancia.Seleccionar(GetComponent<CartaVisual>());
    }

    // Lo llama TableroUI al seleccionar o deseleccionar
    public void SetElevada(bool valor)
    {
        if (valor == elevada) return;

        if (!valor && aplicada)
        {
            rect.anchoredPosition -= Vector2.up * alturaElevada;
            rect.localScale = Vector3.one;
            aplicada = false;
        }

        elevada = valor;
    }

    // Si hay un Layout Group en la mano, recoloca las cartas y borra el desplazamiento.
    // Por eso se reaplica aquí cuando la posición ha cambiado.
    private void LateUpdate()
    {
        if (!elevada) return;
        if (aplicada && rect.anchoredPosition == posAplicada) return;

        rect.anchoredPosition += Vector2.up * alturaElevada;
        rect.localScale = Vector3.one * escalaElevada;
        posAplicada = rect.anchoredPosition;
        aplicada = true;
    }
}