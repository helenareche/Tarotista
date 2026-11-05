using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CartaVisual : MonoBehaviour
{
    public TMP_Text texto;
    public Image fondo;

    public Carta Carta
    {
        get;
        private set;
    }

    public bool BocaArriba
    {
        get;
        private set;
    }

    private void Awake()
    {
        // Solo el fondo recibe clics y arrastres.
        // Si el texto es más ancho que la carta, intercepta los clics de la carta vecina.
        if (texto != null)
        {
            texto.raycastTarget = false;
        }

        if (fondo != null)
        {
            fondo.raycastTarget = true;
        }
    }

    public void Configurar(Carta carta, bool bocaArriba)
    {
        Carta = carta;
        BocaArriba = bocaArriba;
        Actualizar();
    }

    public void MostrarCara()
    {
        BocaArriba = true;
        Actualizar();
    }

    public void MostrarReverso()
    {
        BocaArriba = false;
        Actualizar();
    }

    public void Voltear()
    {
        BocaArriba = !BocaArriba;
        Actualizar();
    }

    private void Actualizar()
    {
        if (Carta == null)
        {
            return;
        }

        if (BocaArriba)
        {
            texto.text =
                Carta.NombreValor + "\n" +
                Carta.Palo;

            fondo.color =
                ColorDelPalo(Carta.Palo);
        }
        else
        {
            texto.text = "?";
            fondo.color = Color.gray;
        }
    }

    private Color ColorDelPalo(Palo palo)
    {
        switch (palo)
        {
            case Palo.Llama:
                return new Color(1f, 0.5f, 0.3f);

            case Palo.Velo:
                return new Color(0.6f, 0.7f, 1f);

            case Palo.Raiz:
                return new Color(0.5f, 0.8f, 0.4f);

            default:
                return new Color(0.7f, 0.5f, 0.8f);
        }
    }
}