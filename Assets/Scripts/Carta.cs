using System;

public enum Palo { Llama, Velo, Raiz, Abismo } 

public class Carta
{
    public Palo Palo { get; } //solo lectura
    public int Valor { get; } //1 a 14

    public Carta(Palo palo, int valor){
        if (valor < 1 || valor > 14)
            throw new ArgumentOutOfRangeException(nameof(valor), "El valor debe estar entre 1 y 14.");

        Palo = palo;
        Valor = valor;
    }

    public string NombreValor{
        get{
            switch(Valor){
                case 11: return "Sota";
                case 12: return "Caballo";
                case 13: return "Reina";
                case 14: return "Rey";
                default: return Valor.ToString();
            }
        }
    } 
    public override string ToString() => $"{NombreValor} de {Palo}";
    
    
}