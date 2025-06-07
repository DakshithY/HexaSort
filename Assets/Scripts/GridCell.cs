using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCell : MonoBehaviour
{
    public Hexstack Stack { get; private set; }

    public bool Isoccupied 
    { get => Stack != null;
        private set { } 
    }
    public void AssignStack(Hexstack stack) 
    {
        Stack = stack;
    }
   

    public void UpdateOccupancy()
    {
        Isoccupied = Stack.Hexagons.Count > 0;
    }

}
