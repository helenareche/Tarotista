using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPrincipal : MonoBehaviour
{
    public GameObject panelTutorial;

    public void Jugar()
    {
        GameManager.Instance.NuevaPartida();
        SceneManager.LoadScene("02_Tablero");
    }

    public void AbrirTutorial()  { panelTutorial.SetActive(true); }
    public void CerrarTutorial() { panelTutorial.SetActive(false); }

    public void Salir()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
