using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DuelManager : MonoBehaviour
{
    [SerializeField] private Vida aliento;// jugador
    [SerializeField] private Vida voluntad;// boss
    [SerializeField] private PistaBoss pistaBoss;
    [SerializeField] private DialogueUI dialogo;
    
    private readonly PosicionCombate[] posiciones =
    {
        new PosicionCombate(TipoPosicion.Pasado),
        new PosicionCombate(TipoPosicion.Presente),
        new PosicionCombate(TipoPosicion.Futuro)
    };

    private readonly List<Carta_Combate> jugadas = new List<Carta_Combate>();
    private readonly List<Carta_Combate> jugadasBoss = new List<Carta_Combate>();

    private bool jugadorConfirmo;

    private bool DueloTerminado => aliento.EstaMuerto || voluntad.EstaMuerto;

    public IEnumerator Duelo()
    {
        while (!DueloTerminado)
            yield return Ronda();

        bool gano = voluntad.EstaMuerto;
        // aquí: dar habilidad y curar si gana, pantalla de derrota si pierde
    }

    private IEnumerator Ronda()
    {
        // 1. Robar hasta 5 (pendiente: mano)
        yield return null;

        // 2. El boss coloca sus 3 cartas y revela una pista
        // (pendiente: rellenar jugadasBoss con la IA del boss, tarjeta I1-10)
        pistaBoss.RevelarCartaAleatoria(jugadasBoss);
        yield return dialogo.Mostrar("El boss te muestra una carta...");

        // 3. Descarte
        yield return EsperarConfirmacion();

        // 4. El jugador coloca 3 cartas boca abajo (rellena 'jugadas')
        yield return EsperarConfirmacion();

        // 5. Resolver
        yield return ResolverPosiciones();

        pistaBoss.OcultarPista();
        jugadas.Clear();
        jugadasBoss.Clear();
    }

    private IEnumerator ResolverPosiciones()
    {
        for (int i = 0; i < posiciones.Length; i++)
        {
            // aquí: animación de voltear las cartas de la posición i
            ResultadoPosicion resultado =
                posiciones[i].Resolver(jugadas[i], jugadasBoss[i]);

            voluntad.RecibirDanio(resultado.DanioAlBoss);
            aliento.RecibirDanio(resultado.DanioAlJugador);
            // Empate: ambos daños son 0, no pasa nada

            yield return new WaitForSeconds(1f);

            if (DueloTerminado) yield break;
        }
    }

    // Lo llama el botón "Confirmar" de la UI
    public void Confirmar() => jugadorConfirmo = true;

    private IEnumerator EsperarConfirmacion()
    {
        jugadorConfirmo = false;
        yield return new WaitUntil(() => jugadorConfirmo);
    }
}
