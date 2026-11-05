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

    [Header("UI")]
    [SerializeField] private Button botonJugar;
    [SerializeField] private TMP_Text textoResultado;
    [SerializeField] private TMP_Text textoVidas;

    [Header("Tiempos")]
    [SerializeField] private float pausaRevelado = 1f;
    [SerializeField] private float pausaEntrePosiciones = 1.5f;
    [SerializeField] private float pausaFinRonda = 2f;
    [SerializeField] private float pausaFinCombate = 2.5f;

    [Header("Escenas")]
    [SerializeField] private string escenaTablero = "02_Tablero";
    [SerializeField] private string escenaMenu = "01_Menu"; // pon aquí el nombre real de tu menú
    [SerializeField] private string escenaResultado = "04_Resultado";

    [Header("Vida del boss")]
    [SerializeField] private int voluntadPrueba = 12;
    [SerializeField] private int voluntadFinal = 30;

    private readonly PosicionCombate[] posiciones =
    {
        new PosicionCombate(TipoPosicion.Pasado),
        new PosicionCombate(TipoPosicion.Presente),
        new PosicionCombate(TipoPosicion.Futuro)
    };

    private bool resolviendo;
    private bool combateTerminado;
    private bool esFinal;

    private void Start()
    {
        GameManager gm = GameManager.Instance;
        esFinal = gm.indiceActual == 3;

        vidaJugador.Inicializar(gm.alientoMaximo, gm.aliento);
        vidaBoss.Inicializar(esFinal ? voluntadFinal : voluntadPrueba);

        vidaJugador.AlCambiar += (a, m) => ActualizarVidas();
        vidaBoss.AlCambiar += (a, m) => ActualizarVidas();
        vidaJugador.AlMorir += AlDerrotaJugador;
        vidaBoss.AlMorir += AlVictoriaJugador;

        botonJugar.onClick.AddListener(AlPulsarJugar);
        botonJugar.interactable = false;

        if (textoResultado != null) textoResultado.text = "";
        ActualizarVidas();
    }

    private void OnDestroy()
    {
        if (vidaJugador != null) vidaJugador.AlMorir -= AlDerrotaJugador;
        if (vidaBoss != null) vidaBoss.AlMorir -= AlVictoriaJugador;
    }

    private void Update()
    {
        // El botón solo está activo con las 3 cartas colocadas
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

        // Se revelan las 3 cartas del boss
        repartoBoss.RevelarTodas();
        yield return new WaitForSeconds(pausaRevelado);

        for (int i = 0; i < 3; i++)
        {
            ResultadoPosicion r = posiciones[i].Resolver(jugador[i], boss[i]);

            MostrarResultado(posiciones[i].Tipo, jugador[i], boss[i], r);

            vidaJugador.RecibirDanio(r.DanioAlJugador);
            vidaBoss.RecibirDanio(r.DanioAlBoss);

            // Si alguien muere, AlMorir ya gestiona el final del combate
            if (combateTerminado)
                yield break;

            yield return new WaitForSeconds(pausaEntrePosiciones);
        }

        yield return new WaitForSeconds(pausaFinRonda);

        // Siguiente ronda: descarta lo jugado, rellena la mano y reparte el boss
        tablero.LimpiarSlots();
        repartoBoss.PrepararRonda();

        textoResultado.text = "";
        resolviendo = false;
    }

    private void MostrarResultado(TipoPosicion pos, Carta cj, Carta cb, ResultadoPosicion r)
    {
        string quien;
        switch (r.Ganador)
        {
            case GanadorPosicion.Jugador: quien = "Ganas"; break;
            case GanadorPosicion.Boss: quien = "Pierdes"; break;
            default: quien = "Empate"; break;
        }

        string msg = pos + ": " + cj + " (" + r.PoderJugador + ") vs " +
                     cb + " (" + r.PoderBoss + ") → " + quien;

        Debug.Log(msg);
        if (textoResultado != null)
            textoResultado.text += msg + "\n";
    }

    private void ActualizarVidas()
    {
        if (textoVidas == null) return;
        textoVidas.text = "Aliento: " + vidaJugador.Actual + "/" + vidaJugador.Maxima +
                          "\nVoluntad: " + vidaBoss.Actual + "/" + vidaBoss.Maxima;
    }

    private void AlVictoriaJugador()
    {
        if (combateTerminado) return;
        combateTerminado = true;
        botonJugar.interactable = false;
        StopAllCoroutines();
        StartCoroutine(FinVictoria());
    }

    private IEnumerator FinVictoria()
    {
        GameManager gm = GameManager.Instance;

        if (esFinal)
        {
            textoResultado.text += "\n¡Has desafiado al destino!";
            yield return new WaitForSeconds(pausaFinCombate);
            gm.NuevaPartida();
            SceneManager.LoadScene(escenaMenu);
            yield break;
        }

        // Ganar una prueba cura la mitad del Aliento máximo
        int curacion = gm.alientoMaximo / 2;
        gm.aliento = Mathf.Min(gm.alientoMaximo, vidaJugador.Actual + curacion);
        gm.pruebaResuelta[gm.indiceActual] = true;
        gm.pruebaSaltada[gm.indiceActual] = false;

        textoResultado.text += "\n¡Has vencido a " + gm.bossActual.nombre + "! +" + curacion + " Aliento";
        yield return new WaitForSeconds(pausaFinCombate);
        SceneManager.LoadScene(escenaTablero);
    }

    private void AlDerrotaJugador()
    {
        if (combateTerminado) return;
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