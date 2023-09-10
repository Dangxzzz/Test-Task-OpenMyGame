using UnityEngine;

namespace App.Scripts.Scenes.SceneChess.Features.GridNavigation.Navigator
{
    public class PathNode
    {
        public Vector2Int Position { get; } 
        public PathNode PrevPathNode { get;} 
        public int PathLengthFromStart { get; set; } 
        public int HeuristicEstimatePathLength { get; set; } 
        public int FullPathLenght { get; set; } 

        public PathNode(Vector2Int position, PathNode prevPathNode)
        {
            Position = position;
            PrevPathNode = prevPathNode;
            PathLengthFromStart = 0;
            HeuristicEstimatePathLength = 0;
            FullPathLenght = 0;
        }
    }
}