using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//Creator: SamLee 
//Description: 
//***************************************** 
namespace Farm.AStar{
    public class Node : IComparable<Node>
    {
        public Vector2Int gridIndices;
        public int gCost; //True cost from the start(it calculates based on curve route instead of a single straight line)
        public int hCost; //Heuristic cost
        public int FCost => gCost + hCost;
        public bool isObstacle = false;
        public Node parentNode;



        public Node(Vector2Int indices)
        {
            gridIndices = indices;
            parentNode = null;
        }

        public int CompareTo(Node other)
        {
            int result = FCost.CompareTo(other.FCost);
            if (result == 0)
            {
                result = hCost.CompareTo(other.hCost);
            }
            return result;
        }
    }
}