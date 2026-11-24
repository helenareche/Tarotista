
using System;

public enum Palo
{
    Llama = 0,
    Velo = 1,
    Raiz = 2,
    Abismo = 3
}

public class Carta : IEquatable<Carta>
{
    public const int BonoAfinidad = 4;

    public Palo Palo { get; }
    public int Valor { get; }

    public Carta(Palo palo, int valor)
    {
        if (valor < 1 || valor > 14)
            throw new ArgumentOutOfRangeException(
                nameof(valor),
                "El valor debe estar entre 1 y 14."
            );

        Palo = palo;
        Valor = valor;
    }

    public string NombreValor
    {
        get
        {
            switch (Valor)
            {
                case 11: return "Sota";
                case 12: return "Caballo";
                case 13: return "Reina";
                case 14: return "Rey";
                default: return Valor.ToString();
            }
        }
    }

    public bool TieneVentajaSobre(Carta otra, bool cicloInvertido = false)
    {
        if (otra == null)
            throw new ArgumentNullException(nameof(otra));

        if (cicloInvertido)
        {
            int siguienteDelOtro = ((int)otra.Palo + 1) % 4;
            return siguienteDelOtro == (int)Palo;
        }

        int siguientePalo = ((int)Palo + 1) % 4;
        return siguientePalo == (int)otra.Palo;
    }

    public int PoderContra(Carta otra, bool cicloInvertido = false)
    {
        if (otra == null)
            throw new ArgumentNullException(nameof(otra));

        int poder = Valor;

        if (TieneVentajaSobre(otra, cicloInvertido))
            poder += BonoAfinidad;

        return poder;
    }

    public bool Equals(Carta otra)
    {
        if (ReferenceEquals(otra, null))
            return false;

        return Palo == otra.Palo && Valor == otra.Valor;
    }

    public override bool Equals(object obj)
    {
        return Equals(obj as Carta);
    }

    public override int GetHashCode()
    {
        return ((int)Palo * 100) + Valor;
    }

    public override string ToString()
    {
        return NombreValor + " de " + Palo;
    }
}
