
using System.Collections.Generic;
using UnityEngine;

public class ColocacionAutomaticaBoss : MonoBehaviour
{
    private readonly List<Carta_Combate> cartasBoss =
        new List<Carta_Combate>();

    // Prepara tres cartas diferentes para cada ronda.
    public List<Carta_Combate> ColocarCartas()
    {
        cartasBoss.Clear();

        List<Carta_Combate> mazoBoss =
            new List<Carta_Combate>();

        // Crear las 56 cartas del mazo.
        foreach (Palos palo in System.Enum.GetValues(typeof(Palos)))
        {
            for (int valor = 1; valor <= 14; valor++)
            {
                mazoBoss.Add(new Carta_Combate(palo, valor));
            }
        }

        // Elegir tres cartas sin repetir.
        for (int i = 0; i < 3; i++)
        {
            int indice = Random.Range(0, mazoBoss.Count);

            Carta_Combate elegida = mazoBoss[indice];
            cartasBoss.Add(elegida);

            mazoBoss.RemoveAt(indice);
        }

        Debug.Log("Boss: tres cartas preparadas.");

        return new List<Carta_Combate>(cartasBoss);
    }
}
