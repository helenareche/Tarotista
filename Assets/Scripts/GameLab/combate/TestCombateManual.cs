using System.Collections.Generic;
using UnityEngine;

public class TestCombateManual : MonoBehaviour
{
    [SerializeField] private PistaBoss pistaBoss;

    private List<Carta_Combate> cartasBoss;

    private void Awake()
    {
        if (pistaBoss == null)
        {
            pistaBoss = GetComponent<PistaBoss>();
        }
    }

    private void Start()
    {
        Debug.Log(
            "=== COMIENZA LA PRUEBA DE COMBATE ==="
        );

        cartasBoss = new List<Carta_Combate>
        {
            new Carta_Combate(Palos.Velo, 7),
            new Carta_Combate(Palos.Raiz, 8),
            new Carta_Combate(Palos.Llama, 9)
        };

        ProbarResolucion();
        ProbarPista();

        Debug.Log(
            "=== FINALIZA LA PRUEBA DE COMBATE ==="
        );
    }

    private void ProbarResolucion()
    {
        Carta_Combate cartaJugador =
            new Carta_Combate(Palos.Llama, 4);

        Carta_Combate cartaBoss =
            cartasBoss[0];

        ResultadoPosicion resultado =
            ResolutorPosicion.Resolver(
                cartaJugador,
                cartaBoss,
                1
            );

        Debug.Log(
            "RESULTADO DE LA POSICIÓN\n" +
            "Carta del jugador: " +
            cartaJugador + "\n" +
            "Carta del boss: " +
            cartaBoss + "\n" +
            "Ganador: " +
            resultado.Ganador + "\n" +
            "Poder final del jugador: " +
            resultado.PoderJugador + "\n" +
            "Poder final del boss: " +
            resultado.PoderBoss + "\n" +
            "Daño al boss: " +
            resultado.DanioAlBoss + "\n" +
            "Daño al jugador: " +
            resultado.DanioAlJugador
        );
    }

    private void ProbarPista()
    {
        if (pistaBoss == null)
        {
            Debug.LogError(
                "El objeto de prueba no tiene " +
                "un componente PistaBoss."
            );

            return;
        }

        Carta_Combate cartaRevelada =
            pistaBoss.RevelarCartaAleatoria(
                cartasBoss
            );

        Debug.Log(
            "PISTA DEL BOSS\n" +
            "Índice revelado: " +
            pistaBoss.IndiceRevelado + "\n" +
            "Carta revelada: " +
            cartaRevelada
        );
    }
}