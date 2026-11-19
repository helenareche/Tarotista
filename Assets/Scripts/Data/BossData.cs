using UnityEngine;

public enum ReglaBoss
{
    Ninguna,
    GanaEmpates,   // El Diablo
    InvierteSigno, // El Ahorcado
    OcultaPista,   // La Luna
    PoderExtra,    // El Mago (opcional)
    InvierteCiclo  // El Loco: el ciclo de Afinidades puede invertirse cada ronda
}

[CreateAssetMenu(fileName = "NuevoBoss", menuName = "Tarotista/Boss")]
public class BossData : ScriptableObject
{
    public string nombre;
    [TextArea] public string descripcionRegla;
    public int voluntad = 12;
    public bool esFinal;
    public ReglaBoss regla;
    public Sprite imagen;
}