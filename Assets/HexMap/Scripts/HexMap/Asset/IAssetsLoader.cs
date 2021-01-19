namespace HexMap.Runtime
{
    public interface IAssetsLoader
    {
        void OnLoadAsset(AssetIdentity identity);
    }
}