
using UnityEngine;

public class CursorPersonalizado : MonoBehaviour
{
    [SerializeField] private Texture2D imagenCursor;

    private static CursorPersonalizado instancia;

    private void Awake()
    {
        if (instancia != null && instancia != this)
        {
            Destroy(gameObject);
            return;
        }

        instancia = this;
        DontDestroyOnLoad(gameObject);

        if (imagenCursor != null)
        {
            Cursor.SetCursor(
                imagenCursor,
                Vector2.zero,
                CursorMode.Auto
            );
        }
    }

    private void OnDestroy()
    {
        if (instancia == this)
        {
            instancia = null;
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
    }
}
