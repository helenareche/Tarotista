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

    // Lo llama CartaSeleccionable al hacer clic en una carta (para el botón Descartar)
    public void Seleccionar(CartaVisual carta)
    {
        seleccionada = carta;
    }

    public void DescartarSeleccionada()
    {
        // Solo un descarte por ronda
        if (descarteUsado || seleccionada == null || mazo == null)
            return;

        // No se puede descartar una carta ya colocada en un slot
        if (seleccionada.transform.parent != mazo.mano)
            return;

        CartaVisual carta = seleccionada;
        seleccionada = null;
        descarteUsado = true;
        mazo.Descartar(carta);
    }

    // True cuando los tres huecos tienen carta
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

    // Cartas jugadas, en orden Pasado, Presente, Futuro
    public Carta[] ObtenerJugadas()
    {
        Carta[] jugadas = new Carta[slots.Length];
        for (int i = 0; i < slots.Length; i++)
        {
            jugadas[i] = slots[i].Ocupante != null ? slots[i].Ocupante.Carta : null;
        }
        return jugadas;
    }

    // Bloquea o desbloquea el arrastre de las cartas colocadas
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

    // Al terminar la resolución: descarta lo jugado y vuelve a robar hasta 5
    public void LimpiarSlots()
    {
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
        seleccionada = null;
        mazo.RellenarMano();
    }
}