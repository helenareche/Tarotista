using UnityEngine;

[CreateAssetMenu(fileName = "BancoSprites", menuName = "Tarotista/Banco de sprites")]
public class BancoSprites : ScriptableObject
{
    // Orden: Llama 1-14, Velo 1-14, Raíz 1-14, Abismo 1-14
    public Sprite[] menores = new Sprite[56];
    public Sprite reverso;

    public Sprite Menor(Palo palo, int valor)
    {
        int i = (int)palo * 14 + (valor - 1);
        if (menores == null || i < 0 || i >= menores.Length) return null;
        return menores[i];
    }
}