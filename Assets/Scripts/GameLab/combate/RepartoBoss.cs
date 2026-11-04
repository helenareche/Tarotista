using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class RepartoBoss : MonoBehaviour
{
    [Header("Información en pantalla")]
    [SerializeField] private GameObject panelInfoBoss;
    [SerializeField] private TMP_Text textoInfoBoss;
    [SerializeField] private float segundosMensaje = 10f;

    private Coroutine ocultarMensajeCoroutine;
    [Header("Prefab utilizado también por el mazo")]
    [SerializeField] private CartaVisual prefabCarta;

    [Header("Slots del boss")]
    [SerializeField] private Transform slotPasado;
    [SerializeField] private Transform slotPresente;
    [SerializeField] private Transform slotFuturo;

    [Header("Sistema de pista")]
    [SerializeField] private PistaBoss pistaBoss;
    [SerializeField] private float retrasoPista = 2f;
    [Header("Prueba temporal")]
    [SerializeField] private bool prepararAlEmpezar = true;
    

    private readonly List<Carta> cartasBoss =
        new List<Carta>();

    private readonly List<CartaVisual> visualesBoss =
        new List<CartaVisual>();

    private void Start()
    {
        if (prepararAlEmpezar)
        {
            PrepararRonda();
        }
    }

    private void Awake()
{
    if (panelInfoBoss != null)
    {
        panelInfoBoss.SetActive(false);
    }
}
    public void PrepararRonda()
    {
        if (!ReferenciasAsignadas())
        {
            return;
        }

        LimpiarRonda();
        GenerarTresCartas();
        CrearVisualesBocaAbajo();

        if (visualesBoss.Count != 3)
        {
            Debug.LogError(
                "No se han podido crear las tres cartas del boss."
            );

            return;
        }

        BossData bossActual = null;

        if (GameManager.Instance != null)
        {
            bossActual =
                GameManager.Instance.bossActual;
        }

        if (bossActual != null &&
    bossActual.regla == ReglaBoss.OcultaPista)
{
    MostrarMensaje(
        bossActual.nombre +
        " oculta sus cartas.\n" +
        "Esta ronda no tendrás ninguna pista."
    );

    return;
}

      StartCoroutine(RevelarPistaConRetraso());
    }
    private IEnumerator RevelarPistaConRetraso()
    {
    yield return new WaitForSeconds(retrasoPista);
    RevelarPista();
    }
    private void GenerarTresCartas()
    {
        List<Carta> mazoTemporal =
            new List<Carta>();

        foreach (
            Palo palo in Enum.GetValues(typeof(Palo)))
        {
            for (int valor = 1; valor <= 14; valor++)
            {
                mazoTemporal.Add(
                    new Carta(palo, valor)
                );
            }
        }

        for (int i = 0; i < 3; i++)
        {
            int indice =
                UnityEngine.Random.Range(
                    0,
                    mazoTemporal.Count
                );

            cartasBoss.Add(
                mazoTemporal[indice]
            );

            mazoTemporal.RemoveAt(indice);
        }
    }

    private void CrearVisualesBocaAbajo()
    {
        Transform[] slots =
        {
            slotPasado,
            slotPresente,
            slotFuturo
        };

        for (int i = 0; i < 3; i++)
        {
            CartaVisual visual =
                Instantiate(
                    prefabCarta,
                    slots[i]
                );

            RectTransform rect =
                visual.GetComponent<RectTransform>();

            if (rect != null)
            {
                rect.anchorMin =
                    new Vector2(0.5f, 0.5f);

                rect.anchorMax =
                    new Vector2(0.5f, 0.5f);

                rect.pivot =
                    new Vector2(0.5f, 0.5f);

                rect.anchoredPosition =
                    Vector2.zero;

                rect.localRotation =
                    Quaternion.identity;

                rect.localScale =
                    Vector3.one;
            }

            visual.Configurar(
                cartasBoss[i],
                false
            );

            DesactivarInteraccion(
                visual
            );

            visualesBoss.Add(
                visual
            );
        }
    }

    private void DesactivarInteraccion(
        CartaVisual visual)
    {
        CartaArrastrable arrastrable =
            visual.GetComponent<CartaArrastrable>();

        if (arrastrable != null)
        {
            arrastrable.PuedeArrastrarse = false;
            arrastrable.enabled = false;
        }

        CartaSeleccionable seleccionable =
            visual.GetComponent<CartaSeleccionable>();

        if (seleccionable != null)
        {
            seleccionable.enabled = false;
        }

        CanvasGroup grupo =
            visual.GetComponent<CanvasGroup>();

        if (grupo != null)
        {
            grupo.blocksRaycasts = false;
        }
    }

    private void RevelarPista()
{
    pistaBoss.RevelarCartaAleatoria(
        cartasBoss
    );

    int indice =
        pistaBoss.IndiceRevelado;

    visualesBoss[indice].MostrarCara();

    string mensaje =
        "El boss revela una pista\n" +
        NombrePosicion(indice) +
        ": " +
        cartasBoss[indice];

    MostrarMensaje(mensaje);
}
    private string NombrePosicion(int indice)
{
    switch (indice)
    {
        case 0:
            return "Pasado";

        case 1:
            return "Presente";

        case 2:
            return "Futuro";

        default:
            return "Posición desconocida";
    }
}
    public Carta[] ObtenerCartas()
    {
        return cartasBoss.ToArray();
    }

    public void RevelarTodas()
    {
        foreach (
            CartaVisual visual in visualesBoss)
        {
            if (visual != null)
            {
                visual.MostrarCara();
            }
        }
    }

    public void LimpiarRonda()
    {
        foreach (
            CartaVisual visual in visualesBoss)
        {
            if (visual != null)
            {
                Destroy(
                    visual.gameObject
                );
            }
        }

        visualesBoss.Clear();
        cartasBoss.Clear();

        if (pistaBoss != null)
        {
            pistaBoss.OcultarPista();
        }
    }

    private bool ReferenciasAsignadas()
    {
        if (prefabCarta == null)
        {
            Debug.LogError(
                "RepartoBoss: falta Prefab Carta."
            );

            return false;
        }

        if (slotPasado == null ||
            slotPresente == null ||
            slotFuturo == null)
        {
            Debug.LogError(
                "RepartoBoss: faltan slots del boss."
            );

            return false;
        }

        if (pistaBoss == null)
        {
            Debug.LogError(
                "RepartoBoss: falta PistaBoss."
            );

            return false;
        }

        return true;
    }
   public void MostrarMensaje(string mensaje)
{
    Debug.Log(mensaje);

    if (textoInfoBoss == null)
    {
        Debug.LogWarning(
            "RepartoBoss: no se ha asignado Texto Info Boss."
        );

        return;
    }

    if (panelInfoBoss != null)
    {
        panelInfoBoss.SetActive(true);
    }

    textoInfoBoss.text = mensaje;

    if (ocultarMensajeCoroutine != null)
    {
        StopCoroutine(ocultarMensajeCoroutine);
    }

    ocultarMensajeCoroutine =
        StartCoroutine(OcultarMensajeTrasEspera());
}

private IEnumerator OcultarMensajeTrasEspera()
{
    yield return new WaitForSeconds(segundosMensaje);

    if (panelInfoBoss != null)
    {
        panelInfoBoss.SetActive(false);
    }

    ocultarMensajeCoroutine = null;
}

}