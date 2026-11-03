using System;
using UnityEngine;

public enum TipoPosicion
{
    Pasado,
    Presente,
    Futuro
}

[Serializable]
public class PosicionCombate
{
    [SerializeField] private TipoPosicion tipo;

    public TipoPosicion Tipo
    {
        get { return tipo; }
    }

    public int Danio
    {
        get
        {
            switch (tipo)
            {
                case TipoPosicion.Pasado:
                    return 1;

                case TipoPosicion.Presente:
                    return 2;

                case TipoPosicion.Futuro:
                    return 3;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public PosicionCombate(TipoPosicion tipo)
    {
        this.tipo = tipo;
    }

    public ResultadoPosicion Resolver(
        Carta_Combate cartaJugador,
        Carta_Combate cartaBoss)
    {
        return ResolutorPosicion.Resolver(
            cartaJugador,
            cartaBoss,
            Danio
        );
    }
}