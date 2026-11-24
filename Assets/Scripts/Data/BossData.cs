
using UnityEngine;

public enum ReglaBoss
{
    Ninguna,
    GanaEmpates,
    InvierteSigno,
    OcultaPista,
    PoderExtra,
    InvierteCiclo,
    DestruyeGanadoras
}

[CreateAssetMenu(fileName = "NuevoBoss", menuName = "Tarotista/Boss")]
public class BossData : ScriptableObject
{
    public string nombre;

    [TextArea]
    public string descripcionRegla;

    public int voluntad = 12;

    public bool esFinal;

    public ReglaBoss regla;

    [Header("Recompensa")]
    public string habilidadOtorgada;

    public Sprite imagen;
}
