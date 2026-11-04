
using System.Collections;
using UnityEngine;

public class TestRepartoAutomatico : MonoBehaviour
{
    [SerializeField] private RepartoAutomaticoRonda reparto;
    [SerializeField] private Mazo mazo;
    [SerializeField] private TableroUI tablero;

    [SerializeField] private SlotPosicion slotPasado;
    [SerializeField] private SlotPosicion slotPresente;
    [SerializeField] private SlotPosicion slotFuturo;

    private IEnumerator Start()
    {
        if (reparto == null || mazo == null || tablero == null ||
            slotPasado == null || slotPresente == null ||
            slotFuturo == null)
        {
            Debug.LogError("Faltan referencias para la prueba.");
            yield break;
        }

        Debug.Log("Coloca tres cartas en Pasado, Presente y Futuro.");

        yield return new WaitUntil(() => tablero.TodasColocadas());

        yield return null;

        // Guardar las dos cartas que no se han jugado.
        CartaVisual[] conservadas =
            mazo.mano.GetComponentsInChildren<CartaVisual>();

        if (conservadas.Length != 2)
        {
            Debug.LogError("PRUEBA FALLIDA: no quedan dos cartas.");
            yield break;
        }

        Debug.Log("Dos cartas conservadas correctamente.");

        // Simular el final de la ronda.
        yield return StartCoroutine(reparto.RepartirTrasRonda());

        // Esperar a que Unity procese las destrucciones.
        yield return null;

        CartaVisual[] manoFinal =
            mazo.mano.GetComponentsInChildren<CartaVisual>();

        bool seConservaron =
            System.Array.IndexOf(manoFinal, conservadas[0]) >= 0 &&
            System.Array.IndexOf(manoFinal, conservadas[1]) >= 0;

        // Comprobar el estado de los tres slots.
        bool slotsLiberados =
            !slotPasado.Ocupado &&
            !slotPresente.Ocupado &&
            !slotFuturo.Ocupado;

        // Comprobar que no quedan cartas visuales.
        bool slotsVisualmenteVacios =
            slotPasado.GetComponentsInChildren<CartaVisual>(true).Length == 0 &&
            slotPresente.GetComponentsInChildren<CartaVisual>(true).Length == 0 &&
            slotFuturo.GetComponentsInChildren<CartaVisual>(true).Length == 0;

        if (manoFinal.Length == 5 &&
            seConservaron &&
            slotsLiberados &&
            slotsVisualmenteVacios)
        {
            Debug.Log("PRUEBA SUPERADA: cinco cartas en la mano.");
            Debug.Log("Las dos cartas anteriores se conservaron.");
            Debug.Log("Las tres posiciones quedaron vacias.");
        }
        else
        {
            Debug.LogError("PRUEBA FALLIDA: revisar el reparto.");

            Debug.Log("Cartas en mano: " + manoFinal.Length);
            Debug.Log("Conservadas: " + seConservaron);
            Debug.Log("Slots liberados: " + slotsLiberados);
            Debug.Log("Slots visualmente vacios: " +
                slotsVisualmenteVacios);
        }
    }
}
