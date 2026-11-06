using System;
using UnityEngine;

public class TestCartas : MonoBehaviour
{
    private int correctos;
    private int fallidos;

    private void Start()
    {
        Debug.Log("=== INICIO TEST_CARTAS ===");

        TestCrearCarta();
        TestValorMinimo();
        TestValorMaximo();
        TestValorInferiorInvalido();
        TestValorSuperiorInvalido();
        TestNombreValoresNumericos();
        TestNombreFiguras();
        TestToString();
        TestVentajasEntrePalos();
        TestSinVentajaMismoPalo();
        TestVentajaConCartaNula();
        TestPoderSinVentaja();
        TestPoderConVentaja();
        TestPoderContraCartaNula();
        TestBonoAfinidad();

        MostrarResultado();
    }

    private void TestCrearCarta()
    {
        Carta carta = new Carta(Palo.Llama, 7);

        Comprobar(
            carta.Palo == Palo.Llama &&
            carta.Valor == 7,
            "Una carta conserva su palo y su valor"
        );
    }

    private void TestValorMinimo()
    {
        Carta carta = new Carta(Palo.Velo, 1);

        Comprobar(
            carta.Valor == 1,
            "El valor mínimo permitido es 1"
        );
    }

    private void TestValorMaximo()
    {
        Carta carta = new Carta(Palo.Raiz, 14);

        Comprobar(
            carta.Valor == 14,
            "El valor máximo permitido es 14"
        );
    }

    private void TestValorInferiorInvalido()
    {
        bool lanzoExcepcion = false;

        try
        {
            new Carta(Palo.Llama, 0);
        }
        catch (ArgumentOutOfRangeException)
        {
            lanzoExcepcion = true;
        }

        Comprobar(
            lanzoExcepcion,
            "Una carta con valor 0 lanza una excepción"
        );
    }

    private void TestValorSuperiorInvalido()
    {
        bool lanzoExcepcion = false;

        try
        {
            new Carta(Palo.Llama, 15);
        }
        catch (ArgumentOutOfRangeException)
        {
            lanzoExcepcion = true;
        }

        Comprobar(
            lanzoExcepcion,
            "Una carta con valor 15 lanza una excepción"
        );
    }

    private void TestNombreValoresNumericos()
    {
        Carta uno = new Carta(Palo.Llama, 1);
        Carta diez = new Carta(Palo.Velo, 10);

        Comprobar(
            uno.NombreValor == "1" &&
            diez.NombreValor == "10",
            "Los valores del 1 al 10 mantienen su nombre numérico"
        );
    }

    private void TestNombreFiguras()
    {
        Carta sota = new Carta(Palo.Llama, 11);
        Carta caballo = new Carta(Palo.Velo, 12);
        Carta reina = new Carta(Palo.Raiz, 13);
        Carta rey = new Carta(Palo.Abismo, 14);

        Comprobar(
            sota.NombreValor == "Sota" &&
            caballo.NombreValor == "Caballo" &&
            reina.NombreValor == "Reina" &&
            rey.NombreValor == "Rey",
            "Los valores 11, 12, 13 y 14 tienen nombres de figura"
        );
    }

    private void TestToString()
    {
        Carta carta = new Carta(Palo.Raiz, 13);

        Comprobar(
            carta.ToString() == "Reina de Raiz",
            "ToString devuelve el nombre y el palo de la carta"
        );
    }

    private void TestVentajasEntrePalos()
    {
        Carta llama = new Carta(Palo.Llama, 5);
        Carta velo = new Carta(Palo.Velo, 5);
        Carta raiz = new Carta(Palo.Raiz, 5);
        Carta abismo = new Carta(Palo.Abismo, 5);

        bool cicloCorrecto =
            llama.TieneVentajaSobre(velo) &&
            velo.TieneVentajaSobre(raiz) &&
            raiz.TieneVentajaSobre(abismo) &&
            abismo.TieneVentajaSobre(llama);

        Comprobar(
            cicloCorrecto,
            "Se respeta el ciclo Llama > Velo > Raiz > Abismo > Llama"
        );
    }

    private void TestSinVentajaMismoPalo()
    {
        Carta primera = new Carta(Palo.Llama, 5);
        Carta segunda = new Carta(Palo.Llama, 10);

        Comprobar(
            !primera.TieneVentajaSobre(segunda),
            "Una carta no tiene ventaja sobre otra del mismo palo"
        );
    }

    private void TestVentajaConCartaNula()
    {
        Carta carta = new Carta(Palo.Llama, 5);
        bool lanzoExcepcion = false;

        try
        {
            carta.TieneVentajaSobre(null);
        }
        catch (ArgumentNullException)
        {
            lanzoExcepcion = true;
        }

        Comprobar(
            lanzoExcepcion,
            "Comprobar ventaja contra null lanza una excepción"
        );
    }

    private void TestPoderSinVentaja()
    {
        Carta llama = new Carta(Palo.Llama, 8);
        Carta raiz = new Carta(Palo.Raiz, 5);

        int poder = llama.PoderContra(raiz);

        Comprobar(
            poder == 8,
            "Sin ventaja, el poder es igual al valor de la carta"
        );
    }

    private void TestPoderConVentaja()
    {
        Carta llama = new Carta(Palo.Llama, 8);
        Carta velo = new Carta(Palo.Velo, 12);

        int poder = llama.PoderContra(velo);

        Comprobar(
            poder == 12,
            "Con ventaja, se suman 4 puntos al valor"
        );
    }

    private void TestPoderContraCartaNula()
    {
        Carta carta = new Carta(Palo.Llama, 5);
        bool lanzoExcepcion = false;

        try
        {
            carta.PoderContra(null);
        }
        catch (ArgumentNullException)
        {
            lanzoExcepcion = true;
        }

        Comprobar(
            lanzoExcepcion,
            "Calcular el poder contra null lanza una excepción"
        );
    }

    private void TestBonoAfinidad()
    {
        Comprobar(
            Carta.BonoAfinidad == 4,
            "El bono de afinidad es 4"
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
            "=== RESULTADO TEST_CARTAS: " +
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