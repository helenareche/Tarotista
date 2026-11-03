using System;

public enum Palo { Llama = 0, Velo = 1, Raiz = 2, Abismo = 3} 

//Clase que solo guarda datos 
public class Carta
{
    public const int BonoAfinidad = 4;
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
    // Llama > Velo > Raíz > Abismo > Llama
    public bool TieneVentajaSobre(Carta otra){
        if (otra == null) throw new ArgumentNullException(nameof(otra));
        int siguientePalo = ((int)Palo + 1) % 4;
        return siguientePalo == (int)otra.Palo;
    }
    // Poder final de esta carta al enfrentarse a otra
    public int PoderContra(Carta otra){
        if (otra == null) throw new ArgumentNullException(nameof(otra));
        int poder = Valor;
        if (TieneVentajaSobre(otra)){
            poder += BonoAfinidad;
        }
        return poder;
    }
 
    public override string ToString() => $"{NombreValor} de {Palo}";
    
}