using System.Collections.Generic;
using UnityEngine;

public class TestTurnnos : MonoBehaviour
{
    private int correctos;
    private int fallidos;

    private GameObject objetoGameManager;
    private GameManager gameManager;

    private GameObject objetoPista;
    private PistaBoss pistaBoss;

    private readonly List<BossData> bossesTemporales =
        new List<BossData>();

    private void Start()
    {
        Debug.Log("=== INICIO TEST_TURNOS ===");

        PrepararObjetos();

        if (gameManager == null || pistaBoss == null)
        {
            Debug.LogError(
                "No se pudieron preparar los objetos de prueba."
            );

            return;
        }

        TestNuevaPartidaReiniciaJugador();
        TestNuevaPartidaReiniciaEstado();
        TestNuevaPartidaEligeTresPruebas();
        TestPruebasNoRepetidas();
        TestNuevaPartidaEligeBossFinal();

        TestSaltarPrueba();
        TestSaltarPruebaRecuperaAliento();
        TestSaltarPruebaNoSuperaMaximo();

        TestGanarPrueba();
        TestGanarPruebaRecuperaAliento();
        TestGanarPruebaNoSuperaMaximo();
        TestGanarPruebaAnadeHabilidad();

        TestNoTodasResueltas();
        TestTodasResueltas();
        TestProgresoCombinado();

        TestRevelarCartaAleatoria();
        TestIndiceReveladoValido();
        TestCartaReveladaCoincideConIndice();
        TestEventoAlRevelarPista();
        TestOcultarPista();
        TestListaNula();
        TestNumeroDeCartasIncorrecto();

        MostrarResultado();
        Limpiar();
    }

    private void PrepararObjetos()
    {
        PrepararGameManager();
        PrepararPistaBoss();
    }

    private void PrepararGameManager()
    {
        if (GameManager.Instance != null)
        {
            Debug.LogError(
                "TestTurnos: ya existe un GameManager activo. " +
                "Elimínalo o desactívalo en la escena Test_Turnos."
            );

            return;
        }

        objetoGameManager =
            new GameObject("GameManager_Prueba");

        gameManager =
            objetoGameManager.AddComponent<GameManager>();

        gameManager.bossesPrueba =
            new List<BossData>();

        gameManager.bossesFinales =
            new List<BossData>();

        gameManager.bossesPrueba.Add(
            CrearBoss(
                "Prueba Llama",
                false,
                ReglaBoss.Ninguna
            )
        );

        gameManager.bossesPrueba.Add(
            CrearBoss(
                "Prueba Velo",
                false,
                ReglaBoss.GanaEmpates
            )
        );

        gameManager.bossesPrueba.Add(
            CrearBoss(
                "Prueba Raiz",
                false,
                ReglaBoss.InvierteSigno
            )
        );

        gameManager.bossesPrueba.Add(
            CrearBoss(
                "Prueba Abismo",
                false,
                ReglaBoss.OcultaPista
            )
        );

        gameManager.bossesFinales.Add(
            CrearBoss(
                "Boss Final Uno",
                true,
                ReglaBoss.PoderExtra
            )
        );

        gameManager.bossesFinales.Add(
            CrearBoss(
                "Boss Final Dos",
                true,
                ReglaBoss.GanaEmpates
            )
        );
    }

    private void PrepararPistaBoss()
    {
        objetoPista =
            new GameObject("PistaBoss_Prueba");

        pistaBoss =
            objetoPista.AddComponent<PistaBoss>();
    }

    private BossData CrearBoss(
        string nombre,
        bool esFinal,
        ReglaBoss regla)
    {
        BossData boss =
            ScriptableObject.CreateInstance<BossData>();

        boss.nombre = nombre;
        boss.esFinal = esFinal;
        boss.regla = regla;
        boss.voluntad = 12;

        bossesTemporales.Add(boss);

        return boss;
    }

    private void PrepararNuevaPartida()
    {
        gameManager.NuevaPartida();
    }

    private List<Carta> CrearTresCartasBoss()
    {
        return new List<Carta>
        {
            new Carta(Palo.Llama, 3),
            new Carta(Palo.Velo, 7),
            new Carta(Palo.Raiz, 12)
        };
    }

    private void TestNuevaPartidaReiniciaJugador()
    {
        gameManager.alientoMaximo = 30;
        gameManager.aliento = 2;
        gameManager.habilidades.Add(
            "Habilidad antigua"
        );

        gameManager.NuevaPartida();

        Comprobar(
            gameManager.alientoMaximo == 10 &&
            gameManager.aliento == 10 &&
            gameManager.habilidades.Count == 0,
            "NuevaPartida reinicia el aliento y las habilidades"
        );
    }

    private void TestNuevaPartidaReiniciaEstado()
    {
        gameManager.indiceActual = 2;
        gameManager.victoriaFinal = true;
        gameManager.bossActual =
            gameManager.bossesPrueba[0];

        gameManager.NuevaPartida();

        bool pruebasSinResolver = true;
        bool pruebasSinSaltar = true;

        for (int i = 0; i < 3; i++)
        {
            if (gameManager.pruebaResuelta[i])
            {
                pruebasSinResolver = false;
            }

            if (gameManager.pruebaSaltada[i])
            {
                pruebasSinSaltar = false;
            }
        }

        Comprobar(
            gameManager.indiceActual == -1 &&
            gameManager.bossActual == null &&
            !gameManager.victoriaFinal &&
            pruebasSinResolver &&
            pruebasSinSaltar,
            "NuevaPartida reinicia el estado de la partida"
        );
    }

    private void TestNuevaPartidaEligeTresPruebas()
    {
        PrepararNuevaPartida();

        bool hayTres =
            gameManager.pruebas != null &&
            gameManager.pruebas.Length == 3 &&
            gameManager.pruebas[0] != null &&
            gameManager.pruebas[1] != null &&
            gameManager.pruebas[2] != null;

        Comprobar(
            hayTres,
            "NuevaPartida selecciona tres bosses de prueba"
        );
    }

    private void TestPruebasNoRepetidas()
    {
        PrepararNuevaPartida();

        bool noRepetidas =
            gameManager.pruebas[0] !=
                gameManager.pruebas[1] &&
            gameManager.pruebas[0] !=
                gameManager.pruebas[2] &&
            gameManager.pruebas[1] !=
                gameManager.pruebas[2];

        Comprobar(
            noRepetidas,
            "Los tres bosses de prueba no se repiten"
        );
    }

    private void TestNuevaPartidaEligeBossFinal()
    {
        PrepararNuevaPartida();

        bool bossValido =
            gameManager.bossFinal != null &&
            gameManager.bossFinal.esFinal &&
            gameManager.bossesFinales.Contains(
                gameManager.bossFinal
            );

        Comprobar(
            bossValido,
            "NuevaPartida selecciona un boss final válido"
        );
    }

    private void TestSaltarPrueba()
    {
        PrepararNuevaPartida();

        gameManager.SaltarPrueba(0);

        Comprobar(
            gameManager.pruebaResuelta[0] &&
            gameManager.pruebaSaltada[0],
            "SaltarPrueba marca la prueba como resuelta y saltada"
        );
    }

    private void TestSaltarPruebaRecuperaAliento()
    {
        PrepararNuevaPartida();
        gameManager.aliento = 4;

        gameManager.SaltarPrueba(1);

        Comprobar(
            gameManager.aliento == 7,
            "Saltar una prueba recupera 3 puntos de aliento"
        );
    }

    private void TestSaltarPruebaNoSuperaMaximo()
    {
        PrepararNuevaPartida();
        gameManager.aliento = 9;

        gameManager.SaltarPrueba(2);

        Comprobar(
            gameManager.aliento == 10,
            "Saltar una prueba no supera el aliento máximo"
        );
    }

    private void TestGanarPrueba()
    {
        PrepararNuevaPartida();

        gameManager.GanarPrueba(0);

        Comprobar(
            gameManager.pruebaResuelta[0] &&
            !gameManager.pruebaSaltada[0],
            "GanarPrueba marca la prueba como ganada"
        );
    }

    private void TestGanarPruebaRecuperaAliento()
    {
        PrepararNuevaPartida();
        gameManager.aliento = 3;

        gameManager.GanarPrueba(1);

        Comprobar(
            gameManager.aliento == 8,
            "Ganar una prueba recupera la mitad del aliento máximo"
        );
    }

    private void TestGanarPruebaNoSuperaMaximo()
    {
        PrepararNuevaPartida();
        gameManager.aliento = 8;

        gameManager.GanarPrueba(2);

        Comprobar(
            gameManager.aliento == 10,
            "Ganar una prueba no supera el aliento máximo"
        );
    }

    private void TestGanarPruebaAnadeHabilidad()
    {
        PrepararNuevaPartida();

        string nombreBoss =
            gameManager.pruebas[0].nombre;

        gameManager.GanarPrueba(0);

        Comprobar(
            gameManager.habilidades.Count == 1 &&
            gameManager.habilidades[0] ==
                nombreBoss,
            "Ganar una prueba añade la habilidad correspondiente"
        );
    }

    private void TestNoTodasResueltas()
    {
        PrepararNuevaPartida();

        gameManager.GanarPrueba(0);
        gameManager.SaltarPrueba(1);

        Comprobar(
            !gameManager.TodasLasPruebasResueltas(),
            "Queda una prueba pendiente"
        );
    }

    private void TestTodasResueltas()
    {
        PrepararNuevaPartida();

        gameManager.GanarPrueba(0);
        gameManager.GanarPrueba(1);
        gameManager.GanarPrueba(2);

        Comprobar(
            gameManager.TodasLasPruebasResueltas(),
            "Las tres pruebas quedan resueltas"
        );
    }

    private void TestProgresoCombinado()
    {
        PrepararNuevaPartida();
        gameManager.aliento = 1;

        gameManager.GanarPrueba(0);
        gameManager.SaltarPrueba(1);
        gameManager.GanarPrueba(2);

        bool estadoCorrecto =
            gameManager.TodasLasPruebasResueltas() &&
            gameManager.pruebaResuelta[0] &&
            gameManager.pruebaResuelta[1] &&
            gameManager.pruebaResuelta[2] &&
            !gameManager.pruebaSaltada[0] &&
            gameManager.pruebaSaltada[1] &&
            !gameManager.pruebaSaltada[2] &&
            gameManager.habilidades.Count == 2 &&
            gameManager.aliento == 10;

        Comprobar(
            estadoCorrecto,
            "Se combinan correctamente pruebas ganadas y saltadas"
        );
    }

    private void TestRevelarCartaAleatoria()
    {
        List<Carta> cartas =
            CrearTresCartasBoss();

        Carta revelada =
            pistaBoss.RevelarCartaAleatoria(
                cartas
            );

        Comprobar(
            revelada != null &&
            cartas.Contains(revelada),
            "La pista revela una de las tres cartas del boss"
        );
    }

    private void TestIndiceReveladoValido()
    {
        List<Carta> cartas =
            CrearTresCartasBoss();

        pistaBoss.RevelarCartaAleatoria(
            cartas
        );

        Comprobar(
            pistaBoss.IndiceRevelado >= 0 &&
            pistaBoss.IndiceRevelado < 3,
            "El índice revelado está entre 0 y 2"
        );
    }

    private void TestCartaReveladaCoincideConIndice()
    {
        List<Carta> cartas =
            CrearTresCartasBoss();

        Carta revelada =
            pistaBoss.RevelarCartaAleatoria(
                cartas
            );

        Comprobar(
            revelada ==
                cartas[pistaBoss.IndiceRevelado],
            "La carta revelada coincide con el índice guardado"
        );
    }

    private void TestEventoAlRevelarPista()
    {
        List<Carta> cartas =
            CrearTresCartasBoss();

        int llamadas = 0;
        int indiceEvento = -1;
        Carta cartaEvento = null;

        System.Action<int, Carta> manejador =
            (indice, carta) =>
            {
                llamadas++;
                indiceEvento = indice;
                cartaEvento = carta;
            };

        pistaBoss.AlRevelarPista += manejador;

        Carta revelada =
            pistaBoss.RevelarCartaAleatoria(
                cartas
            );

        pistaBoss.AlRevelarPista -= manejador;

        Comprobar(
            llamadas == 1 &&
            indiceEvento ==
                pistaBoss.IndiceRevelado &&
            cartaEvento == revelada,
            "AlRevelarPista comunica el índice y la carta"
        );
    }

    private void TestOcultarPista()
    {
        List<Carta> cartas =
            CrearTresCartasBoss();

        pistaBoss.RevelarCartaAleatoria(
            cartas
        );

        pistaBoss.OcultarPista();

        Comprobar(
            pistaBoss.IndiceRevelado == -1,
            "OcultarPista restablece el índice"
        );
    }

    private void TestListaNula()
    {
        bool lanzoExcepcion = false;

        try
        {
            pistaBoss.RevelarCartaAleatoria(
                null
            );
        }
        catch (System.ArgumentNullException)
        {
            lanzoExcepcion = true;
        }

        Comprobar(
            lanzoExcepcion,
            "Una lista nula lanza una excepción"
        );
    }

    private void TestNumeroDeCartasIncorrecto()
    {
        List<Carta> cartas =
            new List<Carta>
            {
                new Carta(Palo.Llama, 3),
                new Carta(Palo.Velo, 7)
            };

        bool lanzoExcepcion = false;

        try
        {
            pistaBoss.RevelarCartaAleatoria(
                cartas
            );
        }
        catch (System.ArgumentException)
        {
            lanzoExcepcion = true;
        }

        Comprobar(
            lanzoExcepcion,
            "La pista exige exactamente tres cartas"
        );
    }

    private void Comprobar(
        bool condicion,
        string nombre)
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
            "=== RESULTADO TEST_TURNOS: " +
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

    private void Limpiar()
    {
        foreach (
            BossData boss in bossesTemporales)
        {
            if (boss != null)
            {
                Destroy(boss);
            }
        }

        bossesTemporales.Clear();

        if (objetoPista != null)
        {
            Destroy(objetoPista);
        }

        if (objetoGameManager != null)
        {
            Destroy(objetoGameManager);
        }
    }
}