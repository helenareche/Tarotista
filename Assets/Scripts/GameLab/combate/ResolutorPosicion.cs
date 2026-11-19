public static class ResolutorPosicion
{
    public static ResultadoPosicion Resolver(
        Carta cartaJugador,
        Carta cartaBoss,
        int danioPosicion,
        ReglaBoss regla = ReglaBoss.Ninguna,
        bool cicloInvertido = false)
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
            cartaJugador.PoderContra(cartaBoss, cicloInvertido);

        int poderBoss =
            cartaBoss.PoderContra(cartaJugador, cicloInvertido);

        bool ganaJugador;
        bool ganaBoss;

        if (regla == ReglaBoss.InvierteSigno)
        {
            // El Ahorcado: el Poder se calcula igual (con Afinidad),
            // pero gana la carta con MENOS Poder.
            ganaJugador = poderJugador < poderBoss;
            ganaBoss = poderBoss < poderJugador;
        }
        else
        {
            ganaJugador = poderJugador > poderBoss;
            ganaBoss = poderBoss > poderJugador;
        }

        if (ganaJugador)
        {
            return new ResultadoPosicion(
                GanadorPosicion.Jugador,
                poderJugador,
                poderBoss,
                0,
                danioPosicion
            );
        }

        if (ganaBoss)
        {
            return new ResultadoPosicion(
                GanadorPosicion.Boss,
                poderJugador,
                poderBoss,
                danioPosicion,
                0
            );
        }

        // El Diablo: gana todos los empates (el jugador recibe el daño).
        if (regla == ReglaBoss.GanaEmpates)
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
