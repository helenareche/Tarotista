
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Bosses disponibles (arrastrar assets)")]
    public List<BossData> bossesPrueba;
    public List<BossData> bossesFinales;

    [Header("Cartas destruidas durante la partida")]
    public HashSet<Carta> cartasDestruidasJugador = new HashSet<Carta>();
    public HashSet<Carta> cartasDestruidasBoss = new HashSet<Carta>();

    [Header("Estado del jugador")]
    public int aliento = 10;
    public int alientoMaximo = 10;
    public List<string> habilidades = new List<string>();

    [Header("Estado de la partida")]
    public BossData[] pruebas = new BossData[3];
    public bool[] pruebaResuelta = new bool[3];
    public bool[] pruebaSaltada = new bool[3];
    public BossData bossFinal;
    public BossData bossActual;
    public int indiceActual = -1; // 0-2 = prueba, 3 = final
    public bool victoriaFinal;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void NuevaPartida()
    {
        alientoMaximo = 10;
        aliento = 10;

        habilidades.Clear();

        pruebaResuelta = new bool[3];
        pruebaSaltada = new bool[3];

        indiceActual = -1;
        bossActual = null;
        victoriaFinal = false;

        // Reiniciar las cartas destruidas de la partida anterior.
        cartasDestruidasJugador.Clear();
        cartasDestruidasBoss.Clear();

        var pool = new List<BossData>(bossesPrueba);

        for (int i = 0; i < 3 && pool.Count > 0; i++)
        {
            int r = Random.Range(0, pool.Count);
            pruebas[i] = pool[r];
            pool.RemoveAt(r);
        }

        if (bossesFinales != null && bossesFinales.Count > 0)
        {
            bossFinal = bossesFinales[
                Random.Range(0, bossesFinales.Count)
            ];
        }
        else
        {
            bossFinal = null;
            Debug.LogError("GameManager: no hay bosses finales asignados.");
        }
    }

    public void SaltarPrueba(int i)
    {
        if (i < 0 || i >= pruebaResuelta.Length)
            return;

        aliento = Mathf.Min(aliento + 3, alientoMaximo);
        pruebaResuelta[i] = true;
        pruebaSaltada[i] = true;
    }

    public void GanarPrueba(int i)
    {
        if (i < 0 || i >= pruebas.Length || pruebas[i] == null)
            return;

        aliento = Mathf.Min(
            aliento + alientoMaximo / 2,
            alientoMaximo
        );

        pruebaResuelta[i] = true;
        pruebaSaltada[i] = false;

        // Conceder la habilidad configurada en el BossData.
        ConcederHabilidad(pruebas[i].habilidadOtorgada);
    }

    // Añade una habilidad y aplica su coste inmediato si lo tiene.
    public void ConcederHabilidad(string habilidad)
    {
        if (string.IsNullOrEmpty(habilidad) ||
            habilidades.Contains(habilidad))
            return;

        habilidades.Add(habilidad);

        // Pacto (El Diablo): el Aliento máximo baja 4 para el resto de la partida.
        if (habilidad == HabilidadesJugador.Pacto)
        {
            alientoMaximo = HabilidadesJugador.AlientoMaximoTrasPacto(alientoMaximo);
            aliento = Mathf.Min(aliento, alientoMaximo);
        }
    }

    public bool TodasLasPruebasResueltas()
    {
        foreach (var resuelta in pruebaResuelta)
        {
            if (!resuelta)
                return false;
        }

        return true;
    }
}
