# UnityAssetLib
Pure .Net implemention of Unity 2017+ asset files.

Based on other great projects ([Perfare/AssetStudio](https://github.com/Perfare/AssetStudio), [HearthSim/UnityPack](https://github.com/HearthSim/UnityPack))

## Example usage
Replacing TMPro.TMP_FontAsset MonoBehaviour and Texture2D font atlas

```c#
private static void Patch()
{
    string assetPath       = @"Kingmaker_Data\sharedassets0.assets";
    string assetBackupPath = @"Kingmaker_Data\sharedassets0.assets.bak";

    string tempPath = Path.Combine(Path.GetTempPath(), "sharedassets0.assets");

    using (AssetsFile f = AssetsFile.Open(assetPath))
    {
        UnitySerializer serializer = new UnitySerializer(f);
        
        AssetInfo fontAsset = f.GetAssetByName("SourceHanSerifSC-Regular");

        // Deserialize asset from AssetsFile using AssetInfo
        TMP_FontAsset oldFont = serializer.Deserialize<TMP_FontAsset>(fontAsset);

        // Deserialize asset from resource
        TMP_FontAsset newFont = serializer.Deserialize<TMP_FontAsset>(Properties.Resources.NanumMyungjo_monobehaviour);

        // Fix PPtr target
        newFont.m_Script = oldFont.m_Script;
        newFont.material = oldFont.material;
        newFont.atlas    = oldFont.atlas;

        // Replace with new asset
        f.ReplaceAsset(fontAsset.pathID, serializer.Serialize(newFont));

        // Replace Texture2D asset
        var atlasAsset = f.GetAssetByName("SourceHanSerifSC-Regular Atlas");
        f.ReplaceAsset(atlasAsset.pathID, Properties.Resources.NanumMyungjo_texture);

        // Save at temp folder
        f.Save(tempPath);
    }

    if (File.Exists(assetBackupPath))
        File.Delete(assetBackupPath);

    // Backup original file, move temp file to original file path
    File.Move(assetPath, assetBackupPath);
    File.Move(tempPath, assetPath);
}
```
