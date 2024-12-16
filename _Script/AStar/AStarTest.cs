using Farm.AStar;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
//*****************************************
//Creator: SamLee 
//Description: 
//***************************************** 
namespace Farm.AStar{
    [RequireComponent(typeof(AStar))]
    public class AStarTest : MonoBehaviour
    {
        private AStar aStar;
        public Vector2Int startCoordinate;
        public Vector2Int targetCoordinate;
        public Tilemap displayTileMap;
        public TileBase displayTile;
        public bool displayStartAndTarget;
        public bool displayPath;
        public bool moveNpc;
        private Stack<MovementStep> npcMovementStepStack;

        [SceneName] string targetScene;
        private AnimationClip stopAnimationClip;

        [SerializeField]private NPCMovement npcMovement;

        [SerializeField]ScheduleDetails scheduleDetails;

        private void Update()
        {
            ShowPathOnGridMap();

            if (moveNpc)
            {
                moveNpc = false;
                //var schedule = new ScheduleDetails(0, 0, 0, 0, Season.Spring, targetScene, targetCoordinate, stopAnimationClip, true);
                npcMovement.BuiltPath(scheduleDetails);
            }
        }

        private void Awake()
        {
            aStar = GetComponent<AStar>();
            npcMovementStepStack = new Stack<MovementStep>();
        }

        private void ShowPathOnGridMap()
        {
            if (displayTileMap != null && displayTile != null)
            {
                if (displayStartAndTarget)
                {
                    displayTileMap.SetTile((Vector3Int)startCoordinate, displayTile);
                    displayTileMap.SetTile((Vector3Int)targetCoordinate, displayTile);
                }
                else
                {
                    displayTileMap.SetTile((Vector3Int)startCoordinate, null);
                    displayTileMap.SetTile((Vector3Int)targetCoordinate, null);
                }

                if (displayPath)
                {
                    string sceneName = SceneLoadManager.Instance.currentScene.sceneName;
                    aStar.BuildPath(sceneName,startCoordinate,targetCoordinate,npcMovementStepStack);
                    foreach (var step in npcMovementStepStack)
                    {
                        displayTileMap.SetTile((Vector3Int)step.gridCoordinate, displayTile);
                    }
                }
                else
                {
                    if (npcMovementStepStack.Count > 0)
                    {
                        foreach (var step in npcMovementStepStack)
                        {
                            displayTileMap.SetTile((Vector3Int)step.gridCoordinate, null);
                        }
                        npcMovementStepStack.Clear();
                    }
                }
            }
        }
    }
}