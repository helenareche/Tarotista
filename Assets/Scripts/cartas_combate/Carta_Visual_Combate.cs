using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Carta_Visual_Combate : MonoBehaviour
{
    [Header("Sprites")]
    [SerializeField] private Sprite spriteReverso;
    [SerializeField] private Sprite spriteCara;

    [Header("Estado inicial")]
    [SerializeField] private bool empiezaBocaArriba;

    private SpriteRenderer spriteRenderer;
    private bool estaBocaArriba;

    private Vector2 tamanoReverso;
    private Vector3 escalaOriginal;

    public bool EstaBocaArriba
    {
        get { return estaBocaArriba; }
    }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        escalaOriginal = transform.localScale;

        if (spriteReverso != null)
        {
            tamanoReverso = spriteReverso.bounds.size;
        }
        else
        {
            Debug.LogError(
                "No se ha asignado el sprite del reverso en " +
                gameObject.name
            );
        }
    }

    private void Start()
    {
        if (empiezaBocaArriba)
        {
            MostrarCara();
        }
        else
        {
            MostrarReverso();
        }
    }

    public void MostrarCara()
    {
        if (spriteCara == null)
        {
            Debug.LogError(
                "No se ha asignado el sprite de la cara en " +
                gameObject.name
            );

            return;
        }

        estaBocaArriba = true;
        spriteRenderer.sprite = spriteCara;

        AjustarEscalaAlReverso();
    }

    public void MostrarReverso()
    {
        if (spriteReverso == null)
        {
            Debug.LogError(
                "No se ha asignado el sprite del reverso en " +
                gameObject.name
            );

            return;
        }

        estaBocaArriba = false;
        spriteRenderer.sprite = spriteReverso;

        transform.localScale = escalaOriginal;
    }

    public void DarLaVuelta()
    {
        if (estaBocaArriba)
        {
            MostrarReverso();
        }
        else
        {
            MostrarCara();
        }
    }

    public void EstablecerCara(Sprite nuevaCara)
    {
        if (nuevaCara == null)
        {
            Debug.LogError(
                "Se ha intentado asignar una cara vacía a " +
                gameObject.name
            );

            return;
        }

        spriteCara = nuevaCara;

        if (estaBocaArriba)
        {
            spriteRenderer.sprite = spriteCara;
            AjustarEscalaAlReverso();
        }
    }

    private void AjustarEscalaAlReverso()
    {
        if (spriteRenderer.sprite == null || spriteReverso == null)
        {
            return;
        }

        Vector2 tamanoActual = spriteRenderer.sprite.bounds.size;

        if (tamanoActual.x == 0 || tamanoActual.y == 0)
        {
            return;
        }

        float escalaX = tamanoReverso.x / tamanoActual.x;
        float escalaY = tamanoReverso.y / tamanoActual.y;

        transform.localScale = new Vector3(
            escalaOriginal.x * escalaX,
            escalaOriginal.y * escalaY,
            escalaOriginal.z
        );
    }
}