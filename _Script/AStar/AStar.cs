using Farm.AStar;
using GridMapNamespace;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
//*****************************************
//Creator: SamLee 
//Description: 
//***************************************** 
namespace Farm.AStar{
    public class AStar : Singleton<AStar>
    {
        private GridNodesSet gridNodesSet;
        private Node startNode;
        private Node targetNode;
        private int gridWidth;
        private int gridHeight;
        private int originX;
        private int originY;
        private List<Node> openNodeList;
        private HashSet<Node> closedNodeList;
        private bool isPathFound;

        public void BuildPath(string sceneName, Vector2Int startCoordinate, Vector2Int targetCoordinate, Stack<MovementStep> npcMovementStep)
        {
            isPathFound = false;

            if (GenerateGridNodes(sceneName, startCoordinate, targetCoordinate))
            {
                if (FindShortestPath())
                {
                    UpdatePathOnMovementStepStack(sceneName, npcMovementStep);
                }
            }
        }

        private bool GenerateGridNodes(string sceneName, Vector2Int startCoordinate, Vector2Int targetCoordinate)
        {
            if (GridMapManager.Instance.GetGridDimensions(sceneName, out Vector2Int gridDimensions, out Vector2Int gridOrigin))
            {
                gridNodesSet = new GridNodesSet(gridDimensions.x, gridDimensions.y);
                gridWidth = gridDimensions.x;
                gridHeight = gridDimensions.y;
                originX = gridOrigin.x;
                originY = gridOrigin.y;
                openNodeList = new List<Node>();
                closedNodeList = new HashSet<Node>();
            }
            else
            {
                return false;
            }
            
            startNode = gridNodesSet.GetGridNode(startCoordinate.x - originX, startCoordinate.y - originY);
            targetNode = gridNodesSet.GetGridNode(targetCoordinate.x - originX, targetCoordinate.y - originY);

            for (int xIndex = 0; xIndex < gridWidth; xIndex++)
            {
                for (int yIndex = 0; yIndex < gridHeight; yIndex++)
                {
                    Vector3Int tileCoordinate = new Vector3Int(xIndex + originX, yIndex + originY, 0);
                    string key = GridMapManager.GetTileDetailDictKey(sceneName, tileCoordinate.x, tileCoordinate.y);
                    Node node = gridNodesSet.GetGridNode(xIndex, yIndex);

                    if (GridMapManager.Instance.tileDetailsDict.ContainsKey(key))
                    {
                        TileDetails tileDetails = GridMapManager.Instance.tileDetailsDict[key];
                        if (tileDetails.isNpcObstacle)
                        {
                            node.isObstacle = true;
                        }
                    }
                }
            }

            return true;
            
        }

        private bool FindShortestPath()
        {
            openNodeList.Add(startNode);

            while (openNodeList.Count > 0)
            {
                openNodeList.Sort();
                Node closeNode = openNodeList[0];
                openNodeList.RemoveAt(0);
                closedNodeList.Add(closeNode);

                if (closeNode == targetNode)
                {
                    isPathFound = true;
                    break;
                }
                else
                {
                    AddNeighborNodesToOpenNodeList(closeNode);
                }
            }
            return isPathFound;
        }

        private void AddNeighborNodesToOpenNodeList(Node currentNode)
        {
            Vector2Int currentNodeIndices = currentNode.gridIndices;
            Node validNeighborNode;
            for (int offsetX = -1; offsetX <= 1; offsetX++)
            {
                for (int offsetY = -1; offsetY <= 1; offsetY++)
                {
                    if (offsetX == 0 && offsetY == 0)
                        continue;

                    validNeighborNode = GetValidNeighborNode(currentNodeIndices.x + offsetX, currentNodeIndices.y + offsetY);
                    
                    if (validNeighborNode != null)
                    {
                        validNeighborNode.gCost = currentNode.gCost + CalculateNodeDistance(currentNode, validNeighborNode);
                        validNeighborNode.hCost = CalculateNodeDistance(currentNode, targetNode);
                        validNeighborNode.parentNode = currentNode;
                        openNodeList.Add(validNeighborNode);
                    }
                }
            }
        }

        private Node GetValidNeighborNode(int NeighborXIndex, int NeighborYIndex)
        {
            if (NeighborXIndex >= gridWidth || NeighborYIndex >= gridHeight || NeighborXIndex<0 || NeighborYIndex < 0)
            {
                return null;
            }

            Node neighborNode = gridNodesSet.GetGridNode(NeighborXIndex, NeighborYIndex);

            if (neighborNode.isObstacle || closedNodeList.Contains(neighborNode) || openNodeList.Contains(neighborNode))
                return null;
            else
                return neighborNode;
        }

        private int CalculateNodeDistance(Node nodeA, Node nodeB)
        {
            int xDistance = Mathf.Abs(nodeA.gridIndices.x - nodeB.gridIndices.x);
            int yDistance = Mathf.Abs(nodeA.gridIndices.y - nodeB.gridIndices.y);

            if (xDistance > yDistance)
            {
                return 14 * yDistance + 10 * (xDistance - yDistance);
            }
            else
            {
                return 14 * xDistance + 10 * (yDistance - xDistance);
            }
        }

        private void UpdatePathOnMovementStepStack(string sceneName, Stack<MovementStep> npcMovementStep)
        {
            Node nextNode = targetNode;

            while (nextNode != null)
            {
                MovementStep newStep = new MovementStep();
                newStep.sceneName = sceneName;
                newStep.gridCoordinate = new Vector2Int(nextNode.gridIndices.x + originX, nextNode.gridIndices.y + originY);
                npcMovementStep.Push(newStep);
                nextNode = nextNode.parentNode;
            }
        }
    }
}