using Farm.AStar;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//Creator: SamLee 
//Description: 
//***************************************** 
public class GridNodesSet
{
    private int width;
    private int height;
    private Node[,] gridNodes;


    public GridNodesSet(int width, int height)
    {
        this.width = width;
        this.height = height;
        gridNodes = new Node[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                gridNodes[x, y] = new Node(new Vector2Int(x, y));
            }
        }
    }
    public Node GetGridNode(int indexX, int indexY)
    {
        if (indexX < width && indexY < height && indexX>=0 && indexY >= 0)
        {
            return gridNodes[indexX, indexY];
        }

        Debug.Log("Given grid node coordinate has acceeded the range.");
        return null;
    }
}
