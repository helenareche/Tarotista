using UnityEngine;
using UnityEngine.UI;
using TMPro;


//Clase que dibuja una carta en pantalla
public class CartaVisual : MonoBehaviour
{
    public TMP_Text texto; //muestra "el valor de la figura y su palo"
    public Image fondo; //cambia el color según el palo
     public Carta Carta { //los datos de la carta
        get;
        private set; }

    public bool BocaArriba { //si se ve la cara o el reverso
        get; 
        private set; //solo este script puede modificar este atributo
    }

   //Que carta es y su estado
    public void Configurar(Carta carta, bool bocaArriba) {
        Carta = carta;
        BocaArriba = bocaArriba;
        Actualizar();
    }

    public void Voltear() {
        BocaArriba = !BocaArriba;
        Actualizar();
    }

    void Actualizar() {
        if (Carta == null) return;

        if (BocaArriba) { //Si está boca arriba: escribe el valor y el palo
            texto.text = Carta.NombreValor + "\n" + Carta.Palo;
            fondo.color = ColorDelPalo(Carta.Palo);
        }
        else
        {
            texto.text = "?";
            fondo.color = Color.gray;
        }
    }

    Color ColorDelPalo(Palo palo) {
        switch (palo){
            case Palo.Llama: return new Color(1f, 0.5f, 0.3f);
            case Palo.Velo: return new Color(0.6f, 0.7f, 1f);
            case Palo.Raiz: return new Color(0.5f, 0.8f, 0.4f);
            default: return new Color(0.7f, 0.5f, 0.8f); // Abismo
        }
    }
}
