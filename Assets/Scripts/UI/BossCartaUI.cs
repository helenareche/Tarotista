using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossCartaUI : MonoBehaviour
{
    [SerializeField] private Image imagenBoss;
    [SerializeField] private TMP_Text textoNombre;
    [SerializeField] private TMP_Text textoRegla;

    private void Start()
    {
        GameManager gm = GameManager.Instance;
        BossData boss = gm != null ? gm.bossActual : null;

        if (boss == null)
        {
            gameObject.SetActive(false); // sin boss (escena abierta suelta): se oculta
            return;
        }

        textoNombre.text = boss.nombre;
        textoRegla.text = boss.descripcionRegla;

        if (imagenBoss != null)
        {
            imagenBoss.sprite = boss.imagen;
            imagenBoss.enabled = boss.imagen != null; // sin sprite, no se dibuja un cuadro blanco
        }
    }
}