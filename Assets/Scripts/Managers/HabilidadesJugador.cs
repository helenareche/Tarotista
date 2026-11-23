using System.Collections.Generic;

// Habilidades que el jugador gana al vencer a un boss.
// GameManager.habilidades guarda el nombre del boss derrotado
// (lo registra CombateManager al ganar una prueba).
public static class HabilidadesJugador
{
    public const string LaLuna = "La Luna";

    public const int ManoNormal = 5;
    public const int ManoConLaLuna = 4;

    public const int PistasNormal = 1;
    public const int PistasConLaLuna = 2;

    public static bool Tiene(IList<string> habilidades, string nombre)
    {
        return habilidades != null && habilidades.Contains(nombre);
    }

    // Habilidad de La Luna: robas hasta 4 cartas en vez de 5...
    public static int TamanoMano(IList<string> habilidades)
    {
        return Tiene(habilidades, LaLuna) ? ManoConLaLuna : ManoNormal;
    }

    // ...pero ves 2 cartas del boss por ronda en vez de 1.
    public static int CartasDePista(IList<string> habilidades)
    {
        return Tiene(habilidades, LaLuna) ? PistasConLaLuna : PistasNormal;
    }

    // Habilidades de la partida actual (null si no hay GameManager).
    public static IList<string> Actuales()
    {
        return GameManager.Instance != null
            ? GameManager.Instance.habilidades
            : null;
    }
}
