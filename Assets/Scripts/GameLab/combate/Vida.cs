using System;
using UnityEngine;

public class Vida : MonoBehaviour
{
    public int Actual { get; private set; }
    public int Maxima { get; private set; }

    public bool EstaMuerto
    {
        get { return Actual <= 0; }
    }

    // La UI se suscribe a estos eventos
    public event Action<int, int> AlCambiar; // (actual, maxima)
    public event Action AlMorir;

    public void Inicializar(int maxima)
    {
        Inicializar(maxima, maxima);
    }

    public void Inicializar(int maxima, int actual)
    {
        if (maxima < 1)
        {
            throw new ArgumentOutOfRangeException(
                nameof(maxima),
                "La vida máxima debe ser al menos 1."
            );
        }

        Maxima = maxima;
        Actual = actual;

        if (Actual > Maxima)
        {
            Actual = Maxima;
        }

        if (Actual < 0)
        {
            Actual = 0;
        }

        NotificarCambio();
    }

    public void RecibirDanio(int cantidad)
    {
        // El daño 0 (empates o posición ganada) no hace nada
        if (EstaMuerto || cantidad <= 0)
        {
            return;
        }

        Actual -= cantidad;

        if (Actual < 0)
        {
            Actual = 0;
        }

        NotificarCambio();

        if (EstaMuerto && AlMorir != null)
        {
            AlMorir();
        }
    }

    public void Curar(int cantidad)
    {
        if (EstaMuerto || cantidad <= 0)
        {
            return;
        }

        Actual += cantidad;

        if (Actual > Maxima)
        {
            Actual = Maxima;
        }

        NotificarCambio();
    }

    private void NotificarCambio()
    {
        if (AlCambiar != null)
        {
            AlCambiar(Actual, Maxima);
        }
    }
}