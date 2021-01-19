/*
 * @Author: fasthro
 * @Date: 2021-01-08 14:19:07
 * @Description: 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HexMap.Runtime
{
    public class HexCell : MonoBehaviour, IAssetsLoader
    {
        public int index;
        public int chunkIndex;
        public int x;
        public int z;
        public HexCoord coordinates;
        public Vector3 position;

        private readonly HexCell[] neighbors = new HexCell[6];

        private string _terrainAssetId;
        private AssetIdentity _terrainAsset;

        private string _displayAssetId;
        private AssetIdentity _displayAsset;

        public HexCell GetNeighbor(HexDirection direction)
        {
            return neighbors[(int) direction];
        }

        public void SetNeighbor(HexDirection direction, HexCell cell)
        {
            neighbors[(int) direction] = cell;
            cell.neighbors[(int) direction.Opposite()] = this;
        }

        public void SetActive(bool active)
        {
            var assetId = MapEditor.instance.GetPrefabAssetPath(index);
            if (active)
            {
                if (assetId == null)
                {
                    if (_terrainAsset != null)
                        Assets.Recycle(_terrainAsset);

                    _terrainAsset = null;
                    _terrainAssetId = null;
                }
                else
                {
                    if (_terrainAssetId != null)
                    {
                        if (assetId != _terrainAssetId)
                        {
                            if (_terrainAsset != null)
                            {
                                Assets.Recycle(_terrainAsset);
                                _terrainAsset = null;
                            }

                            _terrainAssetId = assetId;
                            Assets.Add(assetId, this);
                        }
                    }
                    else
                    {
                        _terrainAssetId = assetId;
                        Assets.Add(assetId, this);
                    }
                }
            }
            else
            {
                if (_terrainAsset != null)
                    Assets.Recycle(_terrainAsset);

                if (_terrainAssetId != null)
                    Assets.Remove(assetId, this);

                _terrainAsset = null;
                _terrainAssetId = null;
            }
        }

        public void OnLoadAsset(AssetIdentity identity)
        {
            if (_terrainAssetId == identity.id)
            {
                _terrainAsset = identity;
                _terrainAsset.transform.SetParent(transform);
                _terrainAsset.transform.localPosition = Vector3.zero;
            }
        }
    }
}