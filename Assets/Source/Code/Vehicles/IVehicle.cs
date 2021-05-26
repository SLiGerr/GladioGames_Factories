using System;
using System.Collections.Generic;
using Source.Code.Cells;
using UnityEngine;

namespace Source.Code.Vehicles
{
    public interface IVehicle
    {
        int ProductionAmount { get; set; }
        int ProductionTransfer { get; set; }
        float ActionDuration { get; set; }
        Cell Start { get; set; }
        Cell End { get; set; }
        Cell Current { get; set; }
        Queue<Action> Moves { get; set; } 
        VehicleBehaviour Behaviour { get; set; }
    }

    public class VehicleBehaviour : MonoBehaviour{}
}