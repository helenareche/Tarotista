
using System.Collections.Generic;

public static class HabilidadesJugador
{
    public const string LaLuna = "La Luna";
    public const string OjoLunar = "Ojo Lunar";
    public const string Derrumbe = "Derrumbe";
    public const string Pacto = "Pacto";

    public const int ManoNormal = 5;
    public const int ManoConOjoLunar = 4;
    public const int PistasNormal = 1;
    public const int PistasConOjoLunar = 2;

    // Pacto (El Diablo): +2 de daño por cada posición ganada,
    // pero el Aliento máximo baja 4 para el resto de la partida.
    public const int DanioExtraPacto = 2;
    public const int ReduccionAlientoPacto = 4;

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

    public static int DanioExtraPorVictoria(IList<string> habilidades)
    {
        return Tiene(habilidades, Pacto) ? DanioExtraPacto : 0;
    }

    // Aliento máximo tras aceptar el Pacto (nunca baja de 1).
    public static int AlientoMaximoTrasPacto(int alientoMaximo)
    {
        int nuevo = alientoMaximo - ReduccionAlientoPacto;
        return nuevo < 1 ? 1 : nuevo;
    }

    public static IList<string> Actuales()
    {
        return GameManager.Instance != null
            ? GameManager.Instance.habilidades
            : null;
    }
}
