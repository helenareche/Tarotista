public static class ResolutorPosicion
{
    public static ResultadoPosicion Resolver(
        Carta cartaJugador,
        Carta cartaBoss,
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

        int poderJugador =
            cartaJugador.PoderContra(cartaBoss);

        int poderBoss =
            cartaBoss.PoderContra(cartaJugador);

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
}