using System;

[Serializable]
public class Carta_Combate
{
    public Palos Palo { get; private set; }
    public int Valor { get; private set; }

    public Carta_Combate(Palos palo, int valor)
    {
        if (valor < 1 || valor > 14)
        {
            throw new ArgumentOutOfRangeException(
                nameof(valor),
                "El valor de una Carta_Combate debe estar entre 1 y 14."
            );
        }

        Palo = palo;
        Valor = valor;
    }

    public override string ToString()
    {
        return Valor + " de " + Palo;
    }
}