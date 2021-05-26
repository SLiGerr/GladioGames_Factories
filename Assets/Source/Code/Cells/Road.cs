using Source.Code.Vehicles;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Source.Code.Cells
{
    public class Road : Cell
    {
        public IVehicle ContainedVehicle;
        
        public Road(Vector2Int pos, Tile tile) : base(pos, tile)
        {
            
        }
    }
}