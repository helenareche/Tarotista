
using System.Collections.Generic;
using UnityEngine;

public class TurnoAutomaticoBoss : MonoBehaviour
{
    [SerializeField] private ColocacionAutomaticaBoss colocacion;
    [SerializeField] private PistaBoss pista;

    private List<Carta_Combate> cartasActuales =
        new List<Carta_Combate>();

    public IReadOnlyList<Carta_Combate> CartasActuales
    {
        get { return cartasActuales; }
    }

    // Prepara las tres cartas del boss y revela una pista.
    public void PrepararRonda()
    {
        if (colocacion == null || pista == null)
        {
            Debug.LogError("Faltan componentes del boss.");
            return;
        }

        cartasActuales = colocacion.ColocarCartas();

        Carta_Combate cartaRevelada =
            pista.RevelarCartaAleatoria(cartasActuales);

        Debug.Log("Boss preparado para la ronda.");
        Debug.Log("Pista revelada: " + cartaRevelada);
    }

    // Copia las cartas para que el combate pueda resolverlas.
    public List<Carta_Combate> ObtenerCartas()
    {
        return new List<Carta_Combate>(cartasActuales);
    }

    // Limpia las cartas al finalizar la ronda.
    public void FinalizarRonda()
    {
        if (pista != null)
        {
            pista.OcultarPista();
        }

        cartasActuales.Clear();
    }
}
