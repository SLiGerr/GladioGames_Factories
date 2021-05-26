using System.Collections.Generic;
using Source.Code.Map;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Source.Code.Cells
{
    public class Producer : Cell, IBuilding
    {
        public float ReproductionTime;
        public int ReproductionAmount;
        private float _timer;
        private ProductVisualization visualizer;

        public Queue<Consumer> DeliveryOrdersAwaiting = new Queue<Consumer>();
        
        public Producer(float reproductionTime, int reproductionAmount, Vector2Int pos, Tile tile) : base(pos, tile)
        {
            ReproductionTime = reproductionTime;
            ReproductionAmount = reproductionAmount;
            
            Initialization();
        }
        
        public int Production { get; set; }

        public void Initialization()
        {
            _timer = ReproductionTime;
        }
        
        public void AddProduction(int value)
        {
            Production += value;

            if (!visualizer)
                visualizer = CreateNewVisualizer();
            else visualizer.gameObject.SetActive(true);
            visualizer.UpdateValue(Production);

            if (DeliveryOrdersAwaiting.Count > 0 && Production > 0) 
                DeliveryOrdersAwaiting.Dequeue().SendCarToTakeOrder();
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

        public void FrameTick()
        {
            _timer -= Time.deltaTime;
            if (_timer <= 0)
            {
                _timer = ReproductionTime;
                AddProduction(ReproductionAmount);
            }
        }
    }
}