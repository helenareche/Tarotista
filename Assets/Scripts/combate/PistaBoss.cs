using System;
using System.Collections.Generic;
using UnityEngine;

public class PistaBoss : MonoBehaviour
{
    public event Action<int, Carta_Combate> AlRevelarPista;

    private int indiceRevelado = -1;

    public int IndiceRevelado
    {
        get { return indiceRevelado; }
    }

    public Carta_Combate RevelarCartaAleatoria(
        IReadOnlyList<Carta_Combate> cartasBoss)
    {
        if (cartasBoss == null)
        {
            throw new ArgumentNullException(nameof(cartasBoss));
        }

        if (cartasBoss.Count != 3)
        {
            throw new ArgumentException(
                "El boss debe tener exactamente tres cartas colocadas.",
                nameof(cartasBoss)
            );
        }

        indiceRevelado = UnityEngine.Random.Range(
            0,
            cartasBoss.Count
        );

        Carta_Combate cartaRevelada =
            cartasBoss[indiceRevelado];

        Debug.Log(
            "Pista del boss: posición " +
            indiceRevelado +
            ", carta " +
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
}