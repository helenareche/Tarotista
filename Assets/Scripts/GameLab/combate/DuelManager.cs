
using System.Collections;
using UnityEngine;

public class DuelManager : MonoBehaviour
{
    [Header("Sistemas del combate")]
    [SerializeField] private TableroUI tablero;
    [SerializeField] private RepartoBoss repartoBoss;
    [SerializeField] private RepartoAutomaticoRonda repartoAutomatico;

    [Header("Vida")]
    [SerializeField] private Vida aliento;
    [SerializeField] private Vida voluntad;

    [Header("Valores de prueba")]
    [SerializeField] private int alientoInicial = 10;
    [SerializeField] private int voluntadInicial = 12;

    [Header("Tiempo entre posiciones")]
    [SerializeField] private float pausaResolucion = 1f;

    private readonly PosicionCombate[] posiciones =
    {
        new PosicionCombate(TipoPosicion.Pasado),
        new PosicionCombate(TipoPosicion.Presente),
        new PosicionCombate(TipoPosicion.Futuro)
    };

    private bool combateTerminado;

    private bool DueloTerminado =>
        aliento.EstaMuerto || voluntad.EstaMuerto;

    private IEnumerator Start()
    {
        if (tablero == null ||
            repartoBoss == null ||
            repartoAutomatico == null ||
            aliento == null ||
            voluntad == null)
        {
            Debug.LogError(
                "DuelManager: faltan referencias en el Inspector."
            );
            yield break;
        }

        int vidaJugador = alientoInicial;
        int vidaBoss = voluntadInicial;

        GameManager partida = GameManager.Instance;

        if (partida != null)
        {
            vidaJugador = partida.aliento;

            if (partida.bossActual != null)
            {
                vidaBoss = partida.bossActual.voluntad;
            }
        }

        aliento.Inicializar(
            partida != null ? partida.alientoMaximo : alientoInicial,
            vidaJugador
        );

        voluntad.Inicializar(vidaBoss);

        // Esperar a que el mazo prepare la mano inicial.
        yield return null;

        yield return StartCoroutine(Duelo());
    }

    private IEnumerator Duelo()
    {
        while (!DueloTerminado && !combateTerminado)
        {
            yield return StartCoroutine(Ronda());
        }

        combateTerminado = true;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.aliento = aliento.Actual;
        }

        if (voluntad.EstaMuerto)
        {
            Debug.Log("Victoria: boss derrotado.");
        }
        else
        {
            Debug.Log("Derrota: el jugador se quedo sin Aliento.");
        }
    }

    private IEnumerator Ronda()
    {
        Debug.Log("Comienza una nueva ronda.");

        // 1. El boss prepara tres cartas.
        repartoBoss.PrepararRonda();

        Carta[] cartasBoss = repartoBoss.ObtenerCartas();

        if (cartasBoss.Length != 3)
        {
            Debug.LogError("El boss no tiene tres cartas.");
            combateTerminado = true;
            yield break;
        }

        // 2. Esperar a que el jugador coloque tres cartas.
        yield return new WaitUntil(
            () => tablero.TodasColocadas()
        );

        Carta[] cartasJugador = tablero.ObtenerJugadas();

        // 3. Revelar las cartas del boss.
        repartoBoss.RevelarTodas();

        // 4. Resolver Pasado, Presente y Futuro.
        for (int i = 0; i < 3; i++)
        {
            ResultadoPosicion resultado =
                posiciones[i].Resolver(
                    cartasJugador[i],
                    cartasBoss[i]
                );

            aliento.RecibirDanio(resultado.DanioAlJugador);
            voluntad.RecibirDanio(resultado.DanioAlBoss);

            Debug.Log(
                posiciones[i].Tipo +
                " - Ganador: " + resultado.Ganador +
                " - Danio jugador: " + resultado.DanioAlJugador +
                " - Danio boss: " + resultado.DanioAlBoss
            );

            yield return new WaitForSeconds(pausaResolucion);

            if (DueloTerminado)
            {
                break;
            }
        }

        // 5. Terminar la ronda del boss.
        repartoBoss.LimpiarRonda();

        if (!DueloTerminado)
        {
            // 6. Reponer la mano del jugador.
            yield return StartCoroutine(
                repartoAutomatico.RepartirTrasRonda()
            );
        }

        Debug.Log("Ronda terminada.");
    }
}
