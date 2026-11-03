using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Bosses disponibles (arrastrar assets)")]
    public List<BossData> bossesPrueba;
    public List<BossData> bossesFinales;

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

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
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

        var pool = new List<BossData>(bossesPrueba);
        for (int i = 0; i < 3 && pool.Count > 0; i++)
        {
            int r = Random.Range(0, pool.Count);
            pruebas[i] = pool[r];
            pool.RemoveAt(r);
        }
        bossFinal = bossesFinales[Random.Range(0, bossesFinales.Count)];
    }

    public void SaltarPrueba(int i)
    {
        aliento = Mathf.Min(aliento + 3, alientoMaximo);
        pruebaResuelta[i] = true;
        pruebaSaltada[i] = true;
    }

    public void GanarPrueba(int i)
    {
        aliento = Mathf.Min(aliento + alientoMaximo / 2, alientoMaximo);
        pruebaResuelta[i] = true;
        pruebaSaltada[i] = false;
        habilidades.Add(pruebas[i].nombre); // placeholder de habilidad
    }

    public bool TodasLasPruebasResueltas()
    {
        foreach (var r in pruebaResuelta) if (!r) return false;
        return true;
    }
}