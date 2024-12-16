using Farm.InventoryNamespace;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
//*****************************************
//Creator: SamLee 
//Description: 
//***************************************** 
namespace GridMapNamespace{
    [ExecuteInEditMode]
    public class GridMap : MonoBehaviour
    {

        public MapDataSO mapData;
        public GridType gridType;
        private Tilemap currentTileMap;

        private void OnEnable()
        {
            if (!Application.IsPlaying(this))
            {
                currentTileMap = GetComponent<Tilemap>();

                if (mapData != null)
                {
                    mapData.tileProperties.Clear();
                }
            }
        }

        private void OnDisable()
        {
            if (!Application.IsPlaying(this))
            {
                currentTileMap = GetComponent<Tilemap>();
                UpdateTileProperties();
#if UNITY_EDITOR
                if (mapData != null)
                {
                    EditorUtility.SetDirty(mapData);
                }
#endif
            }
        }

        private void UpdateTileProperties()
        {
            currentTileMap.CompressBounds();
            if (!Application.IsPlaying(this))
            {
                if (mapData != null)
                {
                    Vector3Int startPos = currentTileMap.cellBounds.min;
                    Vector3Int endPos = currentTileMap.cellBounds.max;
                    for (int i = startPos.x; i < endPos.x; i++)
                    {
                        for (int j = startPos.y; j < endPos.y; j++)
                        {
                            TileBase tile = currentTileMap.GetTile(new Vector3Int(i, j, 0));
                            if (tile != null)
                            {
                                TileProperty newTile = new TileProperty()
                                {
                                    tileCoordinate = new Vector2Int(i, j),
                                    gridType = this.gridType,
                                    boolTypeValue = true,
                                };
                                mapData.tileProperties.Add(newTile);
                            }
                        }
                    }
                }
            }
        }
    }
}