
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultadoUI : MonoBehaviour
{
    public void VolverAlMenu()
    {
        Debug.Log("Volviendo al menú e iniciando una nueva partida");

        if (GameManager.Instance != null)
        {
            GameManager.Instance.NuevaPartida();
        }
        else
        {
            Debug.LogError("No se ha encontrado GameManager.");
        }

        SceneManager.LoadScene("01_Menu");
    }
}
