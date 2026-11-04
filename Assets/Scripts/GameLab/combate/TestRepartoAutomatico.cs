
using System.Collections;
using UnityEngine;

public class TestRepartoAutomatico : MonoBehaviour
{
    [SerializeField] private RepartoAutomaticoRonda reparto;
    [SerializeField] private Mazo mazo;
    [SerializeField] private TableroUI tablero;

    private IEnumerator Start()
    {
        if (reparto == null || mazo == null || tablero == null)
        {
            Debug.LogError("Faltan referencias para la prueba.");
            yield break;
        }

        // Esperar hasta que el jugador coloque tres cartas.
        Debug.Log("Coloca tres cartas en Pasado, Presente y Futuro.");

        yield return new WaitUntil(() => tablero.TodasColocadas());

        // Dejar que termine el movimiento de las cartas.
        yield return null;

        CartaVisual[] conservadas =
            mazo.mano.GetComponentsInChildren<CartaVisual>();

        if (conservadas.Length != 2)
        {
            Debug.LogError("Deben quedar dos cartas en la mano.");
            yield break;
        }

        Debug.Log("Dos cartas conservadas correctamente.");

        // Simular que termina la resolución de la ronda.
        yield return StartCoroutine(reparto.RepartirTrasRonda());

        // Esperar a que se completen las operaciones de Unity.
        yield return null;

        CartaVisual[] manoFinal =
            mazo.mano.GetComponentsInChildren<CartaVisual>();

        bool seConservaron =
            System.Array.IndexOf(manoFinal, conservadas[0]) >= 0 &&
            System.Array.IndexOf(manoFinal, conservadas[1]) >= 0;

        if (manoFinal.Length == 5 && seConservaron &&
            !tablero.TodasColocadas())
        {
            Debug.Log("PRUEBA SUPERADA: cinco cartas en la mano.");
            Debug.Log("Las dos cartas anteriores se conservaron.");
            Debug.Log("Se han repartido tres cartas nuevas.");
        }
        else
        {
            Debug.LogError("PRUEBA FALLIDA: revisar el reparto.");
        }
    }
}
