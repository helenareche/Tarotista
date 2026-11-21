
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CombateManager : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private TableroUI tablero;
    [SerializeField] private RepartoBoss repartoBoss;
    [SerializeField] private Vida vidaJugador;
    [SerializeField] private Vida vidaBoss;

    [Header("UI del combate")]
    [SerializeField] private Button botonJugar;
    [SerializeField] private TMP_Text textoResultado;
    [SerializeField] private TMP_Text textoVidas;

    [Header("UI de victoria")]
    [SerializeField] private GameObject panelVictoria;
    [SerializeField] private TMP_Text textoTituloVictoria;
    [SerializeField] private TMP_Text textoVidaVictoria;
    [SerializeField] private TMP_Text textoHabilidadVictoria;
    [SerializeField] private TMP_Text textoDescripcionVictoria;
    [SerializeField] private Button botonAceptarVictoria;

    [Header("Tiempos")]
    [SerializeField] private float pausaRevelado = 1f;
    [SerializeField] private float pausaEntrePosiciones = 1.5f;
    [SerializeField] private float pausaFinRonda = 2f;
    [SerializeField] private float pausaFinCombate = 2.5f;

    [Header("Escenas")]
    [SerializeField] private string escenaTablero = "02_Tablero";
    [SerializeField] private string escenaMenu = "01_Menu";
    [SerializeField] private string escenaResultado = "04_Resultado";

    [Header("Vida del boss")]
    [SerializeField] private int voluntadPrueba = 12;
    [SerializeField] private int voluntadFinal = 30;

    [Header("El Loco")]
    [Range(0f, 1f)]
    [SerializeField] private float probabilidadInvertirCiclo = 0.5f;

    private readonly PosicionCombate[] posiciones =
    {
        new PosicionCombate(TipoPosicion.Pasado),
        new PosicionCombate(TipoPosicion.Presente),
        new PosicionCombate(TipoPosicion.Futuro)
    };

    private bool resolviendo;
    private bool combateTerminado;
    private bool esFinal;
    private bool aceptarVictoria;
    private ReglaBoss reglaBoss = ReglaBoss.Ninguna;

    private void Start()
    {
        GameManager gm = GameManager.Instance;

        esFinal = gm.indiceActual == 3;

        if (gm.bossActual != null)
            reglaBoss = gm.bossActual.regla;

        vidaJugador.Inicializar(gm.alientoMaximo, gm.aliento);
        vidaBoss.Inicializar(esFinal ? voluntadFinal : voluntadPrueba);

        vidaJugador.AlCambiar += (a, m) => ActualizarVidas();
        vidaBoss.AlCambiar += (a, m) => ActualizarVidas();

        vidaJugador.AlMorir += AlDerrotaJugador;
        vidaBoss.AlMorir += AlVictoriaJugador;

        botonJugar.onClick.AddListener(AlPulsarJugar);
        botonJugar.interactable = false;

        if (botonAceptarVictoria != null)
            botonAceptarVictoria.onClick.AddListener(AceptarVictoria);

        if (panelVictoria != null)
            panelVictoria.SetActive(false);

        if (textoResultado != null)
            textoResultado.text = "";

        ActualizarVidas();
    }

    private void OnDestroy()
    {
        if (vidaJugador != null)
            vidaJugador.AlMorir -= AlDerrotaJugador;

        if (vidaBoss != null)
            vidaBoss.AlMorir -= AlVictoriaJugador;

        if (botonJugar != null)
            botonJugar.onClick.RemoveListener(AlPulsarJugar);

        if (botonAceptarVictoria != null)
            botonAceptarVictoria.onClick.RemoveListener(AceptarVictoria);
    }

    private void Update()
    {
        if (!resolviendo && !combateTerminado)
            botonJugar.interactable = tablero.TodasColocadas();
    }

    public void AlPulsarJugar()
    {
        if (resolviendo || combateTerminado || !tablero.TodasColocadas())
            return;

        StartCoroutine(ResolverRonda());
    }

    private IEnumerator ResolverRonda()
    {
        resolviendo = true;
        botonJugar.interactable = false;
        tablero.BloquearSlots(true);
        textoResultado.text = "";

        Carta[] jugador = tablero.ObtenerJugadas();
        Carta[] boss = repartoBoss.ObtenerCartas();

        repartoBoss.RevelarTodas();

        yield return new WaitForSeconds(pausaRevelado);

        bool cicloInvertido = false;

        if (reglaBoss == ReglaBoss.InvierteCiclo)
        {
            cicloInvertido = Random.value < probabilidadInvertirCiclo;

            string aviso = cicloInvertido
                ? "¡El Loco invierte el ciclo! Velo > Llama, Raíz > Velo, Abismo > Raíz, Llama > Abismo"
                : "El Loco deja el ciclo normal: Llama > Velo > Raíz > Abismo > Llama";

            Debug.Log(aviso);

            if (textoResultado != null)
                textoResultado.text += aviso + "\n";

            yield return new WaitForSeconds(pausaRevelado);
        }

        for (int i = 0; i < 3; i++)
        {
            ResultadoPosicion r = posiciones[i].Resolver(
                jugador[i],
                boss[i],
                reglaBoss,
                cicloInvertido
            );

            MostrarResultado(posiciones[i].Tipo, jugador[i], boss[i], r);

            vidaJugador.RecibirDanio(r.DanioAlJugador);
            vidaBoss.RecibirDanio(r.DanioAlBoss);

            if (combateTerminado)
                yield break;

            yield return new WaitForSeconds(pausaEntrePosiciones);
        }

        yield return new WaitForSeconds(pausaFinRonda);

        tablero.LimpiarSlots();
        repartoBoss.PrepararRonda();

        textoResultado.text = "";
        resolviendo = false;
    }

    private void MostrarResultado(
        TipoPosicion pos,
        Carta cj,
        Carta cb,
        ResultadoPosicion r)
    {
        string quien;

        switch (r.Ganador)
        {
            case GanadorPosicion.Jugador:
                quien = "Ganas";
                break;

            case GanadorPosicion.Boss:
                quien = "Pierdes";
                break;

            default:
                quien = "Empate";
                break;
        }

        string msg = pos + ": " + cj + " (" + r.PoderJugador + ") vs " +
                     cb + " (" + r.PoderBoss + ") → " + quien;

        Debug.Log(msg);

        if (textoResultado != null)
            textoResultado.text += msg + "\n";
    }

    private void ActualizarVidas()
    {
        if (textoVidas == null)
            return;

        textoVidas.text =
            "Aliento: " + vidaJugador.Actual + "/" + vidaJugador.Maxima +
            "\nVoluntad: " + vidaBoss.Actual + "/" + vidaBoss.Maxima;
    }

    private void AlVictoriaJugador()
    {
        if (combateTerminado)
            return;

        combateTerminado = true;
        botonJugar.interactable = false;

        StopAllCoroutines();
        StartCoroutine(FinVictoria());
    }

    private IEnumerator FinVictoria()
    {
        GameManager gm = GameManager.Instance;

        // Si es el combate final, se mantiene el comportamiento anterior.
        if (esFinal)
        {
            textoResultado.text += "\n¡Has desafiado al destino!";

            yield return new WaitForSeconds(pausaFinCombate);

            gm.NuevaPartida();
            SceneManager.LoadScene(escenaMenu);

            yield break;
        }

        // Curación por ganar la prueba.
        int curacion = gm.alientoMaximo / 2;

        gm.aliento = Mathf.Min(
            gm.alientoMaximo,
            vidaJugador.Actual + curacion
        );

        gm.pruebaResuelta[gm.indiceActual] = true;
        gm.pruebaSaltada[gm.indiceActual] = false;

        // Registrar la habilidad del jefe derrotado.
        string nombreHabilidad = gm.bossActual.nombre;
        string descripcionHabilidad = gm.bossActual.descripcionRegla;

        if (!gm.habilidades.Contains(nombreHabilidad))
            gm.habilidades.Add(nombreHabilidad);

        // Preparar los textos de la ventana.
        if (textoTituloVictoria != null)
            textoTituloVictoria.text = "¡Ronda superada!";

        if (textoVidaVictoria != null)
        {
            textoVidaVictoria.text =
                "Aliento: " + gm.aliento + "/" + gm.alientoMaximo +
                "\nHas recuperado " + curacion + " de Aliento";
        }

        if (textoHabilidadVictoria != null)
            textoHabilidadVictoria.text = "Habilidad conseguida: " + nombreHabilidad;

        if (textoDescripcionVictoria != null)
            textoDescripcionVictoria.text = descripcionHabilidad;

        // Mostrar la ventana y esperar a que el jugador acepte.
        aceptarVictoria = false;

        if (panelVictoria != null)
            panelVictoria.SetActive(true);

        while (!aceptarVictoria)
            yield return null;

        SceneManager.LoadScene(escenaTablero);
    }

    public void AceptarVictoria()
    {
        aceptarVictoria = true;
    }

    private void AlDerrotaJugador()
    {
        if (combateTerminado)
            return;

        combateTerminado = true;
        botonJugar.interactable = false;

        StopAllCoroutines();
        StartCoroutine(FinDerrota());
    }

    private IEnumerator FinDerrota()
    {
        textoResultado.text += "\nHas caído... el destino estaba escrito.";

        yield return new WaitForSeconds(pausaFinCombate);

        GameManager.Instance.NuevaPartida();
        SceneManager.LoadScene(escenaResultado);
    }
}
