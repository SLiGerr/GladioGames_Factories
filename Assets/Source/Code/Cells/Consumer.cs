using System.Collections.Generic;
using System.Linq;
using Source.Code.Map;
using Source.Code.Vehicles;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Source.Code.Cells
{
    public class Consumer : Cell, IBuilding
    {
        public List<Vector2Int> PathToProducer;
        public Producer FavouriteProducer;
        private ProductVisualization visualizer;
        
        public Consumer(Vector2Int pos, Tile tile) : base(pos, tile)
        {
            Initialization();
        }
        
        public int Production { get; set; }
        private ConsumerState _currentState;
        
        public void Initialization()
        {
            _currentState = ConsumerState.PlacingOrder;

            FavouriteProducer = MapHandler.Producers.OrderBy(o => Vector2Int.Distance(o.mapPos, mapPos)).FirstOrDefault();
        }

        public void FrameTick()
        {
            if (_currentState == ConsumerState.PlacingOrder)
            {
                PlaceOrder();
            }
        }

        public void SendCarToTakeOrder()
        {
            if(FavouriteProducer.Production == 0) return;
            SendVehicleToProducer();
            _currentState = ConsumerState.DeliveryAwaiting;
        }

        private void PlaceOrder()
        {
            if (!FavouriteProducer.DeliveryOrdersAwaiting.Contains(this))
                FavouriteProducer.DeliveryOrdersAwaiting.Enqueue(this);
            _currentState = ConsumerState.ManufacturingAwaiting;
        }

        private void SendVehicleToProducer()
        {
            new CarVehicle(this, FavouriteProducer, PathToProducer);
            FavouriteProducer.DeliveryOrdersAwaiting.Enqueue(this);
            _currentState = ConsumerState.DeliveryAwaiting;
        }

        public void AddProduction(int value)
        {
            Production += value;
            _currentState = ConsumerState.PlacingOrder;

            if (!visualizer)
                visualizer = CreateNewVisualizer();
            else visualizer.gameObject.SetActive(true);
            visualizer.UpdateValue(Production);
        }
        
        private ProductVisualization CreateNewVisualizer()
        {
            var prefab = MapHandler.ValuePrefab;
            var pos = (this as Cell).WorldPos;
            return Object.Instantiate(prefab, pos, Quaternion.identity).GetComponent<ProductVisualization>();
        }

        public int TakeProduction(int value)
        {
            Production -= value;
            
            visualizer.UpdateValue(Production);
            if(Production <= 0) visualizer.gameObject.SetActive(true);
            
            return value;
        }

        private enum ConsumerState
        {
            DeliveryAwaiting,
            ManufacturingAwaiting,
            PlacingOrder
        }
    }
}