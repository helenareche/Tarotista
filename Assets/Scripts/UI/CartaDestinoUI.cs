using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CartaDestinoUI : MonoBehaviour
{
    public TextMeshProUGUI textoNombre;
    public Button botonJugar;
    public Button botonSaltar;
    public Image fondo;

    public Color colorOculta = new Color(0.35f, 0.2f, 0.5f);
    public Color colorResuelta = new Color(0.3f, 0.3f, 0.3f);

    public void Configurar(int indice, BossData boss, bool resuelta, bool esFinal,
                           bool bloqueada, System.Action<int> alJugar, System.Action<int> alSaltar)
    {
        botonJugar.onClick.RemoveAllListeners();
        botonSaltar.onClick.RemoveAllListeners();
        botonJugar.onClick.AddListener(() => alJugar(indice));
        botonSaltar.onClick.AddListener(() => alSaltar(indice));

        if (resuelta)
        {
            textoNombre.text = boss.nombre + "\n(hecha)";
            fondo.color = colorResuelta;
            botonJugar.gameObject.SetActive(false);
            botonSaltar.gameObject.SetActive(false);
            return;
        }

        fondo.color = colorOculta;
        textoNombre.text = bloqueada ? "???\n(bloqueada)" : (esFinal ? "BOSS FINAL" : "???");
        botonJugar.gameObject.SetActive(!bloqueada);
        botonSaltar.gameObject.SetActive(!bloqueada && !esFinal);
    }
}