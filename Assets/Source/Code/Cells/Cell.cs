using System;
using Source.Code.Map;
using UnityEngine;
using UnityEngine.Tilemaps;
using Object = UnityEngine.Object;

namespace Source.Code.Cells
{
    [Serializable]
    public class Cell
    {
        public Cell(Vector2Int pos, Tile tile)
        {
            mapPos = pos;
            refTile = Object.Instantiate(tile);
        }

        public void Populate()
        {
            MapHandler.SetTile(this);
        }
        
        public Vector2Int mapPos;
        public Tile refTile;
        
        public Vector2 WorldPos => new Vector2(mapPos.x - mapPos.y, (mapPos.y + mapPos.x) / 2f);
    }
}
