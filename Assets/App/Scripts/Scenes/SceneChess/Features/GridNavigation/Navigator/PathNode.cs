using UnityEngine;

namespace App.Scripts.Scenes.SceneChess.Features.GridNavigation.Navigator
{
    public class PathNode
    {
        public Vector2Int Position { get; }
        public PathNode Parent { get; }
        public int MoveCount { get; }


        public PathNode(Vector2Int position, PathNode parent, int moveCount)
        {
            Position = position;
            Parent = parent;
            MoveCount = moveCount;
        }
    }
}