using Source.Code.Cells;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Source.Code
{
    public static class MapFactory
    {
        public static Cell CreateConsumer(Vector2Int pos, Tile tile)
        {
            return new Consumer(pos, tile);
        }

        public static Cell CreateProducer(float reproductionTime, int reproductionAmount, Vector2Int pos, Tile tile)
        {
            return new Producer(reproductionTime, reproductionAmount, pos, tile);
        }
        
        public static Cell CreateEmpty(Vector2Int pos, Tile tile)
        {
            return new BlankField(pos, tile);
        }
        
        public static Cell CreateRoad(Vector2Int pos, Tile tile)
        {
            return new BlankField(pos, tile);
        }
    }
}