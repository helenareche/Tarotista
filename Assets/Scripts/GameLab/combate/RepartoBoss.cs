
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
    private Coroutine pistaCoroutine;

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

    private readonly List<Carta> cartasBoss = new List<Carta>();
    private readonly List<CartaVisual> visualesBoss = new List<CartaVisual>();

    private int indicePrimeraPista = -1;
    private bool primeraPistaRevelada = false;

    // Se conserva durante todas las rondas del mismo combate.
    private bool segundaPistaUsada = false;

    private HashSet<Carta> CartasDestruidas
    {
        get
        {
            if (GameManager.Instance == null)
                return null;

            return GameManager.Instance.cartasDestruidasBoss;
        }
    }

    private void Awake()
    {
        if (panelInfoBoss != null)
            panelInfoBoss.SetActive(false);
    }

    private void Start()
    {
        if (prepararAlEmpezar)
            PrepararRonda();
    }

    // Se llama una sola vez cuando comienza un combate nuevo.
    public void ReiniciarOjoLunar()
    {
        segundaPistaUsada = false;
    }

    public void PrepararRonda()
    {
        if (!ReferenciasAsignadas())
            return;

        LimpiarRonda();

        indicePrimeraPista = -1;
        primeraPistaRevelada = false;

        // IMPORTANTE:
        // No reiniciamos segundaPistaUsada aquí.
        // Así Ojo Lunar solo puede utilizarse una vez por combate.

        if (!GenerarTresCartas())
        {
            Debug.LogError("No quedan suficientes cartas disponibles para el boss.");
            return;
        }

        CrearVisualesBocaAbajo();

        if (visualesBoss.Count != 3)
        {
            Debug.LogError("No se han podido crear las tres cartas del boss.");
            return;
        }

        BossData bossActual = GameManager.Instance != null
            ? GameManager.Instance.bossActual
            : null;

        // La regla de La Luna oculta todas las pistas.
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

        pistaCoroutine = StartCoroutine(RevelarPistaConRetraso());
    }

    private IEnumerator RevelarPistaConRetraso()
    {
        yield return new WaitForSeconds(retrasoPista);

        RevelarPista();

        pistaCoroutine = null;
    }

    private bool GenerarTresCartas()
    {
        List<Carta> mazoTemporal = new List<Carta>();
        HashSet<Carta> destruidas = CartasDestruidas;

        foreach (Palo palo in Enum.GetValues(typeof(Palo)))
        {
            for (int valor = 1; valor <= 14; valor++)
            {
                Carta carta = new Carta(palo, valor);

                if (destruidas == null || !destruidas.Contains(carta))
                    mazoTemporal.Add(carta);
            }
        }

        if (mazoTemporal.Count < 3)
            return false;

        for (int i = 0; i < 3; i++)
        {
            int indice = UnityEngine.Random.Range(0, mazoTemporal.Count);

            cartasBoss.Add(mazoTemporal[indice]);
            mazoTemporal.RemoveAt(indice);
        }

        return true;
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
            CartaVisual visual = Instantiate(prefabCarta, slots[i]);

            RectTransform rect = visual.GetComponent<RectTransform>();

            if (rect != null)
            {
                rect.anchorMin = new Vector2(0.5f, 0.5f);
                rect.anchorMax = new Vector2(0.5f, 0.5f);
                rect.pivot = new Vector2(0.5f, 0.5f);
                rect.anchoredPosition = Vector2.zero;
                rect.localRotation = Quaternion.identity;
                rect.localScale = Vector3.one;
            }

            visual.Configurar(cartasBoss[i], false);

            DesactivarInteraccion(visual);

            CartaPistaSeleccionable seleccionable =
                visual.GetComponent<CartaPistaSeleccionable>();

            if (seleccionable == null)
            {
                seleccionable =
                    visual.gameObject.AddComponent<CartaPistaSeleccionable>();
            }

            seleccionable.Configurar(this, i);

            visualesBoss.Add(visual);
        }
    }

    private void DesactivarInteraccion(CartaVisual visual)
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
            seleccionable.enabled = false;

        CanvasGroup grupo = visual.GetComponent<CanvasGroup>();

        if (grupo != null)
        {
            grupo.blocksRaycasts = true;
            grupo.interactable = true;
        }
    }

    private void RevelarPista()
    {
        if (cartasBoss.Count != 3 || visualesBoss.Count != 3)
            return;

        pistaBoss.RevelarCartaAleatoria(cartasBoss);

        indicePrimeraPista = pistaBoss.IndiceRevelado;

        if (indicePrimeraPista < 0 ||
            indicePrimeraPista >= visualesBoss.Count)
            return;

        visualesBoss[indicePrimeraPista].MostrarCara();

        primeraPistaRevelada = true;

        bool tieneOjoLunar = HabilidadesJugador.Tiene(
            HabilidadesJugador.Actuales(),
            HabilidadesJugador.OjoLunar
        );

        string mensaje =
            "El boss revela una pista\n" +
            NombrePosicion(indicePrimeraPista) + ": " +
            cartasBoss[indicePrimeraPista];

        if (tieneOjoLunar && !segundaPistaUsada)
        {
            mensaje +=
                "\n\n¡Ojo Lunar! Selecciona otra carta del boss " +
                "para revelar una segunda pista.";
        }
        else if (tieneOjoLunar && segundaPistaUsada)
        {
            mensaje += "\n\nYa has utilizado Ojo Lunar en este combate.";
        }

        MostrarMensaje(mensaje);
    }

    public void SeleccionarSegundaPista(int indice)
    {
        if (!primeraPistaRevelada)
            return;

        if (segundaPistaUsada)
            return;

        if (indice < 0 || indice >= visualesBoss.Count)
            return;

        if (indice == indicePrimeraPista)
            return;

        if (visualesBoss[indice] == null)
            return;

        bool tieneOjoLunar = HabilidadesJugador.Tiene(
            HabilidadesJugador.Actuales(),
            HabilidadesJugador.OjoLunar
        );

        if (!tieneOjoLunar)
            return;

        BossData bossActual = GameManager.Instance != null
            ? GameManager.Instance.bossActual
            : null;

        // La regla de La Luna tiene prioridad.
        if (bossActual != null &&
            bossActual.regla == ReglaBoss.OcultaPista)
            return;

        // Se marca como usada para el resto del combate.
        segundaPistaUsada = true;

        visualesBoss[indice].MostrarCara();

        MostrarMensaje(
            "¡Ojo Lunar!\n" +
            "Segunda pista revelada.\n" +
            NombrePosicion(indice) + ": " +
            cartasBoss[indice]
        );
    }

    private string NombrePosicion(int indice)
    {
        switch (indice)
        {
            case 0: return "Pasado";
            case 1: return "Presente";
            case 2: return "Futuro";
            default: return "Posición desconocida";
        }
    }

    public Carta[] ObtenerCartas()
    {
        return cartasBoss.ToArray();
    }

    public void RevelarTodas()
    {
        foreach (CartaVisual visual in visualesBoss)
        {
            if (visual != null)
                visual.MostrarCara();
        }
    }

    public void DestruirCarta(int indice)
    {
        if (indice < 0 || indice >= cartasBoss.Count)
            return;

        Carta carta = cartasBoss[indice];

        if (carta != null)
        {
            HashSet<Carta> destruidas = CartasDestruidas;

            if (destruidas != null)
                destruidas.Add(carta);
            else
                Debug.LogWarning(
                    "RepartoBoss: no se ha podido registrar la carta destruida en GameManager."
                );
        }

        if (indice < visualesBoss.Count)
        {
            CartaVisual visual = visualesBoss[indice];

            if (visual != null)
                Destroy(visual.gameObject);

            visualesBoss[indice] = null;
        }
    }

    public void LimpiarRonda()
    {
        if (pistaCoroutine != null)
        {
            StopCoroutine(pistaCoroutine);
            pistaCoroutine = null;
        }

        foreach (CartaVisual visual in visualesBoss)
        {
            if (visual != null)
                Destroy(visual.gameObject);
        }

        visualesBoss.Clear();
        cartasBoss.Clear();

        indicePrimeraPista = -1;
        primeraPistaRevelada = false;

        // No reiniciamos segundaPistaUsada aquí.

        if (pistaBoss != null)
            pistaBoss.OcultarPista();
    }

    private bool ReferenciasAsignadas()
    {
        if (prefabCarta == null)
        {
            Debug.LogError("RepartoBoss: falta Prefab Carta.");
            return false;
        }

        if (slotPasado == null ||
            slotPresente == null ||
            slotFuturo == null)
        {
            Debug.LogError("RepartoBoss: faltan slots del boss.");
            return false;
        }

        if (pistaBoss == null)
        {
            Debug.LogError("RepartoBoss: falta PistaBoss.");
            return false;
        }

        return true;
    }

    public void MostrarMensaje(string mensaje)
    {
        Debug.Log(mensaje);

        if (textoInfoBoss == null)
        {
            Debug.LogWarning("RepartoBoss: falta Texto Info Boss.");
            return;
        }

        if (panelInfoBoss != null)
            panelInfoBoss.SetActive(true);

        textoInfoBoss.text = mensaje;

        if (ocultarMensajeCoroutine != null)
            StopCoroutine(ocultarMensajeCoroutine);

        ocultarMensajeCoroutine =
            StartCoroutine(OcultarMensajeTrasEspera());
    }

    private IEnumerator OcultarMensajeTrasEspera()
    {
        yield return new WaitForSeconds(segundosMensaje);

        if (panelInfoBoss != null)
            panelInfoBoss.SetActive(false);

        ocultarMensajeCoroutine = null;
    }
}
