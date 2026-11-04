using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class BarraVidaUI : MonoBehaviour
{
    [SerializeField] private Vida vida;
    private Slider slider;

    private void Awake()
    {
        slider = GetComponent<Slider>();
        slider.wholeNumbers = true;
    }

    // OnEnable corre antes que el Start del CombateManager,
    // así la barra recibe la primera inicialización
    private void OnEnable()  { vida.AlCambiar += Actualizar; }
    private void OnDisable() { vida.AlCambiar -= Actualizar; }

    private void Actualizar(int actual, int maxima)
    {
        slider.maxValue = maxima;
        slider.value = actual;
    }
}