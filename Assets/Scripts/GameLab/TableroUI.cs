using UnityEngine;

public class TableroUI : MonoBehaviour
{
    public static TableroUI Instancia { get; private set; }

    [SerializeField] private Mazo mazo;
    [SerializeField] private SlotPosicion[] slots; // Pasado, Presente, Futuro, en ese orden

    private CartaVisual seleccionada;

    private void Awake()
    {
        Instancia = this;
    }

    // Lo llama CartaVisual al hacer clic en una carta (para el botón Descartar)
    public void Seleccionar(CartaVisual carta)
    {
        seleccionada = carta;
    }

    public void DescartarSeleccionada()
    {
        if (seleccionada == null || mazo == null)
            return;

        CartaVisual carta = seleccionada;
        seleccionada = null;
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

    // Al terminar la resolución: descarta lo jugado y vuelve a robar hasta 5
    public void LimpiarSlots()
    {
        foreach (SlotPosicion slot in slots)
        {
            if (slot.Ocupado)
            {
                CartaVisual carta = slot.Ocupante;
                slot.Liberar();
                mazo.Descartar(carta);
            }
        }
    }
}