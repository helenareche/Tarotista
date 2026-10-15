using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
using UnityEngine.InputSystem;
#endif

public class IntroHistoria : MonoBehaviour
{
    [SerializeField] private TMP_Text texto;
    [SerializeField] private CanvasGroup grupoTexto;   // para el fundido
    [SerializeField] [TextArea(2, 6)] private string[] parrafos;
    [SerializeField] private float segundosPorLetra = 0.05f;
    [SerializeField] private float duracionFundido = 1f;
    [SerializeField] private string escenaSiguiente = "Juego";

    private void Start()
    {
        if (texto == null || grupoTexto == null)
        {
            Debug.LogError("IntroHistoria: faltan el texto o el CanvasGroup en el Inspector.");
            return;
        }

        StartCoroutine(Secuencia());
    }

    private IEnumerator Secuencia()
    {
        foreach (string parrafo in parrafos)
        {
            yield return MostrarParrafo(parrafo);
        }

        SceneManager.LoadScene(escenaSiguiente);
    }

    private IEnumerator MostrarParrafo(string parrafo)
    {
        grupoTexto.alpha = 1f;
        texto.text = parrafo;
        texto.maxVisibleCharacters = 0;

        texto.ForceMeshUpdate();
        int total = texto.textInfo.characterCount;

        // Escribir letra a letra; un clic completa el párrafo
        for (int i = 0; i <= total; i++)
        {
            texto.maxVisibleCharacters = i;

            if (ClicEsteFrame())
            {
                texto.maxVisibleCharacters = total;
                break;
            }

            yield return new WaitForSeconds(segundosPorLetra);
        }

        // Evita que el mismo clic cuente dos veces
        yield return null;

        // Esperar un clic para continuar
        while (!ClicEsteFrame())
        {
            yield return null;
        }

        yield return Fundido();
    }

    private IEnumerator Fundido()
    {
        float transcurrido = 0f;

        while (transcurrido < duracionFundido)
        {
            transcurrido += Time.deltaTime;
            grupoTexto.alpha = 1f - transcurrido / duracionFundido;
            yield return null;
        }

        grupoTexto.alpha = 0f;
    }

    private bool ClicEsteFrame()
    {
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
        return Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;
#else
        return Input.GetMouseButtonDown(0);
#endif
    }
}
