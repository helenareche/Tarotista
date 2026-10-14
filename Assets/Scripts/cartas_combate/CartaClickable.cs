using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Carta_Visual_Combate))]
public class CartaClickable : MonoBehaviour
{
    private Carta_Visual_Combate cartaVisual;

    private void Awake()
    {
        cartaVisual = GetComponent<Carta_Visual_Combate>();
    }

    private void OnMouseDown()
    {
        cartaVisual.DarLaVuelta();
    }
}
