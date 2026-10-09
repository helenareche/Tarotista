using System;

public enum Palo { Llama, Velo, Raiz, Abismo } 

public class Carta
{
    public Palo Palo { get; }
    public int Valor { get; }

    public Carta(Palo palo, int valor)
    {
        if (valor < 1 || valor > 14)
            throw new ArgumentOutOfRangeException(nameof(valor), "El valor debe estar entre 1 y 14.");

        Palo = palo;
        Valor = valor;
    }

    public override string ToString() => $"{Palo} {Valor}";
}