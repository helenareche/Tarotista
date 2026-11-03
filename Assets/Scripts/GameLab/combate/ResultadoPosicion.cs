public enum GanadorPosicion
{
    Jugador,
    Boss,
    Empate
}

public class ResultadoPosicion
{
    public GanadorPosicion Ganador { get; private set; }
    public int PoderJugador { get; private set; }
    public int PoderBoss { get; private set; }
    public int DanioAlJugador { get; private set; }
    public int DanioAlBoss { get; private set; }

    public ResultadoPosicion(
        GanadorPosicion ganador,
        int poderJugador,
        int poderBoss,
        int danioAlJugador,
        int danioAlBoss)
    {
        Ganador = ganador;
        PoderJugador = poderJugador;
        PoderBoss = poderBoss;
        DanioAlJugador = danioAlJugador;
        DanioAlBoss = danioAlBoss;
    }
}