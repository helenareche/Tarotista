using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultadoUI : MonoBehaviour
{
    public void VolverAlMenu()
    {
        Debug.Log("Se ha pulsado Volver al menú");
        SceneManager.LoadScene("01_Menu");
    }
}