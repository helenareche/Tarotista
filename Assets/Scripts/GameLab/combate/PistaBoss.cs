using System;
using System.Collections.Generic;
using UnityEngine;

public class PistaBoss : MonoBehaviour
{
    public event Action<int, Carta> AlRevelarPista;

    private int indiceRevelado = -1;

    public int IndiceRevelado
    {
        get { return indiceRevelado; }
    }

    public Carta RevelarCartaAleatoria(
        IReadOnlyList<Carta> cartasBoss)
    {
        if (cartasBoss == null)
        {
            throw new ArgumentNullException(
                nameof(cartasBoss)
            );
        }

        if (cartasBoss.Count != 3)
        {
            throw new ArgumentException(
                "El boss debe tener exactamente tres cartas.",
                nameof(cartasBoss)
            );
        }

        indiceRevelado =
            UnityEngine.Random.Range(
                0,
                cartasBoss.Count
            );

        Carta cartaRevelada =
            cartasBoss[indiceRevelado];

        Debug.Log(
            "PISTA DEL BOSS\n" +
            "Posición: " +
            NombrePosicion(indiceRevelado) +
            "\nCarta: " +
            cartaRevelada
        );

        if (AlRevelarPista != null)
        {
            AlRevelarPista(
                indiceRevelado,
                cartaRevelada
            );
        }

        return cartaRevelada;
    }

    // Revela varias cartas distintas (habilidad de La Luna: 2 cartas).
    public List<int> RevelarCartasAleatorias(
        IReadOnlyList<Carta> cartasBoss,
        int cantidad)
    {
        if (cartasBoss == null)
        {
            throw new ArgumentNullException(
                nameof(cartasBoss)
            );
        }

        if (cartasBoss.Count != 3)
        {
            throw new ArgumentException(
                "El boss debe tener exactamente tres cartas.",
                nameof(cartasBoss)
            );
        }

        if (cantidad < 1 || cantidad > cartasBoss.Count)
        {
            throw new ArgumentOutOfRangeException(
                nameof(cantidad),
                "Se pueden revelar entre 1 y 3 cartas."
            );
        }

        List<int> disponibles = new List<int> { 0, 1, 2 };
        List<int> reveladas = new List<int>();

        for (int i = 0; i < cantidad; i++)
        {
            int r = UnityEngine.Random.Range(0, disponibles.Count);
            reveladas.Add(disponibles[r]);
            disponibles.RemoveAt(r);
        }

        reveladas.Sort();
        indiceRevelado = reveladas[0];

        foreach (int indice in reveladas)
        {
            Debug.Log(
                "PISTA DEL BOSS\n" +
                "Posición: " +
                NombrePosicion(indice) +
                "\nCarta: " +
                cartasBoss[indice]
            );

            if (AlRevelarPista != null)
            {
                AlRevelarPista(indice, cartasBoss[indice]);
            }
        }

        return reveladas;
    }

    public void OcultarPista()
    {
        indiceRevelado = -1;
    }

    private string NombrePosicion(int indice)
    {
        switch (indice)
        {
            case 0:
                return "Pasado";

            case 1:
                return "Presente";

            case 2:
                return "Futuro";

            default:
                return "Desconocida";
        }
    }
}