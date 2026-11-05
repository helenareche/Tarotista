using UnityEngine;

public class TableroUI : MonoBehaviour
{
    public static TableroUI Instancia { get; private set; }

    [SerializeField] private Mazo mazo;
    [SerializeField] private SlotPosicion[] slots; // Pasado, Presente, Futuro, en ese orden

    private CartaVisual seleccionada;
    private bool descarteUsado;

    public bool DescarteUsado { get { return descarteUsado; } }

    private void Awake()
    {
        Instancia = this;
    }

    // Lo llama CartaSeleccionable al hacer clic en una carta
    public void Seleccionar(CartaVisual carta)
    {
        // Solo se pueden seleccionar cartas de la mano
        if (carta == null || mazo == null || carta.transform.parent != mazo.mano)
            return;

        // Clic en la ya seleccionada: se deselecciona
        if (seleccionada == carta)
        {
            Deseleccionar();
            return;
        }

        Deseleccionar();
        seleccionada = carta;

        CartaSeleccionable sel = carta.GetComponent<CartaSeleccionable>();
        if (sel != null) sel.SetElevada(true);
    }

    public void Deseleccionar()
    {
        if (seleccionada != null)
        {
            CartaSeleccionable sel = seleccionada.GetComponent<CartaSeleccionable>();
            if (sel != null) sel.SetElevada(false);
        }
        seleccionada = null;
    }

    public void DescartarSeleccionada()
    {
        // Solo se permite un descarte por ronda
        if (descarteUsado || seleccionada == null || mazo == null)
            return;

        // No permitir descartar si ya hay cartas en la mesa
        foreach (SlotPosicion slot in slots)
        {
            if (slot.Ocupado)
            {
                Debug.Log("No puedes descartar después de colocar cartas.");
                return;
            }
        }

        // Solo se puede descartar una carta de la mano
        if (seleccionada.transform.parent != mazo.mano)
            return;

        // Solo se descarta la carta seleccionada
        CartaVisual carta = seleccionada;
        seleccionada = null; // la carta se destruye, no hace falta bajarla
        descarteUsado = true;

        mazo.Descartar(carta);
    }

    // Devuelve una carta colocada a la mano
    public void DevolverCartaAMano(CartaArrastrable carta)
    {
        if (carta == null || mazo == null || mazo.mano == null)
            return;

        if (!carta.PuedeArrastrarse)
            return;

        SlotPosicion slot = carta.transform.parent.GetComponent<SlotPosicion>();
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
            jugadas[i] = slots[i].Ocupante != null ? slots[i].Ocupante.Carta : null;
        }

        return jugadas;
    }

    public void BloquearSlots(bool bloquear)
    {
        foreach (SlotPosicion slot in slots)
        {
            if (slot.Ocupado)
            {
                CartaArrastrable arr = slot.Ocupante.GetComponent<CartaArrastrable>();
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
}