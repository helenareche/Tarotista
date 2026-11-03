
using UnityEngine;
using UnityEngine.SceneManagement;

public class CambiarEscena : MonoBehaviour
{
    [SerializeField] private SaltarBoss saltarBoss;

    public void IrACombate()
    {
        int boss = SeleccionBoss.bossSeleccionado;

        if (boss == -1)
        {
            Debug.Log("Primero selecciona una prueba.");
            return;
        }

        if (saltarBoss != null && saltarBoss.EstaSaltado(boss))
        {
            Debug.Log("No puedes jugar una prueba que ya has saltado.");
            return;
        }


        if (SeleccionBoss.combateIniciado)
        {
            Debug.Log("Esta prueba ya tiene un combate iniciado.");
            return;
        }

        SeleccionBoss.combateIniciado = true;

        SceneManager.LoadScene("EscenaCombate");
    }

    public void VolverASeleccion()
    {
        SceneManager.LoadScene("Test_Bosses");
    }
}
