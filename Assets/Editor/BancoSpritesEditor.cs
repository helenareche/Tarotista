using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BancoSprites))]
public class BancoSpritesEditor : Editor
{
    private const string Carpeta = "Assets/Sprites/cartas";

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        GUILayout.Space(8);
        if (GUILayout.Button("Rellenar desde " + Carpeta))
            Rellenar((BancoSprites)target);
    }

    private static void Rellenar(BancoSprites banco)
    {
        string[] palos = { "llama", "velo", "raiz", "abismo" };
        banco.menores = new Sprite[56];
        banco.reverso = null;
        int encontradas = 0;

        foreach (string guid in AssetDatabase.FindAssets("t:Texture2D", new[] { Carpeta }))
        {
            string ruta = AssetDatabase.GUIDToAssetPath(guid);
            string nombre = System.IO.Path.GetFileNameWithoutExtension(ruta).ToLowerInvariant();

            bool esReverso = nombre == "reverso";
            int palo = -1, valor = 0;

            if (!esReverso)
            {
                int guion = nombre.LastIndexOf('_');
                if (guion < 0) continue;
                palo = System.Array.IndexOf(palos, nombre.Substring(0, guion));
                if (palo < 0) continue;
                if (!int.TryParse(nombre.Substring(guion + 1), out valor) || valor < 1 || valor > 14) continue;
            }

            // Si la imagen no está importada como Sprite, la corrige
            var importador = AssetImporter.GetAtPath(ruta) as TextureImporter;
            if (importador != null && importador.textureType != TextureImporterType.Sprite)
            {
                importador.textureType = TextureImporterType.Sprite;
                importador.spriteImportMode = SpriteImportMode.Single;
                importador.mipmapEnabled = false;
                importador.SaveAndReimport();
            }

            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(ruta);
            if (sprite == null) continue;

            if (esReverso) { banco.reverso = sprite; continue; }

            banco.menores[palo * 14 + valor - 1] = sprite;
            encontradas++;
        }

        EditorUtility.SetDirty(banco);
        AssetDatabase.SaveAssets();
        Debug.Log("Banco rellenado: " + encontradas + "/56 menores, reverso: " + (banco.reverso != null ? "sí" : "NO"));
    }
}