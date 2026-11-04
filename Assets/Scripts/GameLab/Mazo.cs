using System.Collections.Generic;
using UnityEngine;

public class Mazo : MonoBehaviour
{
    public CartaVisual prefabCarta;
    public Transform mano;
    public int tamMano = 5;

    List<Carta> cartas = new List<Carta>();    // cartas que aún no se han robado
    List<Carta> descartes = new List<Carta>(); // cartas usadas o descartadas

    void Start()
    {
        foreach (Palo palo in System.Enum.GetValues(typeof(Palo)))
            for (int valor = 1; valor <= 14; valor++)
                cartas.Add(new Carta(palo, valor));

        Barajar(cartas);
        RellenarMano();
    }

    // Algoritmo de Fisher-Yates
    void Barajar(List<Carta> lista)
    {
        for (int i = lista.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (lista[i], lista[j]) = (lista[j], lista[i]);
        }
    }

    // Roba hasta tener 5 cartas en la mano
    public void RellenarMano()
    {
        bool hayCarta = true;
        while (hayCarta && mano.childCount < tamMano)
        {
            hayCarta = Robar();
        }
    }

    bool Robar()
    {
        if (cartas.Count == 0)
        {
            if (descartes.Count == 0)
                return false;

            cartas.AddRange(descartes);
            descartes.Clear();
            Barajar(cartas);
        }

        Carta carta = cartas[0];
        cartas.RemoveAt(0);

        CartaVisual visual = Instantiate(prefabCarta, mano);
        visual.Configurar(carta, true);
        return true;
    }

    // Descarta una carta y rellena la mano (botón Descartar)
    public void Descartar(CartaVisual visual)
    {
        DescartarSinRellenar(visual);
        RellenarMano();
    }

    // Descarta sin robar; útil al limpiar varios slots seguidos
    public void DescartarSinRellenar(CartaVisual visual)
    {
        descartes.Add(visual.Carta);
        // Se desvincula antes de destruir para que childCount sea correcto al instante
        visual.transform.SetParent(null, false);
        Destroy(visual.gameObject);
    }
}