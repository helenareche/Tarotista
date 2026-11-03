using System.Collections;
using TMPro;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
using UnityEngine.InputSystem;
#endif

public class DialogueUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text texto;
    [SerializeField] private float segundosPorLetra = 0.03f;

    private void Awake()
    {
        if (panel != null)
        {
            panel.SetActive(false);
        }
    }

    // Escribe la línea letra a letra y espera un clic para cerrarse
    public IEnumerator Mostrar(string linea)
    {
        if (texto == null || panel == null)
        {
            Debug.LogError("DialogueUI: faltan el panel o el texto en el Inspector.");
            yield break;
        }

        panel.SetActive(true);
        texto.text = linea;
        texto.maxVisibleCharacters = 0;

        // Necesario para que TMP calcule cuántos caracteres visibles hay
        // (las etiquetas <color>, <i>... no cuentan)
        texto.ForceMeshUpdate();
        int total = texto.textInfo.characterCount;

        // Escribir letra a letra; un clic completa la línea de golpe
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

        // Esperar un clic para avanzar
        while (!ClicEsteFrame())
        {
            yield return null;
        }

        panel.SetActive(false);
    }

    // Bocadillo que desaparece solo tras 'segundosVisible'
    public IEnumerator MostrarAuto(string linea, float segundosVisible = 1.5f)
    {
        if (texto == null || panel == null)
        {
            Debug.LogError("DialogueUI: faltan el panel o el texto en el Inspector.");
            yield break;
        }

        panel.SetActive(true);
        texto.text = linea;
        texto.maxVisibleCharacters = 0;

        texto.ForceMeshUpdate();
        int total = texto.textInfo.characterCount;

        for (int i = 0; i <= total; i++)
        {
            texto.maxVisibleCharacters = i;
            yield return new WaitForSeconds(segundosPorLetra);
        }

        yield return new WaitForSeconds(segundosVisible);

        panel.SetActive(false);
    }

    // Muestra varias líneas seguidas, esperando un clic en cada una
    public IEnumerator MostrarVarias(params string[] lineas)
    {
        foreach (string linea in lineas)
        {
            yield return Mostrar(linea);
        }
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