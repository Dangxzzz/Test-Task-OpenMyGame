using UnityEngine;

namespace App.Scripts.Scenes.SceneChess.Features.GridNavigation.Navigator
{
    public class PathNode
    {
        public Vector2Int Position { get; }
        public PathNode Parent { get; set; }
        public int MoveCount { get; set; }


        public PathNode(Vector2Int position, PathNode parent, int moveCount)
        {
            Position = position;
            Parent = parent;
            MoveCount = moveCount;
        }
    }
}