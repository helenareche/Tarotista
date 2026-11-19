using System;
using UnityEngine;

public class TestCombate : MonoBehaviour
{
    private int correctos;
    private int fallidos;

    private void Start()
    {
        Debug.Log("=== INICIO TEST_COMBATE ===");

        TestVictoriaJugadorPorValor();
        TestVictoriaBossPorValor();
        TestEmpate();
        TestVictoriaJugadorPorAfinidad();
        TestVictoriaBossPorAfinidad();
        TestDanioAlBoss();
        TestDanioAlJugador();
        TestEmpateSinDanio();
        TestCartaJugadorNula();
        TestCartaBossNula();
        TestDanioCeroInvalido();
        TestDanioNegativoInvalido();
        TestDanioSegunPosicion();
        TestResolverPosicionPasado();
        TestResolverPosicionPresente();
        TestResolverPosicionFuturo();
        TestAplicarDanioAlBoss();
        TestAplicarDanioAlJugador();
        TestAhorcadoGanaJugadorConMenosPoder();
        TestAhorcadoGanaBossConMenosPoder();
        TestAhorcadoAfinidadPerjudica();
        TestAhorcadoEmpateSigueEmpate();
        TestAhorcadoPosicionHaceSuDanio();
        TestLocoCicloNormalNoCambia();
        TestLocoCicloInvertido();
        TestLocoCicloInvertidoCompleto();
        TestLocoMismoPaloSinVentaja();
        TestLocoCambiaGanadorDePosicion();

        MostrarResultado();
    }

    private void TestVictoriaJugadorPorValor()
    {
        Carta jugador = new Carta(Palo.Llama, 10);
        Carta boss = new Carta(Palo.Llama, 5);

        ResultadoPosicion resultado =
            ResolutorPosicion.Resolver(jugador, boss, 2);

        Comprobar(
            resultado.Ganador == GanadorPosicion.Jugador &&
            resultado.PoderJugador == 10 &&
            resultado.PoderBoss == 5,
            "El jugador gana cuando tiene mayor poder"
        );
    }

    private void TestVictoriaBossPorValor()
    {
        Carta jugador = new Carta(Palo.Velo, 4);
        Carta boss = new Carta(Palo.Velo, 12);

        ResultadoPosicion resultado =
            ResolutorPosicion.Resolver(jugador, boss, 2);

        Comprobar(
            resultado.Ganador == GanadorPosicion.Boss &&
            resultado.PoderJugador == 4 &&
            resultado.PoderBoss == 12,
            "El boss gana cuando tiene mayor poder"
        );
    }

    private void TestEmpate()
    {
        Carta jugador = new Carta(Palo.Raiz, 8);
        Carta boss = new Carta(Palo.Raiz, 8);

        ResultadoPosicion resultado =
            ResolutorPosicion.Resolver(jugador, boss, 3);

        Comprobar(
            resultado.Ganador == GanadorPosicion.Empate &&
            resultado.PoderJugador == 8 &&
            resultado.PoderBoss == 8,
            "Dos cartas con el mismo poder empatan"
        );
    }

    private void TestVictoriaJugadorPorAfinidad()
    {
        // Llama tiene ventaja sobre Velo.
        // Poder jugador: 7 + 4 = 11.
        // Poder boss: 10.
        Carta jugador = new Carta(Palo.Llama, 7);
        Carta boss = new Carta(Palo.Velo, 10);

        ResultadoPosicion resultado =
            ResolutorPosicion.Resolver(jugador, boss, 2);

        Comprobar(
            resultado.Ganador == GanadorPosicion.Jugador &&
            resultado.PoderJugador == 11 &&
            resultado.PoderBoss == 10,
            "La afinidad permite al jugador ganar con menor valor"
        );
    }

    private void TestVictoriaBossPorAfinidad()
    {
        // Abismo tiene ventaja sobre Llama.
        // Poder jugador: 10.
        // Poder boss: 7 + 4 = 11.
        Carta jugador = new Carta(Palo.Llama, 10);
        Carta boss = new Carta(Palo.Abismo, 7);

        ResultadoPosicion resultado =
            ResolutorPosicion.Resolver(jugador, boss, 2);

        Comprobar(
            resultado.Ganador == GanadorPosicion.Boss &&
            resultado.PoderJugador == 10 &&
            resultado.PoderBoss == 11,
            "La afinidad permite al boss ganar con menor valor"
        );
    }

    private void TestDanioAlBoss()
    {
        Carta jugador = new Carta(Palo.Llama, 12);
        Carta boss = new Carta(Palo.Llama, 5);

        ResultadoPosicion resultado =
            ResolutorPosicion.Resolver(jugador, boss, 3);

        Comprobar(
            resultado.DanioAlBoss == 3 &&
            resultado.DanioAlJugador == 0,
            "Si gana el jugador, el daño se asigna solamente al boss"
        );
    }

    private void TestDanioAlJugador()
    {
        Carta jugador = new Carta(Palo.Velo, 2);
        Carta boss = new Carta(Palo.Velo, 9);

        ResultadoPosicion resultado =
            ResolutorPosicion.Resolver(jugador, boss, 2);

        Comprobar(
            resultado.DanioAlJugador == 2 &&
            resultado.DanioAlBoss == 0,
            "Si gana el boss, el daño se asigna solamente al jugador"
        );
    }

    private void TestEmpateSinDanio()
    {
        Carta jugador = new Carta(Palo.Abismo, 9);
        Carta boss = new Carta(Palo.Abismo, 9);

        ResultadoPosicion resultado =
            ResolutorPosicion.Resolver(jugador, boss, 3);

        Comprobar(
            resultado.DanioAlJugador == 0 &&
            resultado.DanioAlBoss == 0,
            "Un empate no causa daño"
        );
    }

    private void TestCartaJugadorNula()
    {
        Carta boss = new Carta(Palo.Llama, 5);
        bool lanzoExcepcion = false;

        try
        {
            ResolutorPosicion.Resolver(null, boss, 1);
        }
        catch (ArgumentNullException)
        {
            lanzoExcepcion = true;
        }

        Comprobar(
            lanzoExcepcion,
            "Resolver rechaza una carta del jugador nula"
        );
    }

    private void TestCartaBossNula()
    {
        Carta jugador = new Carta(Palo.Llama, 5);
        bool lanzoExcepcion = false;

        try
        {
            ResolutorPosicion.Resolver(jugador, null, 1);
        }
        catch (ArgumentNullException)
        {
            lanzoExcepcion = true;
        }

        Comprobar(
            lanzoExcepcion,
            "Resolver rechaza una carta del boss nula"
        );
    }

    private void TestDanioCeroInvalido()
    {
        Carta jugador = new Carta(Palo.Llama, 5);
        Carta boss = new Carta(Palo.Velo, 5);
        bool lanzoExcepcion = false;

        try
        {
            ResolutorPosicion.Resolver(jugador, boss, 0);
        }
        catch (ArgumentOutOfRangeException)
        {
            lanzoExcepcion = true;
        }

        Comprobar(
            lanzoExcepcion,
            "Resolver rechaza un daño igual a cero"
        );
    }

    private void TestDanioNegativoInvalido()
    {
        Carta jugador = new Carta(Palo.Llama, 5);
        Carta boss = new Carta(Palo.Velo, 5);
        bool lanzoExcepcion = false;

        try
        {
            ResolutorPosicion.Resolver(jugador, boss, -2);
        }
        catch (ArgumentOutOfRangeException)
        {
            lanzoExcepcion = true;
        }

        Comprobar(
            lanzoExcepcion,
            "Resolver rechaza un daño negativo"
        );
    }

    private void TestDanioSegunPosicion()
    {
        PosicionCombate pasado =
            new PosicionCombate(TipoPosicion.Pasado);

        PosicionCombate presente =
            new PosicionCombate(TipoPosicion.Presente);

        PosicionCombate futuro =
            new PosicionCombate(TipoPosicion.Futuro);

        Comprobar(
            pasado.Danio == 1 &&
            presente.Danio == 2 &&
            futuro.Danio == 3,
            "Las posiciones causan 1, 2 y 3 puntos de daño"
        );
    }

    private void TestResolverPosicionPasado()
    {
        PosicionCombate posicion =
            new PosicionCombate(TipoPosicion.Pasado);

        Carta jugador = new Carta(Palo.Llama, 10);
        Carta boss = new Carta(Palo.Llama, 4);

        ResultadoPosicion resultado =
            posicion.Resolver(jugador, boss);

        Comprobar(
            posicion.Tipo == TipoPosicion.Pasado &&
            resultado.Ganador == GanadorPosicion.Jugador &&
            resultado.DanioAlBoss == 1,
            "Pasado aplica 1 punto de daño"
        );
    }

    private void TestResolverPosicionPresente()
    {
        PosicionCombate posicion =
            new PosicionCombate(TipoPosicion.Presente);

        Carta jugador = new Carta(Palo.Raiz, 3);
        Carta boss = new Carta(Palo.Raiz, 10);

        ResultadoPosicion resultado =
            posicion.Resolver(jugador, boss);

        Comprobar(
            posicion.Tipo == TipoPosicion.Presente &&
            resultado.Ganador == GanadorPosicion.Boss &&
            resultado.DanioAlJugador == 2,
            "Presente aplica 2 puntos de daño"
        );
    }

    private void TestResolverPosicionFuturo()
    {
        PosicionCombate posicion =
            new PosicionCombate(TipoPosicion.Futuro);

        Carta jugador = new Carta(Palo.Abismo, 14);
        Carta boss = new Carta(Palo.Abismo, 1);

        ResultadoPosicion resultado =
            posicion.Resolver(jugador, boss);

        Comprobar(
            posicion.Tipo == TipoPosicion.Futuro &&
            resultado.Ganador == GanadorPosicion.Jugador &&
            resultado.DanioAlBoss == 3,
            "Futuro aplica 3 puntos de daño"
        );
    }

    private void TestAplicarDanioAlBoss()
    {
        GameObject objetoBoss =
            new GameObject("VidaBoss_Prueba");

        Vida vidaBoss =
            objetoBoss.AddComponent<Vida>();

        vidaBoss.Inicializar(10);

        Carta jugador =
            new Carta(Palo.Llama, 12);

        Carta boss =
            new Carta(Palo.Llama, 5);

        ResultadoPosicion resultado =
            ResolutorPosicion.Resolver(
                jugador,
                boss,
                3
            );

        vidaBoss.RecibirDanio(
            resultado.DanioAlBoss
        );

        Comprobar(
            vidaBoss.Actual == 7,
            "El daño del resultado se puede aplicar a la vida del boss"
        );

        Destroy(objetoBoss);
    }

    private void TestAplicarDanioAlJugador()
    {
        GameObject objetoJugador =
            new GameObject("VidaJugador_Prueba");

        Vida vidaJugador =
            objetoJugador.AddComponent<Vida>();

        vidaJugador.Inicializar(10);

        Carta jugador =
            new Carta(Palo.Velo, 2);

        Carta boss =
            new Carta(Palo.Velo, 10);

        ResultadoPosicion resultado =
            ResolutorPosicion.Resolver(
                jugador,
                boss,
                2
            );

        vidaJugador.RecibirDanio(
            resultado.DanioAlJugador
        );

        Comprobar(
            vidaJugador.Actual == 8,
            "El daño del resultado se puede aplicar a la vida del jugador"
        );

        Destroy(objetoJugador);
    }

    // ---------- El Ahorcado (036): gana la carta con MENOS Poder ----------

    private void TestAhorcadoGanaJugadorConMenosPoder()
    {
        Carta jugador = new Carta(Palo.Llama, 3);
        Carta boss = new Carta(Palo.Llama, 9);

        ResultadoPosicion resultado =
            ResolutorPosicion.Resolver(jugador, boss, 2, ReglaBoss.InvierteSigno);

        Comprobar(
            resultado.Ganador == GanadorPosicion.Jugador &&
            resultado.DanioAlBoss == 2 &&
            resultado.DanioAlJugador == 0,
            "El Ahorcado: el jugador gana con menos Poder"
        );
    }

    private void TestAhorcadoGanaBossConMenosPoder()
    {
        Carta jugador = new Carta(Palo.Velo, 12);
        Carta boss = new Carta(Palo.Velo, 2);

        ResultadoPosicion resultado =
            ResolutorPosicion.Resolver(jugador, boss, 3, ReglaBoss.InvierteSigno);

        Comprobar(
            resultado.Ganador == GanadorPosicion.Boss &&
            resultado.DanioAlJugador == 3 &&
            resultado.DanioAlBoss == 0,
            "El Ahorcado: el boss gana con menos Poder"
        );
    }

    private void TestAhorcadoAfinidadPerjudica()
    {
        // Llama > Velo: el jugador suma +4 (5 + 4 = 9) y el boss se queda en 7.
        // Con El Ahorcado gana el menor, así que gana el boss.
        Carta jugador = new Carta(Palo.Llama, 5);
        Carta boss = new Carta(Palo.Velo, 7);

        ResultadoPosicion resultado =
            ResolutorPosicion.Resolver(jugador, boss, 1, ReglaBoss.InvierteSigno);

        Comprobar(
            resultado.Ganador == GanadorPosicion.Boss &&
            resultado.PoderJugador == 9 &&
            resultado.PoderBoss == 7,
            "El Ahorcado: la Afinidad suma +4 y eso perjudica"
        );
    }

    private void TestAhorcadoEmpateSigueEmpate()
    {
        Carta jugador = new Carta(Palo.Raiz, 6);
        Carta boss = new Carta(Palo.Raiz, 6);

        ResultadoPosicion resultado =
            ResolutorPosicion.Resolver(jugador, boss, 2, ReglaBoss.InvierteSigno);

        Comprobar(
            resultado.Ganador == GanadorPosicion.Empate &&
            resultado.DanioAlJugador == 0 &&
            resultado.DanioAlBoss == 0,
            "El Ahorcado: los empates siguen siendo empate"
        );
    }

    private void TestAhorcadoPosicionHaceSuDanio()
    {
        PosicionCombate futuro = new PosicionCombate(TipoPosicion.Futuro);
        Carta jugador = new Carta(Palo.Abismo, 1);
        Carta boss = new Carta(Palo.Abismo, 14);

        ResultadoPosicion resultado =
            futuro.Resolver(jugador, boss, ReglaBoss.InvierteSigno);

        Comprobar(
            resultado.Ganador == GanadorPosicion.Jugador &&
            resultado.DanioAlBoss == 3,
            "El Ahorcado: ganar el Futuro con menos Poder hace 3 de daño"
        );
    }

    // ---------- El Loco (041): el ciclo de Afinidades se invierte ----------

    private void TestLocoCicloNormalNoCambia()
    {
        Carta llama = new Carta(Palo.Llama, 5);
        Carta velo = new Carta(Palo.Velo, 5);

        Comprobar(
            llama.TieneVentajaSobre(velo, false) &&
            !velo.TieneVentajaSobre(llama, false),
            "El Loco: sin invertir, Llama sigue ganando a Velo"
        );
    }

    private void TestLocoCicloInvertido()
    {
        Carta llama = new Carta(Palo.Llama, 5);
        Carta velo = new Carta(Palo.Velo, 5);

        Comprobar(
            velo.TieneVentajaSobre(llama, true) &&
            !llama.TieneVentajaSobre(velo, true),
            "El Loco: con el ciclo invertido, Velo gana a Llama"
        );
    }

    private void TestLocoCicloInvertidoCompleto()
    {
        // Invertido: Velo > Llama, Raíz > Velo, Abismo > Raíz, Llama > Abismo
        bool correcto =
            new Carta(Palo.Velo, 1).TieneVentajaSobre(new Carta(Palo.Llama, 1), true) &&
            new Carta(Palo.Raiz, 1).TieneVentajaSobre(new Carta(Palo.Velo, 1), true) &&
            new Carta(Palo.Abismo, 1).TieneVentajaSobre(new Carta(Palo.Raiz, 1), true) &&
            new Carta(Palo.Llama, 1).TieneVentajaSobre(new Carta(Palo.Abismo, 1), true);

        Comprobar(correcto, "El Loco: el ciclo invertido funciona con los cuatro palos");
    }

    private void TestLocoMismoPaloSinVentaja()
    {
        Carta a = new Carta(Palo.Raiz, 4);
        Carta b = new Carta(Palo.Raiz, 9);

        Comprobar(
            !a.TieneVentajaSobre(b, true) &&
            a.PoderContra(b, true) == 4,
            "El Loco: con el mismo palo no hay ventaja aunque se invierta"
        );
    }

    private void TestLocoCambiaGanadorDePosicion()
    {
        // Normal: Llama 7 + 4 = 11 contra Velo 5 -> gana el boss.
        // Invertido: Velo 5 + 4 = 9 contra Llama 7 -> gana el jugador.
        Carta jugador = new Carta(Palo.Velo, 5);
        Carta boss = new Carta(Palo.Llama, 7);

        ResultadoPosicion normal =
            ResolutorPosicion.Resolver(jugador, boss, 2, ReglaBoss.InvierteCiclo, false);
        ResultadoPosicion invertido =
            ResolutorPosicion.Resolver(jugador, boss, 2, ReglaBoss.InvierteCiclo, true);

        Comprobar(
            normal.Ganador == GanadorPosicion.Boss &&
            invertido.Ganador == GanadorPosicion.Jugador &&
            invertido.PoderJugador == 9 &&
            invertido.PoderBoss == 7 &&
            invertido.DanioAlBoss == 2,
            "El Loco: invertir el ciclo cambia quién gana la posición"
        );
    }

    private void Comprobar(bool condicion, string nombre)
    {
        if (condicion)
        {
            correctos++;
            Debug.Log("[OK] " + nombre);
        }
        else
        {
            fallidos++;
            Debug.LogError("[FALLO] " + nombre);
        }
    }

    private void MostrarResultado()
    {
        string resultado =
            "=== RESULTADO TEST_COMBATE: " +
            correctos + " correctos, " +
            fallidos + " fallidos ===";

        if (fallidos == 0)
        {
            Debug.Log(resultado);
        }
        else
        {
            Debug.LogError(resultado);
        }
    }
}