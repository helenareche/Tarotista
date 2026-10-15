using UnityEngine;

public class TableroUI : MonoBehaviour
{
    public static TableroUI Instancia { get; private set; }

    [SerializeField] private Mazo mazo;
    [SerializeField] private Transform posicionCarta;

    private CartaVisual seleccionada;

    private void Awake()
    {
        Instancia = this;
    }

    public void Seleccionar(CartaVisual carta)
    {
        seleccionada = carta;
    }

    public void ColocarSeleccionada()
    {
        if (seleccionada == null || mazo == null ||
            posicionCarta == null || posicionCarta.childCount > 0)
            return;

        CartaVisual carta = seleccionada;
        seleccionada = null;

        carta.transform.SetParent(posicionCarta, false);

        RectTransform rect = carta.GetComponent<RectTransform>();
        if (rect != null)
            rect.anchoredPosition = Vector2.zero;

        mazo.RellenarMano();
    }

    public void DescartarSeleccionada()
    {
        if (seleccionada == null || mazo == null)
            return;

        CartaVisual carta = seleccionada;
        seleccionada = null;
        mazo.Descartar(carta);
    }
}