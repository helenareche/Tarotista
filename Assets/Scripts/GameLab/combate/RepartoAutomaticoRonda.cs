
using System.Collections;
using UnityEngine;

public class RepartoAutomaticoRonda : MonoBehaviour
{
    [SerializeField] private Mazo mazo;
    [SerializeField] private TableroUI tablero;

    // Se llama cuando termina de resolverse una ronda.
    public IEnumerator RepartirTrasRonda()
    {
        if (mazo == null || tablero == null)
        {
            Debug.LogError("Falta asignar Mazo o TableroUI.");
            yield break;
        }

        // Retirar las cartas jugadas del tablero.
        tablero.LimpiarSlots();

        // Esperar a que Unity destruya los objetos descartados.
        yield return null;

        // Completar la mano hasta tener 5 cartas.
        mazo.RellenarMano();

        Debug.Log("Reparto de la siguiente ronda completado.");
    }
}
