# UnityAssetLib
Simple library to handle Unity 2017+ assets files.

Based on other great projects ([Perfare/AssetStudio](https://github.com/Perfare/AssetStudio), [HearthSim/UnityPack](https://github.com/HearthSim/UnityPack))

## Example usage
Replacing TMPro.TMP_FontAsset MonoBehaviour and Texture2D font atlas

```c#
private static void Patch()
{
    string assetPath       = @"Kingmaker_Data\sharedassets0.assets";
    string assetBackupPath = @"Kingmaker_Data\sharedassets0.assets.bak";

    string tempPath = Path.Combine(Path.GetTempPath(), "sharedassets0.assets");

    if (!File.Exists(assetPath))
    {
        Console.WriteLine("게임 애셋 파일을 찾을 수 없습니다.");
        return;
    }

    using (var f = AssetsFile.Open(assetPath))
    {
        var fontAsset = f.GetAssetByName("SourceHanSerifSC-Regular");

        if (fontAsset == null)
        {
            Console.WriteLine("Original font not found.");
            return;
        }

        // Deserialize asset from AssetsFile using AssetInfo
        TMP_FontAsset oldFont = UnitySerializer.Deserialize<TMP_FontAsset>(fontAsset);

        // Deserialize asset from byte array
        TMP_FontAsset newFont = UnitySerializer.Deserialize<TMP_FontAsset>(Properties.Resources.NanumMyungjo_monobehaviour);

        // Fix PPtr target
        newFont.m_Script = oldFont.m_Script;
        newFont.material = oldFont.material;
        newFont.atlas    = oldFont.atlas;

        // Replace with new asset
        f.ReplaceAsset(fontAsset.pathID, UnitySerializer.Serialize(newFont));

        // Replace Texture2D asset
        var atlasAsset = f.GetAssetByName("SourceHanSerifSC-Regular Atlas");
        f.ReplaceAsset(atlasAsset.pathID, Properties.Resources.NanumMyungjo_texture);

        // Save at temp folder
        f.Save(tempPath);
    }

    if (File.Exists(assetBackupPath))
    {
        File.Delete(assetBackupPath);
    }

    // Back original file, move temp file to original file path
    File.Move(assetPath, assetBackupPath);
    File.Move(tempPath, assetPath);

    Console.WriteLine("패치가 완료되었습니다.");
}
```
