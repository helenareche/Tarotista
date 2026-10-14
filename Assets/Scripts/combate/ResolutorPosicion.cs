public static class ResolutorPosicion
{
    private const int BONIFICACION_AFINIDAD = 4;

    public static ResultadoPosicion Resolver(
        Carta_Combate cartaJugador,
        Carta_Combate cartaBoss,
        int danioPosicion)
    {
        if (cartaJugador == null)
        {
            throw new System.ArgumentNullException(
                nameof(cartaJugador)
            );
        }

        if (cartaBoss == null)
        {
            throw new System.ArgumentNullException(
                nameof(cartaBoss)
            );
        }

        if (danioPosicion < 1)
        {
            throw new System.ArgumentOutOfRangeException(
                nameof(danioPosicion),
                "El daño de la posición debe ser positivo."
            );
        }

        int poderJugador = cartaJugador.Valor;
        int poderBoss = cartaBoss.Valor;

        if (TieneVentaja(
            cartaJugador.Palo,
            cartaBoss.Palo))
        {
            poderJugador += BONIFICACION_AFINIDAD;
        }
        else if (TieneVentaja(
            cartaBoss.Palo,
            cartaJugador.Palo))
        {
            poderBoss += BONIFICACION_AFINIDAD;
        }

        if (poderJugador > poderBoss)
        {
            return new ResultadoPosicion(
                GanadorPosicion.Jugador,
                poderJugador,
                poderBoss,
                0,
                danioPosicion
            );
        }

        if (poderBoss > poderJugador)
        {
            return new ResultadoPosicion(
                GanadorPosicion.Boss,
                poderJugador,
                poderBoss,
                danioPosicion,
                0
            );
        }

        return new ResultadoPosicion(
            GanadorPosicion.Empate,
            poderJugador,
            poderBoss,
            0,
            0
        );
    }

    public static bool TieneVentaja(
        Palos atacante,
        Palos defensor)
    {
        return
            atacante == Palos.Llama &&
            defensor == Palos.Velo ||

            atacante == Palos.Velo &&
            defensor == Palos.Raiz ||

            atacante == Palos.Raiz &&
            defensor == Palos.Abismo ||

            atacante == Palos.Abismo &&
            defensor == Palos.Llama;
    }
}