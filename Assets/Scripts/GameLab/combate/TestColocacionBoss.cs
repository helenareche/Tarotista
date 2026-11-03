
using UnityEngine;
using System.Collections.Generic;

public class TestColocacionBoss : MonoBehaviour
{
    [SerializeField] private TurnoAutomaticoBoss turnoBoss;

    private void Start()
    {
        if (turnoBoss == null)
        {
            Debug.LogError("Falta asignar TurnoAutomaticoBoss.");
            return;
        }

        // El boss prepara sus tres cartas y revela una pista.
        turnoBoss.PrepararRonda();

        // Obtener las cartas que ha elegido el boss.
        List<Carta_Combate> cartas = turnoBoss.ObtenerCartas();

        string[] posiciones = { "Pasado", "Presente", "Futuro" };

        // Comprobar que el boss ha generado tres cartas.
        if (cartas.Count != 3)
        {
            Debug.LogError("El boss debe tener exactamente 3 cartas.");
            return;
        }

        // Mostrar las cartas en la consola.
        for (int i = 0; i < cartas.Count; i++)
        {
            Debug.Log(posiciones[i] + ": " + cartas[i]);
        }

        Debug.Log("Prueba de preparación del boss completada.");

        // Limpiar las cartas al finalizar la ronda.
        turnoBoss.FinalizarRonda();
    }
}
