namespace HexMap.Runtime
{
    public interface IAssetsLoader
    {
        bool IsNeedLoadAsset(string assetId);
        void OnLoadAsset(AssetIdentity identity);
    }
}