using System.Collections.Generic;
using System.Linq;
using Source.Code.Cells;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

namespace Source.Code.Map
{
    public class MapInstaller : MonoBehaviour
    {
        [SerializeField] private GameObject numberVisPrefab;
        [Space]
        [SerializeField, Min(1)] private int producersAmount = 1;
        [SerializeField, Min(1)] private int consumersAmount = 1;
        [Space]
        [SerializeField, Min(0.1f)] private float reproductionTime = 1f;
        [SerializeField, Min(1)] private int reproductionAmount = 1;
        [Space]
        [SerializeField] private Tilemap tileMap;
        [Header("Field minimum size is 10x10")]
        [SerializeField] private Vector2Int fieldSize = new Vector2Int(256, 256);

        [Space] 
        [SerializeField] private Tile blankTile;
        [SerializeField] private Tile consumerTile;
        [SerializeField] private Tile producerTile;
        [SerializeField] private Tile roadTile;

        private void Start()
        {
            MapHandler.Tilemap = tileMap;
            MapHandler.ValuePrefab = numberVisPrefab;
            MinimumException();

            CreateBlankMap();
            
            CreateProducers();
            CreateConsumers();

            CreateConsumerProducerPath();

            PopulateWorld();
        }

        private void MinimumException()
        {
            if (fieldSize.x < 10) fieldSize.x = 10;
            if (fieldSize.y < 10) fieldSize.y = 10;
        }

        private void PopulateWorld()
        {
            foreach (var cellPair in MapHandler.Content)
            {
                cellPair.Value.Populate();
            }
        }

        private void Update()
        {
            MapHandler.Producers.ForEach(o => o.FrameTick());
            MapHandler.Consumers.ForEach(o => o.FrameTick());
        }

        private void CreateBlankMap()
        {
            var cells = new Dictionary<Vector2Int, Cell>();
            for (var x = 0; x < fieldSize.x; x++)
            {
                for (var y = 0; y < fieldSize.y; y++)
                {
                    var pos = new Vector2Int(x, y);
                    cells.Add(pos, CreateBlankCell(pos));
                }
            }

            MapHandler.Content = cells;
        }
        
        private List<KeyValuePair<Vector2Int, Cell>> BlankCells => 
            MapHandler.Content.Where(c => c.Value.GetType() == typeof(BlankField)).ToList();

        private Cell GetRandomBlankCell()
        {
            var availablePlace = BlankCells;
            return availablePlace[Random.Range(0, availablePlace.Count)].Value;
        }

        private void CreateConsumers()
        {
            MapHandler.Consumers = new List<Consumer>();
            for (var i = 0; i < consumersAmount; i++)
            {
                var randomPlace = GetRandomBlankCell();
                var cell = CreateConsumerCell(randomPlace.mapPos);
                UpdateMapCell(cell);
                MapHandler.Consumers.Add(cell as Consumer);
            }
        }

        private void CreateProducers()
        {
            MapHandler.Producers = new List<Producer>();
            for (var i = 0; i < producersAmount; i++)
            {
                var randomPlace = GetRandomBlankCell();
                var cell = CreateProducerCell(randomPlace.mapPos);
                UpdateMapCell(cell);
                MapHandler.Producers.Add(cell as Producer);
            }
        }

        private void CreateConsumerProducerPath()
        {
            foreach (var consumer in MapHandler.Consumers)
            {
                foreach (var producer in MapHandler.Producers)
                {
                    var road = MakeRoad(consumer, producer);
                    if (consumer.FavouriteProducer == producer) consumer.PathToProducer = road;
                }
            }
        }

        private List<Vector2Int> MakeRoad(Cell first, Cell second)
        {
            var path = EasyRoadBuilder(first, second);
            path.ForEach(InstallRoadCell);
            return path;
        }

        private List<Vector2Int> EasyRoadBuilder(Cell first, Cell second)
        {
            var path = new List<Vector2Int>();
            
            var midPos = new Vector2Int(first.mapPos.x, second.mapPos.y);
            path.Add(midPos);

            var xSmallest = midPos.x < second.mapPos.x ? midPos : second.mapPos;
            var xBiggest = midPos.x >= second.mapPos.x ? midPos : second.mapPos;
            
            for (var x = xSmallest.x; x < xBiggest.x; x++)
            {
                var pos = new Vector2Int(x, xBiggest.y);
                path.Add(pos);
            }
            
            var ySmallest = midPos.y < first.mapPos.y ? midPos : first.mapPos;
            var yBiggest = midPos.y >= first.mapPos.y ? midPos : first.mapPos;
            
            for (var y = ySmallest.y; y < yBiggest.y; y++)
            {
                var pos = new Vector2Int(yBiggest.x, y);
                path.Add(pos);
            }

            path.Sort((v1, v2) => (v1 - first.mapPos).sqrMagnitude.CompareTo((v2 - first.mapPos).sqrMagnitude));
            
            return path;
        }

        private void UpdateMapCell(Cell cell)
        {
            MapHandler.Content[cell.mapPos] = cell;
        }

        private void InstallRoadCell(Vector2Int pos)
        {
            if (MapHandler.Content[pos].GetType() == typeof(BlankField))
                UpdateMapCell(CreateRoadCell(pos));
        }

        private Cell CreateBlankCell(Vector2Int pos) => MapFactory.CreateEmpty(pos, blankTile);
        private Cell CreateRoadCell(Vector2Int pos) => MapFactory.CreateRoad(pos, roadTile);
        private Cell CreateConsumerCell(Vector2Int pos) => MapFactory.CreateConsumer(pos, consumerTile);
        private Cell CreateProducerCell(Vector2Int pos) => 
            MapFactory.CreateProducer(reproductionTime, reproductionAmount, pos, producerTile);
    }
}