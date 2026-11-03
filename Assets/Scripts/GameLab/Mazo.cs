using System.Collections.Generic;
using UnityEngine;

public class Mazo : MonoBehaviour
{
    public CartaVisual prefabCarta; 
    public Transform mano;
    public int tamMano = 5;

    List<Carta> cartas = new List<Carta>(); //el mazo, cartas que aún no se han robado
    List<Carta> descartes = new List<Carta>(); //las cartas que ya se han usado o descartado

    void Start(){
       //Se crean todas las cartas
        foreach (Palo palo in System.Enum.GetValues(typeof(Palo)))
            for (int valor = 1; valor <= 14; valor++)
                cartas.Add(new Carta(palo, valor));

        Barajar(cartas);
        RellenarMano();
    }

    //algortimo de Fisher-Yates
    void Barajar(List<Carta> lista) {
        for (int i = lista.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (lista[i], lista[j]) = (lista[j], lista[i]);
        }
    }

    // Roba hasta tener 5 cartas en la mano
    public void RellenarMano() {
        bool hayCarta = true;
        while (hayCarta && mano.childCount < tamMano){
            hayCarta = Robar();
        }
    }

    bool Robar() {
        if (cartas.Count == 0){
            // Si se acaba el mazo, se rebarajan los descartes
            if (descartes.Count == 0){
                return false;
            }
            cartas.AddRange(descartes);
            descartes.Clear();
            Barajar(cartas);
        }

        Carta carta = cartas[0]; //Saca la carta de arriba
        cartas.RemoveAt(0); //quita la carta para que no se robe dos veces

        CartaVisual visual = Instantiate(prefabCarta, mano);
        visual.Configurar(carta, true); //le dice que carta es
        return true;
    }

    // Lo llamará quien gestione el descarte
    public void Descartar(CartaVisual visual){
        descartes.Add(visual.Carta);
        Destroy(visual.gameObject);
        // Destroy no es inmediato: esperamos al final del frame para robar
        Invoke(nameof(RellenarMano), 0f);
    }
}