
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

    [Header("Informacion del Boss")]
    [SerializeField] private TMP_Text textoNombreBoss;
    [SerializeField] private TMP_Text textoDescripcionBoss;

    [Header("Marcador de enfrentamiento")]
    [SerializeField] private GameObject panelEnfrentamiento;
    [SerializeField] private RectTransform marcadorJugador;
    [SerializeField] private RectTransform marcadorBoss;
    [SerializeField] private TMP_Text textoPoderJugador;
    [SerializeField] private TMP_Text textoPoderBoss;

    [Header("Animacion de cartas y marcadores")]
    [SerializeField] private float elevacionCartas = 25f;
    [SerializeField] private float expansionMarcadores = 1.12f;
    [SerializeField] private float duracionMovimiento = 0.4f;

    [Header("Tiempos de enfrentamiento")]
    [SerializeField] private float pausaCartaJugador = 1f;
    [SerializeField] private float pausaCartaBoss = 1f;
    [SerializeField] private float pausaEvaluacion = 1f;
    [SerializeField] private float pausaMostrarResultado = 1.2f;
    [SerializeField] private float pausaMostrarDanio = 1.2f;

    [Header("UI de victoria")]
    [SerializeField] private GameObject panelVictoria;
    [SerializeField] private TMP_Text textoTituloVictoria;
    [SerializeField] private TMP_Text textoVidaVictoria;
    [SerializeField] private TMP_Text textoHabilidadVictoria;
    [SerializeField] private TMP_Text textoDescripcionVictoria;
    [SerializeField] private Button botonAceptarVictoria;

    [Header("Tiempos generales")]
    [SerializeField] private float pausaRevelado = 1f;
    [SerializeField] private float pausaEntrePosiciones = 0.5f;
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

    private bool derrumbeActivado;
    private bool resolviendo;
    private bool combateTerminado;
    private bool esFinal;
    private bool aceptarVictoria;

    private ReglaBoss reglaBoss = ReglaBoss.Ninguna;

    private Vector3 escalaOriginalJugador;
    private Vector3 escalaOriginalBoss;

    private Vector2 posicionOriginalMarcadorJugador;
    private Vector2 posicionOriginalMarcadorBoss;

    private void Start()
    {
        GameManager gm = GameManager.Instance;

        esFinal = gm.indiceActual == 3;

        // Cargamos la información del boss seleccionado.
        if (gm.bossActual != null)
        {
            reglaBoss = gm.bossActual.regla;

            if (textoNombreBoss != null)
                textoNombreBoss.text = gm.bossActual.nombre;

            if (textoDescripcionBoss != null)
                textoDescripcionBoss.text = gm.bossActual.descripcionRegla;
        }
        else
        {
            Debug.LogWarning(
                "No hay ningún BossData asignado en GameManager."
            );

            if (textoNombreBoss != null)
                textoNombreBoss.text = "Boss desconocido";

            if (textoDescripcionBoss != null)
                textoDescripcionBoss.text = "";
        }

        // Reiniciamos Ojo Lunar al comenzar este combate.
        if (repartoBoss != null)
            repartoBoss.ReiniciarOjoLunar();

        // Guardamos las posiciones y escalas originales.
        if (marcadorJugador != null)
        {
            escalaOriginalJugador = marcadorJugador.localScale;
            posicionOriginalMarcadorJugador =
                marcadorJugador.anchoredPosition;
        }

        if (marcadorBoss != null)
        {
            escalaOriginalBoss = marcadorBoss.localScale;
            posicionOriginalMarcadorBoss =
                marcadorBoss.anchoredPosition;
        }

        OcultarMarcador();
        RestaurarEscalasMarcadores();
        RestaurarPosicionesMarcadores();

        if (textoPoderJugador != null)
            textoPoderJugador.text = "";

        if (textoPoderBoss != null)
            textoPoderBoss.text = "";

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

        if (textoResultado != null)
            textoResultado.text = "";

        // Preparamos los marcadores para la nueva ronda.
        OcultarMarcador();
        RestaurarEscalasMarcadores();
        RestaurarPosicionesMarcadores();

        if (textoPoderJugador != null)
            textoPoderJugador.text = "";

        if (textoPoderBoss != null)
            textoPoderBoss.text = "";

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

        ResultadoPosicion[] resultados = new ResultadoPosicion[3];

        bool hayVictoriaJugador = false;

        // Calculamos los resultados de las tres posiciones.
        for (int i = 0; i < 3; i++)
        {
            resultados[i] = posiciones[i].Resolver(
                jugador[i],
                boss[i],
                reglaBoss,
                cicloInvertido
            );

            if (resultados[i].Ganador == GanadorPosicion.Jugador)
                hayVictoriaJugador = true;
        }

        bool tieneDerrumbe = HabilidadesJugador.Tiene(
            HabilidadesJugador.Actuales(),
            HabilidadesJugador.Derrumbe
        );

        bool activarDerrumbe =
            tieneDerrumbe &&
            !derrumbeActivado &&
            hayVictoriaJugador;

        if (activarDerrumbe)
        {
            derrumbeActivado = true;

            if (textoResultado != null)
                textoResultado.text +=
                    "¡DERRUMBE! ¡El daño de esta tirada se duplica!\n";
        }

        bool esLaTorre = reglaBoss == ReglaBoss.DestruyeGanadoras;

        int multiplicador = activarDerrumbe ? 2 : 1;

        // Pacto (El Diablo): cada posición ganada hace +2 de daño.
        int danioExtraPacto = HabilidadesJugador.DanioExtraPorVictoria(
            HabilidadesJugador.Actuales()
        );

        if (danioExtraPacto > 0 && hayVictoriaJugador && textoResultado != null)
            textoResultado.text +=
                "¡PACTO! Tus victorias hacen +" + danioExtraPacto + " de daño\n";

        // El panel permanece visible durante toda la resolución.
        if (panelEnfrentamiento != null)
            panelEnfrentamiento.SetActive(true);

        // Resolvemos cada posición de forma individual.
        for (int i = 0; i < 3; i++)
        {
            ResultadoPosicion r = resultados[i];

            MostrarResultado(
                posiciones[i].Tipo,
                jugador[i],
                boss[i],
                r
            );

            bool jugadorGana = r.Ganador == GanadorPosicion.Jugador;
            bool bossGana = r.Ganador == GanadorPosicion.Boss;

            int danioJugador = r.DanioAlJugador;
            int danioBoss = r.DanioAlBoss * multiplicador;

            if (jugadorGana)
                danioBoss += danioExtraPacto;

            yield return StartCoroutine(
                MostrarEnfrentamiento(
                    i,
                    r,
                    danioJugador,
                    danioBoss
                )
            );

            // Destrucción de cartas según las reglas.
            if (jugadorGana && (esLaTorre || activarDerrumbe))
                tablero.ResolverCarta(i, true);

            if (bossGana && esLaTorre)
                repartoBoss.DestruirCarta(i);

            // Aplicamos el daño al boss tras esta posición.
            if (danioBoss > 0)
            {
                vidaBoss.RecibirDanio(danioBoss);

                if (combateTerminado)
                    yield break;
            }

            // Aplicamos el daño al jugador.
            if (danioJugador > 0)
            {
                vidaJugador.RecibirDanio(danioJugador);

                if (combateTerminado)
                    yield break;
            }

            yield return new WaitForSeconds(pausaEntrePosiciones);
        }

        // Terminada la ronda, ocultamos y restauramos los marcadores.
        OcultarMarcador();
        RestaurarEscalasMarcadores();
        RestaurarPosicionesMarcadores();

        if (textoPoderJugador != null)
            textoPoderJugador.text = "";

        if (textoPoderBoss != null)
            textoPoderBoss.text = "";

        yield return new WaitForSeconds(pausaFinRonda);

        tablero.LimpiarSlots();
        repartoBoss.PrepararRonda();

        if (textoResultado != null)
            textoResultado.text = "";

        resolviendo = false;
    }

    private IEnumerator MostrarEnfrentamiento(
        int indice,
        ResultadoPosicion resultado,
        int danioJugador,
        int danioBoss)
    {
        CartaVisual cartaJugador = tablero.ObtenerVisualCarta(indice);
        CartaVisual cartaBoss = repartoBoss.ObtenerVisualCarta(indice);

        RectTransform rectJugador = cartaJugador != null
            ? cartaJugador.GetComponent<RectTransform>()
            : null;

        RectTransform rectBoss = cartaBoss != null
            ? cartaBoss.GetComponent<RectTransform>()
            : null;

        Vector2 posicionCartaJugador = rectJugador != null
            ? rectJugador.anchoredPosition
            : Vector2.zero;

        Vector2 posicionCartaBoss = rectBoss != null
            ? rectBoss.anchoredPosition
            : Vector2.zero;

        // Guardamos las posiciones originales de los marcadores.
        Vector2 posicionMarcadorJugador =
            posicionOriginalMarcadorJugador;

        Vector2 posicionMarcadorBoss =
            posicionOriginalMarcadorBoss;

        // Limpiamos los textos anteriores.
        if (textoPoderJugador != null)
            textoPoderJugador.text = "";

        if (textoPoderBoss != null)
            textoPoderBoss.text = "";

        // 1. Se eleva la carta del jugador y su cuadrado.
        if (rectJugador != null)
        {
            yield return StartCoroutine(
                MoverRectTransform(
                    rectJugador,
                    posicionCartaJugador +
                    Vector2.up * elevacionCartas
                )
            );
        }

        if (marcadorJugador != null)
        {
            yield return StartCoroutine(
                MoverRectTransform(
                    marcadorJugador,
                    posicionMarcadorJugador +
                    Vector2.up * elevacionCartas
                )
            );

            yield return StartCoroutine(
                AnimarEscala(
                    marcadorJugador,
                    escalaOriginalJugador * expansionMarcadores
                )
            );
        }

        if (textoPoderJugador != null)
            textoPoderJugador.text =
                resultado.PoderJugador.ToString();

        yield return new WaitForSeconds(pausaCartaJugador);

        // 2. Se eleva la carta del boss y su cuadrado.
        if (rectBoss != null)
        {
            yield return StartCoroutine(
                MoverRectTransform(
                    rectBoss,
                    posicionCartaBoss +
                    Vector2.up * elevacionCartas
                )
            );
        }

        if (marcadorBoss != null)
        {
            yield return StartCoroutine(
                MoverRectTransform(
                    marcadorBoss,
                    posicionMarcadorBoss +
                    Vector2.up * elevacionCartas
                )
            );

            yield return StartCoroutine(
                AnimarEscala(
                    marcadorBoss,
                    escalaOriginalBoss * expansionMarcadores
                )
            );
        }

        if (textoPoderBoss != null)
            textoPoderBoss.text =
                resultado.PoderBoss.ToString();

        yield return new WaitForSeconds(pausaCartaBoss);

        // 3. Evaluamos quién gana.
        if (textoResultado != null)
        {
            switch (resultado.Ganador)
            {
                case GanadorPosicion.Jugador:
                    textoResultado.text +=
                        "\n¡GANAS " + posiciones[indice].Tipo + "!";
                    break;

                case GanadorPosicion.Boss:
                    textoResultado.text +=
                        "\n¡EL BOSS GANA " + posiciones[indice].Tipo + "!";
                    break;

                default:
                    textoResultado.text +=
                        "\n¡EMPATE EN " + posiciones[indice].Tipo + "!";
                    break;
            }
        }

        yield return new WaitForSeconds(pausaEvaluacion);

        // 4. Mostramos el daño.
        if (textoPoderJugador != null)
        {
            textoPoderJugador.text = danioJugador > 0
                ? "-" + danioJugador + "\n"
                : "";
        }

        if (textoPoderBoss != null)
        {
            textoPoderBoss.text = danioBoss > 0
                ? "-" + danioBoss + "\n"
                : "";
        }

        yield return new WaitForSeconds(pausaMostrarDanio);

        // 5. Bajamos las cartas y los marcadores.
        if (rectJugador != null)
        {
            yield return StartCoroutine(
                MoverRectTransform(
                    rectJugador,
                    posicionCartaJugador
                )
            );
        }

        if (marcadorJugador != null)
        {
            yield return StartCoroutine(
                MoverRectTransform(
                    marcadorJugador,
                    posicionMarcadorJugador
                )
            );

            yield return StartCoroutine(
                AnimarEscala(
                    marcadorJugador,
                    escalaOriginalJugador
                )
            );
        }

        if (rectBoss != null)
        {
            yield return StartCoroutine(
                MoverRectTransform(
                    rectBoss,
                    posicionCartaBoss
                )
            );
        }

        if (marcadorBoss != null)
        {
            yield return StartCoroutine(
                MoverRectTransform(
                    marcadorBoss,
                    posicionMarcadorBoss
                )
            );

            yield return StartCoroutine(
                AnimarEscala(
                    marcadorBoss,
                    escalaOriginalBoss
                )
            );
        }

        yield return new WaitForSeconds(pausaMostrarResultado);
    }

    private IEnumerator MoverRectTransform(
        RectTransform elemento,
        Vector2 destino)
    {
        if (elemento == null)
            yield break;

        Vector2 origen = elemento.anchoredPosition;
        float tiempo = 0f;

        if (duracionMovimiento <= 0f)
        {
            elemento.anchoredPosition = destino;
            yield break;
        }

        while (tiempo < duracionMovimiento)
        {
            tiempo += Time.deltaTime;

            float t = Mathf.Clamp01(tiempo / duracionMovimiento);
            t = Mathf.SmoothStep(0f, 1f, t);

            elemento.anchoredPosition = Vector2.Lerp(
                origen,
                destino,
                t
            );

            yield return null;
        }

        elemento.anchoredPosition = destino;
    }

    private IEnumerator AnimarEscala(
        RectTransform elemento,
        Vector3 destino)
    {
        if (elemento == null)
            yield break;

        Vector3 origen = elemento.localScale;
        float tiempo = 0f;

        if (duracionMovimiento <= 0f)
        {
            elemento.localScale = destino;
            yield break;
        }

        while (tiempo < duracionMovimiento)
        {
            tiempo += Time.deltaTime;

            float t = Mathf.Clamp01(tiempo / duracionMovimiento);
            t = Mathf.SmoothStep(0f, 1f, t);

            elemento.localScale = Vector3.Lerp(
                origen,
                destino,
                t
            );

            yield return null;
        }

        elemento.localScale = destino;
    }

    private void RestaurarEscalasMarcadores()
    {
        if (marcadorJugador != null)
            marcadorJugador.localScale = escalaOriginalJugador;

        if (marcadorBoss != null)
            marcadorBoss.localScale = escalaOriginalBoss;
    }

    private void RestaurarPosicionesMarcadores()
    {
        if (marcadorJugador != null)
            marcadorJugador.anchoredPosition =
                posicionOriginalMarcadorJugador;

        if (marcadorBoss != null)
            marcadorBoss.anchoredPosition =
                posicionOriginalMarcadorBoss;
    }

    private void OcultarMarcador()
    {
        if (panelEnfrentamiento != null)
            panelEnfrentamiento.SetActive(false);
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

        string msg =
            pos + ": " + cj + " (" + r.PoderJugador + ") vs " +
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
            "Aliento: " + vidaJugador.Actual + "/" +
            vidaJugador.Maxima +
            "\nVoluntad: " + vidaBoss.Actual + "/" +
            vidaBoss.Maxima;
    }

    private void AlVictoriaJugador()
    {
        if (combateTerminado)
            return;

        combateTerminado = true;
        botonJugar.interactable = false;

        StopAllCoroutines();

        OcultarMarcador();
        RestaurarEscalasMarcadores();
        RestaurarPosicionesMarcadores();

        StartCoroutine(FinVictoria());
    }

    private IEnumerator FinVictoria()
    {
        GameManager gm = GameManager.Instance;

        if (esFinal)
        {
            if (textoResultado != null)
                textoResultado.text +=
                    "\n¡Has desafiado al destino!";

            yield return new WaitForSeconds(pausaFinCombate);

            SceneManager.LoadScene(escenaMenu);
            yield break;
        }

        int curacion = gm.alientoMaximo / 2;

        gm.aliento = Mathf.Min(
            gm.alientoMaximo,
            vidaJugador.Actual + curacion
        );

        gm.pruebaResuelta[gm.indiceActual] = true;
        gm.pruebaSaltada[gm.indiceActual] = false;

        string nombreHabilidad = gm.bossActual.habilidadOtorgada;
        string descripcionHabilidad = gm.bossActual.descripcionRegla;

        int maximoAntes = gm.alientoMaximo;

        // Añade la habilidad y aplica su coste (Pacto baja el Aliento máximo).
        gm.ConcederHabilidad(nombreHabilidad);

        if (textoTituloVictoria != null)
            textoTituloVictoria.text = "¡Ronda superada!";

        if (textoVidaVictoria != null)
        {
            textoVidaVictoria.text =
                "Aliento: " + gm.aliento + "/" +
                gm.alientoMaximo +
                "\nHas recuperado " + curacion + " de Aliento";

            if (gm.alientoMaximo < maximoAntes)
                textoVidaVictoria.text +=
                    "\nPacto: tu Aliento máximo baja de " +
                    maximoAntes + " a " + gm.alientoMaximo;
        }

        if (textoHabilidadVictoria != null)
            textoHabilidadVictoria.text =
                "Habilidad conseguida: " + nombreHabilidad;

        if (textoDescripcionVictoria != null)
            textoDescripcionVictoria.text = descripcionHabilidad;

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

        OcultarMarcador();
        RestaurarEscalasMarcadores();
        RestaurarPosicionesMarcadores();

        StartCoroutine(FinDerrota());
    }

    private IEnumerator FinDerrota()
    {
        if (textoResultado != null)
            textoResultado.text +=
                "\nHas caído... el destino estaba escrito.";

        yield return new WaitForSeconds(pausaFinCombate);

        SceneManager.LoadScene(escenaResultado);
    }
}
