using UnityEditor;
using UnityEngine;

public class TextureAssetPostprocessor : AssetPostprocessor
{

    void OnPostprocessTexture(Texture2D texture2D)
    {
        TextureImporter textureImporter = assetImporter as TextureImporter;
        textureImporter.textureType = TextureImporterType.Sprite;
        //textureImporter.spriteImportMode = SpriteImportMode.Single;
        textureImporter.spritePixelsPerUnit = 100;
        textureImporter.spritePivot = new Vector2(0.5f, 0.5f);
        textureImporter.generateMipsInLinearSpace = false;
        textureImporter.mipmapEnabled = false;
        textureImporter.filterMode = FilterMode.Bilinear;
        textureImporter.textureFormat = TextureImporterFormat.AutomaticTruecolor;
        textureImporter.spritePackingTag = string.Empty;
    }
}
