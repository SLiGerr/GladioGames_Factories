using System.Collections.Generic;
using Source.Code.Cells;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Source.Code.Map
{
    public static class MapHandler
    {
        public static Dictionary<Vector2Int, Cell> Content;
        public static List<Consumer> Consumers;
        public static List<Producer> Producers;
        public static Tilemap Tilemap;

        public static GameObject ValuePrefab;
        
        public static void SetTile(Cell cell)
        {
            var pos = new Vector3Int(cell.mapPos.x, cell.mapPos.y, 0);
            Tilemap.SetTile(pos, cell.refTile);
        }
    }
}
