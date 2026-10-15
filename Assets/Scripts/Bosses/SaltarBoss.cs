
using System.Collections.Generic;
using UnityEngine;

public class SaltarBoss : MonoBehaviour
{
    [SerializeField] private SeleccionBoss seleccionBoss;

    private int alientoPrueba = 0;
    private HashSet<int> bossesSaltados = new HashSet<int>();

    public void SaltarUnBoss()
    {
        if (seleccionBoss == null)
        {
            Debug.LogWarning("Falta conectar SeleccionBoss.");
            return;
        }

        int boss = seleccionBoss.ObtenerBossSeleccionado();

        if (SeleccionBoss.combateIniciado)
        {
            Debug.Log("No puedes saltar una prueba cuyo combate ya ha comenzado.");
            return;
        }

        if (boss == -1)
        {
            Debug.Log("Primero debes seleccionar una prueba.");
            return;
        }

        if (boss == 4)
        {
            Debug.Log("No puedes saltar el Boss final.");
            return;
        }

        if (boss < 1 || boss > 3)
        {
            Debug.LogWarning("Prueba no válida.");
            return;
        }

        if (bossesSaltados.Contains(boss))
        {
            Debug.Log("Esta prueba ya ha sido saltada.");
            return;
        }



        bossesSaltados.Add(boss);
        alientoPrueba += 3;
        SeleccionBoss.CompletarPrueba();

        Debug.Log("Prueba " + boss + " saltada.");
        Debug.Log("Aliento recuperado de prueba: +3");
        Debug.Log("Aliento acumulado en pruebas: " + alientoPrueba);
    }


    public bool EstaSaltado(int numeroBoss)
    {
        return bossesSaltados.Contains(numeroBoss);
    }

}
