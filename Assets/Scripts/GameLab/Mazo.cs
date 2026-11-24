
using System.Collections.Generic;
using UnityEngine;

public class Mazo : MonoBehaviour
{
    public CartaVisual prefabCarta;
    public Transform mano;
    public int tamMano = 5;

    private List<Carta> cartas = new List<Carta>();
    private List<Carta> descartes = new List<Carta>();

    private HashSet<Carta> CartasDestruidas
    {
        get
        {
            if (GameManager.Instance == null)
                return null;

            return GameManager.Instance.cartasDestruidasJugador;
        }
    }

    private void Start()
    {
        tamMano = HabilidadesJugador.TamanoMano(
            HabilidadesJugador.Actuales()
        );

        HashSet<Carta> destruidas = CartasDestruidas;

        foreach (Palo palo in System.Enum.GetValues(typeof(Palo)))
        {
            for (int valor = 1; valor <= 14; valor++)
            {
                Carta carta = new Carta(palo, valor);

                if (destruidas == null ||
                    !destruidas.Contains(carta))
                {
                    cartas.Add(carta);
                }
            }
        }

        Barajar(cartas);
        RellenarMano();
    }

    private void Barajar(List<Carta> lista)
    {
        for (int i = lista.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);

            (lista[i], lista[j]) =
                (lista[j], lista[i]);
        }
    }

    public void RellenarMano()
    {
        if (mano == null)
            return;

        while (mano.childCount < tamMano)
        {
            if (!Robar())
                break;
        }
    }

    private bool Robar()
    {
        if (cartas.Count == 0)
        {
            if (descartes.Count == 0)
                return false;

            HashSet<Carta> destruidas = CartasDestruidas;

            if (destruidas != null)
            {
                descartes.RemoveAll(
                    c => destruidas.Contains(c)
                );
            }

            cartas.AddRange(descartes);
            descartes.Clear();

            Barajar(cartas);
        }

        if (cartas.Count == 0)
            return false;

        Carta carta = cartas[0];
        cartas.RemoveAt(0);

        CartaVisual visual = Instantiate(prefabCarta, mano);
        visual.Configurar(carta, true);

        return true;
    }

    public void Descartar(CartaVisual visual)
    {
        DescartarSinRellenar(visual);
        RellenarMano();
    }

    public void DescartarSinRellenar(CartaVisual visual)
    {
        if (visual == null)
            return;

        Carta carta = visual.Carta;
        HashSet<Carta> destruidas = CartasDestruidas;

        if (carta != null &&
            (destruidas == null ||
             !destruidas.Contains(carta)))
        {
            descartes.Add(carta);
        }

        // Sacarla de la mano inmediatamente.
        // Así childCount ya no cuenta esta carta
        // cuando RellenarMano comprueba cuántas hay.
        visual.transform.SetParent(null);

        Destroy(visual.gameObject);
    }

    public void Destruir(CartaVisual visual)
    {
        if (visual == null)
            return;

        Carta carta = visual.Carta;

        if (carta != null)
        {
            HashSet<Carta> destruidas = CartasDestruidas;

            if (destruidas != null)
            {
                destruidas.Add(carta);
            }
            else
            {
                Debug.LogWarning(
                    "Mazo: no se ha podido registrar la carta destruida."
                );
            }

            cartas.Remove(carta);
            descartes.Remove(carta);
        }

        visual.transform.SetParent(null);
        Destroy(visual.gameObject);
    }
}
