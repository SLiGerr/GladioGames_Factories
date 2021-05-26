using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Source.Code.Cells;
using Source.Code.Map;
using UnityEngine;

namespace Source.Code.Vehicles
{
    public class CarVehicle : IVehicle
    {
        public int ProductionAmount { get; set; }
        public int ProductionTransfer { get; set; }
        public float ActionDuration { get; set; }
        public Cell Start { get; set; }
        public Cell End { get; set; }
        public Cell Current { get; set; }
        public Queue<Action> Moves { get; set; }
        public VehicleBehaviour Behaviour { get; set; }

        private List<Vector2Int> _path;
        private List<Vector2Int> _fullPath;
        private int _currentCellId;

        public CarVehicle(Cell start, Cell end, List<Vector2Int> path)
        {
            ProductionAmount = 1;
            ActionDuration = 0.1f;
            _path = new List<Vector2Int>(path);
            
            Start = start;
            End = end;
            Current = start;

            InitializeCar();
            FormActionList();
            
            MoveNext();
        }

        private void InitializeCar()
        {
            Behaviour = GameObject.CreatePrimitive(PrimitiveType.Cube).AddComponent<VehicleBehaviour>();
            Behaviour.transform.position = Start.WorldPos + Vector2.up*.5f;
        }

        private void FormActionList()
        {
            _fullPath = new List<Vector2Int>();
            Moves = new Queue<Action>();
            foreach (var cell in _path)
            {
                if (cell == _path.Last())
                    Moves.Enqueue(ArrivedAtEnd);
                else Moves.Enqueue(MoveNext);
            }

            _fullPath.AddRange(_path);

            _path.Reverse();
            
            _fullPath.AddRange(_path);
            
            foreach (var cell in _path)
            {
                if(cell == _path.Last())
                    Moves.Enqueue(ArrivedBackToStart);
                else Moves.Enqueue(MoveNext);
            }
        }

        private void MoveNext()
        {
            if(_currentCellId >= _fullPath.Count)
            {
                Moves?.Dequeue();
                return;
            }
            var to = _fullPath[_currentCellId];
            Behaviour.StartCoroutine(MakeMove( MapHandler.Content[to], Moves.Dequeue()));
            _currentCellId++;
            
        }

        private IEnumerator MakeMove(Cell to, Action nextAction)
        {
            yield return new WaitForSeconds(ActionDuration);
            Behaviour.transform.position = to.WorldPos + Vector2.up*.5f;
            Current = to;
            nextAction?.Invoke();
        }

        private void ArrivedAtEnd()
        {
            if (End.GetType() != typeof(Producer)) return;
            var producer = End as Producer;
            ProductionTransfer = producer.TakeProduction(ProductionAmount);
            MoveNext();
        }

        private void ArrivedBackToStart()
        {
            if (Start.GetType() != typeof(Consumer)) return;
            var consumer = Start as Consumer;
            consumer.AddProduction(ProductionTransfer);
            MonoBehaviour.Destroy(Behaviour.gameObject);
        }
    }
}