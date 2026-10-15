
using UnityEngine;

public class ComprobarBoss : MonoBehaviour
{
    void Start()
    {
        int boss = SeleccionBoss.bossSeleccionado;

        if (boss >= 1 && boss <= 3)
        {
            Debug.Log("Entrando al combate de la Prueba " + boss);
        }
        else if (boss == 4)
        {
            Debug.Log("Entrando al combate del Boss final");
        }
        
        
        else
        {
            Debug.LogWarning("No se ha seleccionado ninguna prueba.");
        }
    }
}
