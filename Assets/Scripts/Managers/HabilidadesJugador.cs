
using System.Collections.Generic;

public static class HabilidadesJugador
{
    public const string LaLuna = "La Luna";
    public const string OjoLunar = "Ojo Lunar";
    public const string Derrumbe = "Derrumbe";

    public const int ManoNormal = 5;
    public const int ManoConOjoLunar = 4;
    public const int PistasNormal = 1;
    public const int PistasConOjoLunar = 2;

    public static bool Tiene(IList<string> habilidades, string nombre)
    {
        return habilidades != null && habilidades.Contains(nombre);
    }

    public static int TamanoMano(IList<string> habilidades)
    {
        return Tiene(habilidades, OjoLunar)
            ? ManoConOjoLunar
            : ManoNormal;
    }

    public static int CartasDePista(IList<string> habilidades)
    {
        return Tiene(habilidades, OjoLunar)
            ? PistasConOjoLunar
            : PistasNormal;
    }

    public static IList<string> Actuales()
    {
        return GameManager.Instance != null
            ? GameManager.Instance.habilidades
            : null;
    }
}
