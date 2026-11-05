using UnityEngine;
using UnityEngine.EventSystems;

// Zona donde se puede soltar una carta para devolverla a la mano.
// Ponla en el objeto Mano, o en un panel transparente más grande que apunte a Mano.
public class ZonaMano : MonoBehaviour, IDropHandler
{
    [Tooltip("Contenedor de la mano. Si se deja vacío se usa este mismo objeto.")]
    [SerializeField] private Transform mano;

    private void Awake()
    {
        // Si es un panel aparte (no el propio Mano), se manda al fondo del Canvas
        // para que no tape los huecos, las cartas colocadas ni los botones.
        if (mano != null && mano != transform)
        {
            transform.SetAsFirstSibling();
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null)
            return;

        CartaArrastrable carta =
            eventData.pointerDrag.GetComponent<CartaArrastrable>();

        if (carta == null)
            return;

        carta.SoltarEnMano(mano != null ? mano : transform);
    }
}