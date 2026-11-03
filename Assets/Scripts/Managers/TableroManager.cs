using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TableroManager : MonoBehaviour
{
    public Transform contenedorCartas;
    public CartaDestinoUI prefabCarta;
    public TextMeshProUGUI textoAliento;
    public TextMeshProUGUI textoMensaje;

    CartaDestinoUI[] cartas = new CartaDestinoUI[4];

    void Start()
    {
        for (int i = 0; i < 4; i++)
            cartas[i] = Instantiate(prefabCarta, contenedorCartas);

        Refrescar();
    }

    void Refrescar()
    {
        var gm = GameManager.Instance;

        textoAliento.text = $"Aliento: {gm.aliento}/{gm.alientoMaximo}";
        bool finalDesbloqueado = gm.TodasLasPruebasResueltas();
        textoMensaje.text = finalDesbloqueado
            ? "El destino te espera. Enfréntate al boss final."
            : "Juega la prueba activa o sáltala para recuperar 3 de Aliento.";

        int activa = -1;
        for (int i = 0; i < 3; i++)
        {
            if (!gm.pruebaResuelta[i]) { activa = i; break; }
        }

        for (int i = 0; i < 3; i++)
        {
            bool bloqueada = !gm.pruebaResuelta[i] && i != activa;
            cartas[i].Configurar(i, gm.pruebas[i], gm.pruebaResuelta[i], gm.pruebaSaltada[i],
                                 false, bloqueada, Jugar, Saltar);
        }

        cartas[3].Configurar(3, gm.bossFinal, false, false, true, !finalDesbloqueado, Jugar, Saltar);
    }

    void Jugar(int indice)
    {
        var gm = GameManager.Instance;
        gm.indiceActual = indice;
        gm.bossActual = indice == 3 ? gm.bossFinal : gm.pruebas[indice];
        SceneManager.LoadScene("03_Combate");
    }

    void Saltar(int indice)
    {
        GameManager.Instance.SaltarPrueba(indice);
        Refrescar();
    }
}