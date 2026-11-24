
using UnityEngine;

public class TableroUI : MonoBehaviour
{
    public static TableroUI Instancia { get; private set; }

    [SerializeField] private Mazo mazo;
    [SerializeField] private SlotPosicion[] slots;

    private CartaVisual seleccionada;
    private bool descarteUsado;

    public bool DescarteUsado
    {
        get { return descarteUsado; }
    }

    public Transform Mano
    {
        get { return mazo != null ? mazo.mano : null; }
    }

    private void Awake()
    {
        Instancia = this;
    }

    public void Seleccionar(CartaVisual carta)
    {
        if (carta == null || mazo == null ||
            carta.transform.parent != mazo.mano)
            return;

        if (seleccionada == carta)
        {
            Deseleccionar();
            return;
        }

        Deseleccionar();
        seleccionada = carta;

        CartaSeleccionable sel =
            carta.GetComponent<CartaSeleccionable>();

        if (sel != null)
            sel.SetElevada(true);
    }

    public void Deseleccionar()
    {
        if (seleccionada != null)
        {
            CartaSeleccionable sel =
                seleccionada.GetComponent<CartaSeleccionable>();

            if (sel != null)
                sel.SetElevada(false);
        }

        seleccionada = null;
    }

    public void DescartarSeleccionada()
    {
        if (descarteUsado || seleccionada == null || mazo == null)
            return;

        foreach (SlotPosicion slot in slots)
        {
            if (slot.Ocupado)
            {
                Debug.Log("No puedes descartar después de colocar cartas.");
                return;
            }
        }

        if (seleccionada.transform.parent != mazo.mano)
            return;

        CartaVisual carta = seleccionada;
        seleccionada = null;
        descarteUsado = true;

        mazo.Descartar(carta);
    }

    public void DevolverCartaAMano(CartaArrastrable carta)
    {
        if (carta == null || mazo == null || mazo.mano == null)
            return;

        if (!carta.PuedeArrastrarse)
            return;

        SlotPosicion slot =
            carta.transform.parent.GetComponent<SlotPosicion>();

        if (slot == null)
            return;

        slot.Liberar();

        carta.transform.SetParent(mazo.mano, false);
        carta.transform.SetAsLastSibling();

        if (seleccionada == carta.GetComponent<CartaVisual>())
            Deseleccionar();
    }

    public bool TodasColocadas()
    {
        if (slots == null || slots.Length == 0)
            return false;

        foreach (SlotPosicion slot in slots)
        {
            if (!slot.Ocupado)
                return false;
        }

        return true;
    }

    public Carta[] ObtenerJugadas()
    {
        Carta[] jugadas = new Carta[slots.Length];

        for (int i = 0; i < slots.Length; i++)
        {
            jugadas[i] = slots[i].Ocupante != null
                ? slots[i].Ocupante.Carta
                : null;
        }

        return jugadas;
    }

    // Resuelve qué hacer con la carta de una posición.
    // Si destruir es false, se descartará normalmente al limpiar la ronda.
    public void ResolverCarta(int indice, bool destruir)
    {
        if (indice < 0 || indice >= slots.Length)
            return;

        SlotPosicion slot = slots[indice];

        if (!slot.Ocupado)
            return;

        CartaVisual carta = slot.Ocupante;

        if (destruir)
        {
            slot.Liberar();
            mazo.Destruir(carta);
        }
    }

    public void BloquearSlots(bool bloquear)
    {
        foreach (SlotPosicion slot in slots)
        {
            if (slot.Ocupado)
            {
                CartaArrastrable arr =
                    slot.Ocupante.GetComponent<CartaArrastrable>();

                if (arr != null)
                    arr.PuedeArrastrarse = !bloquear;
            }
        }
    }

    public void LimpiarSlots()
    {
        Deseleccionar();

        foreach (SlotPosicion slot in slots)
        {
            if (slot.Ocupado)
            {
                CartaVisual carta = slot.Ocupante;

                slot.Liberar();
                mazo.DescartarSinRellenar(carta);
            }
        }

        descarteUsado = false;
        mazo.RellenarMano();
    }

    //Elevar la carta que está colocada en cada posición
    public CartaVisual ObtenerVisualCarta(int indice)
    {
        if (slots == null || indice < 0 || indice >= slots.Length)
            return null;

        if (slots[indice] == null || !slots[indice].Ocupado)
            return null;

        return slots[indice].Ocupante;
    }
}
