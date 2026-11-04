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