using System;
using UnityEngine;

public class TestVidas : MonoBehaviour
{
    private int correctos;
    private int fallidos;
    private GameObject objetoPrueba;
    private Vida vida;

    private void Start()
    {
        Debug.Log("=== INICIO TEST_VIDAS ===");

        Preparar();

        TestInicializarConVidaMaxima();
        TestInicializarConVidaActual();
        TestActualMayorQueMaxima();
        TestActualNegativa();
        TestMaximaInvalida();
        TestRecibirDanio();
        TestDanioSuperiorALaVida();
        TestDanioCeroONegativo();
        TestCurar();
        TestCurarPorEncimaDelMaximo();
        TestNoCurarEstandoMuerto();
        TestEventoAlCambiar();
        TestEventoAlMorir();

        MostrarResultado();
        Limpiar();
    }

    private void Preparar()
    {
        objetoPrueba = new GameObject("Vida_Prueba");
        vida = objetoPrueba.AddComponent<Vida>();
    }

    private void Reiniciar(int maxima, int actual)
    {
        vida.Inicializar(maxima, actual);
    }

    private void TestInicializarConVidaMaxima()
    {
        vida.Inicializar(10);

        Comprobar(
            vida.Maxima == 10 && vida.Actual == 10,
            "Inicializar(10) deja la vida en 10/10"
        );
    }

    private void TestInicializarConVidaActual()
    {
        vida.Inicializar(10, 6);

        Comprobar(
            vida.Maxima == 10 && vida.Actual == 6,
            "Inicializar(10, 6) deja la vida en 6/10"
        );
    }

    private void TestActualMayorQueMaxima()
    {
        vida.Inicializar(10, 15);

        Comprobar(
            vida.Actual == 10,
            "La vida actual no puede superar la máxima"
        );
    }

    private void TestActualNegativa()
    {
        vida.Inicializar(10, -5);

        Comprobar(
            vida.Actual == 0 && vida.EstaMuerto,
            "Una vida inicial negativa se limita a 0"
        );
    }

    private void TestMaximaInvalida()
    {
        bool lanzoExcepcion = false;

        try
        {
            vida.Inicializar(0);
        }
        catch (ArgumentOutOfRangeException)
        {
            lanzoExcepcion = true;
        }

        Comprobar(
            lanzoExcepcion,
            "Inicializar con vida máxima 0 lanza una excepción"
        );
    }

    private void TestRecibirDanio()
    {
        Reiniciar(10, 10);

        vida.RecibirDanio(3);

        Comprobar(
            vida.Actual == 7 && !vida.EstaMuerto,
            "Recibir 3 de daño reduce la vida de 10 a 7"
        );
    }

    private void TestDanioSuperiorALaVida()
    {
        Reiniciar(10, 4);

        vida.RecibirDanio(20);

        Comprobar(
            vida.Actual == 0 && vida.EstaMuerto,
            "El daño superior a la vida deja la vida en 0"
        );
    }

    private void TestDanioCeroONegativo()
    {
        Reiniciar(10, 8);

        vida.RecibirDanio(0);
        bool ignoraCero = vida.Actual == 8;

        vida.RecibirDanio(-5);
        bool ignoraNegativo = vida.Actual == 8;

        Comprobar(
            ignoraCero && ignoraNegativo,
            "El daño cero o negativo no modifica la vida"
        );
    }

    private void TestCurar()
    {
        Reiniciar(10, 4);

        vida.Curar(3);

        Comprobar(
            vida.Actual == 7,
            "Curar 3 aumenta la vida de 4 a 7"
        );
    }

    private void TestCurarPorEncimaDelMaximo()
    {
        Reiniciar(10, 8);

        vida.Curar(20);

        Comprobar(
            vida.Actual == 10,
            "Curar no permite superar la vida máxima"
        );
    }

    private void TestNoCurarEstandoMuerto()
    {
        Reiniciar(10, 0);

        vida.Curar(5);

        Comprobar(
            vida.Actual == 0 && vida.EstaMuerto,
            "No se puede curar después de morir"
        );
    }

    private void TestEventoAlCambiar()
    {
        Reiniciar(10, 10);

        int llamadas = 0;
        int actualRecibida = -1;
        int maximaRecibida = -1;

        Action<int, int> manejador = (actual, maxima) =>
        {
            llamadas++;
            actualRecibida = actual;
            maximaRecibida = maxima;
        };

        vida.AlCambiar += manejador;
        vida.RecibirDanio(4);
        vida.AlCambiar -= manejador;

        Comprobar(
            llamadas == 1 &&
            actualRecibida == 6 &&
            maximaRecibida == 10,
            "AlCambiar comunica los valores 6/10 al recibir daño"
        );
    }

    private void TestEventoAlMorir()
    {
        Reiniciar(10, 3);

        int llamadas = 0;

        Action manejador = () =>
        {
            llamadas++;
        };

        vida.AlMorir += manejador;
        vida.RecibirDanio(3);
        vida.AlMorir -= manejador;

        Comprobar(
            llamadas == 1 &&
            vida.Actual == 0 &&
            vida.EstaMuerto,
            "AlMorir se ejecuta una vez cuando la vida llega a 0"
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
            "=== RESULTADO TEST_VIDAS: " +
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
        Destroy(objetoPrueba);
    }
}