
using UnityEngine;

public class SeleccionBoss : MonoBehaviour
{
    public static int bossSeleccionado = -1;

    private static int pruebaActual = 1;

    public static bool combateIniciado = false;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ReiniciarProgreso()
    {
        bossSeleccionado = -1;
        pruebaActual = 1;
        combateIniciado = false;
    }


    public void SeleccionarUnBoss(int numeroBoss)
    {
        if (numeroBoss < 1 || numeroBoss > 4)
        {
            Debug.LogWarning("Boss no válido.");
            return;
        }

        if (numeroBoss != pruebaActual)
        {
            Debug.Log("Primero debes resolver la Prueba " + pruebaActual);
            return;
        }

        bossSeleccionado = numeroBoss;

        if (numeroBoss == 4)
        {
            Debug.Log("Boss final seleccionado.");
        }
        else
        {
            Debug.Log("Prueba " + numeroBoss + " seleccionada.");
        }
    }

    public int ObtenerBossSeleccionado()
    {
        return bossSeleccionado;
    }

    public static void CompletarPrueba()
    {
        if (pruebaActual < 4)
        {
            pruebaActual++;
            bossSeleccionado = -1;

            Debug.Log("Siguiente prueba desbloqueada: " + pruebaActual);
        }
    }
}
