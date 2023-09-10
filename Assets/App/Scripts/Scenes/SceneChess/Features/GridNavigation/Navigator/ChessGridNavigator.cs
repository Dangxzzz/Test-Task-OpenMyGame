using System.Collections.Generic;
using App.Scripts.Scenes.SceneChess.Features.ChessField.GridMatrix;
using App.Scripts.Scenes.SceneChess.Features.ChessField.Piece;
using App.Scripts.Scenes.SceneChess.Features.ChessField.Types;
using UnityEngine;

namespace App.Scripts.Scenes.SceneChess.Features.GridNavigation.Navigator
{
    public class ChessGridNavigator : IChessGridNavigator
    { 
        public List<Vector2Int> FindPath(ChessUnitType unit, Vector2Int from, Vector2Int to, ChessGrid grid)
     {
    if (!IsValidPosition(from, grid) || !IsValidPosition(to, grid) || from == to)
    {
        Debug.Log("Invalid position!");
        return null;
    }
    
    List<PathNode> freeSet = new List<PathNode>();
    
    HashSet<Vector2Int> closedSet = new HashSet<Vector2Int>();
    
    PathNode startPathNode = new PathNode(from, null);
    freeSet.Add(startPathNode);

    while (freeSet.Count > 0)
    {
        int lowestFScoreIndex = 0;
        for (int i = 1; i < freeSet.Count; i++)
        {
            if (freeSet[i].FullPathLenght < freeSet[lowestFScoreIndex].FullPathLenght)
            {
                lowestFScoreIndex = i;
            }
        }

        PathNode currentPathNode = freeSet[lowestFScoreIndex];

        if (currentPathNode.Position == to)
        {
            return ReversePath(currentPathNode);
        }

        freeSet.RemoveAt(lowestFScoreIndex);
        closedSet.Add(currentPathNode.Position);

        List<Vector2Int> neighbors = GetNeighbors(currentPathNode.Position, grid,unit);

        CheckNeighbors(to, grid, neighbors, closedSet, currentPathNode, freeSet);
    }
    Debug.Log("Return null");
    return null;
}

        private void CheckNeighbors( Vector2Int to, ChessGrid grid, List<Vector2Int> neighbors, HashSet<Vector2Int> closedSet,
            PathNode currentPathNode, List<PathNode> freeSet)
        {
            foreach (Vector2Int neighborPos in neighbors)
            {
                if (closedSet.Contains(neighborPos))
                {
                    continue;
                }

                int tryPathLengthFromStart = currentPathNode.PathLengthFromStart + 1;

                if (!CanMoveToPosition(neighborPos, grid))
                {
                    continue;
                }

                PathNode neighborPathNode = new PathNode(neighborPos, currentPathNode);

                bool isNewNeighbor = !freeSet.Contains(neighborPathNode);

                if (isNewNeighbor || tryPathLengthFromStart < neighborPathNode.PathLengthFromStart)
                {
                    neighborPathNode.PathLengthFromStart = tryPathLengthFromStart;
                    neighborPathNode.HeuristicEstimatePathLength = CalculateHeuristicEstimatePathLength(neighborPos, to);
                    neighborPathNode.FullPathLenght = neighborPathNode.PathLengthFromStart + neighborPathNode.HeuristicEstimatePathLength;

                    if (isNewNeighbor)
                    {
                        freeSet.Add(neighborPathNode);
                    }
                }
            }
        }

        private List<Vector2Int> ReversePath(PathNode endPathNode)
        {
            List<Vector2Int> path = new List<Vector2Int>();
            PathNode currentPathNode = endPathNode;

            while (currentPathNode != null)
            {
                path.Add(currentPathNode.Position);
                currentPathNode = currentPathNode.PrevPathNode;
            }

            path.Reverse();
            return path;
        }
        
        private int CalculateHeuristicEstimatePathLength(Vector2Int current, Vector2Int goal)
        {
            return Mathf.Abs(current.x - goal.x) + Mathf.Abs(current.y - goal.y);
        }
        
        private bool CanMoveToPosition( Vector2Int targetPosition, ChessGrid grid)
        {
            ChessUnit targetPiece = grid.Get(targetPosition);
            if (!IsValidPosition(targetPosition, grid)||targetPiece != null)
            {
                return false;
            }
            return true;
        }
        
        private bool IsValidPosition(Vector2Int position, ChessGrid grid)
        {
            return position.x >= 0 && position.x < grid.Size.x && position.y >= 0 && position.y < grid.Size.y;
        }


        private List<Vector2Int> GetNeighbors(Vector2Int position, ChessGrid grid, ChessUnitType unit)
        {
            List<Vector2Int> neighbors = new List<Vector2Int>();

            switch (unit)
            {
                case ChessUnitType.Pon:
                    GetWhitePawnNeighbors(position, grid, neighbors);
                    //GetBlackPawnNeighbors(position,grid, neighbors); can be used if we add colors for figures
                    break;
                case ChessUnitType.King:
                    GetKingNeighbors(position,grid,neighbors);
                    break;
                case ChessUnitType.Queen:
                    GetQueenNeighbors(position, grid, neighbors);
                    break;
                case ChessUnitType.Rook:
                    GetRookNeighbors(position,grid,neighbors);
                    break;
                case ChessUnitType.Knight:
                    GetKnightNeighbors(position,grid,neighbors);
                    break;
                case ChessUnitType.Bishop:
                    GetBishopNeighbors(position,grid,neighbors);
                    break;
            }
            
            return neighbors;
        }

        private void FindNeighbords(Vector2Int position, ChessGrid grid, List<Vector2Int> neighbors, Vector2Int[] figureMoves)
        {
            foreach (var move in figureMoves)
            {
                Vector2Int neighborPosition = position + move;

                if (IsValidPosition(neighborPosition, grid))
                {
                    neighbors.Add(neighborPosition);
                }
            }
        }
        
        private void GetKnightNeighbors(Vector2Int position, ChessGrid grid, List<Vector2Int> neighbors)
        {
            Vector2Int[] knightMoves =
            {
                new Vector2Int(1, 2),
                new Vector2Int(2, 1),
                new Vector2Int(2, -1),
                new Vector2Int(1, -2),
                new Vector2Int(-1, -2),
                new Vector2Int(-2, -1),
                new Vector2Int(-2, 1),
                new Vector2Int(-1, 2)
            };

            FindNeighbords(position, grid, neighbors, knightMoves);
        }
        

        private void GetKingNeighbors(Vector2Int position, ChessGrid grid, List<Vector2Int> neighbors)
        {
            Vector2Int[] kingMoves =
            {
                new Vector2Int(0, -1),  
                new Vector2Int(0, 1),   
                new Vector2Int(-1, 0),  
                new Vector2Int(1, 0),   
                new Vector2Int(-1, -1), 
                new Vector2Int(1, -1),  
                new Vector2Int(-1, 1),  
                new Vector2Int(1, 1)    
            };

            FindNeighbords(position, grid, neighbors, kingMoves);
        }
        
        private void GetRookNeighbors(Vector2Int position, ChessGrid grid, List<Vector2Int> neighbors)
        {
            Vector2Int[] rookMoves =
            {
                new Vector2Int(0, -1),  
                new Vector2Int(0, 1),   
                new Vector2Int(-1, 0),  
                new Vector2Int(1, 0)   
            };

            FindNeighbords(position, grid, neighbors, rookMoves);
        }
        
        private void GetWhitePawnNeighbors(Vector2Int position, ChessGrid grid, List<Vector2Int> neighbors)
        {
            Vector2Int topNeighbor = new Vector2Int(position.x, position.y + 1);
            if (IsValidPosition(topNeighbor, grid))
            {
                neighbors.Add(topNeighbor);
            }
        }
        
        private void GetBlackPawnNeighbors(Vector2Int position, ChessGrid grid, List<Vector2Int> neighbors)
        {
            Vector2Int topNeighbor = new Vector2Int(position.x, position.y -1);
            if (IsValidPosition(topNeighbor, grid))
            {
                neighbors.Add(topNeighbor);
            }
        }

        private void GetBishopNeighbors(Vector2Int position, ChessGrid grid, List<Vector2Int> neighbors)
        {
            Vector2Int[] bishopMoves =
            {
                new Vector2Int(-1, -1), 
                new Vector2Int(1, -1),  
                new Vector2Int(-1, 1),  
                new Vector2Int(1, 1)   
            };
            FindNeighbords(position, grid, neighbors, bishopMoves);
        }

        
        private void GetQueenNeighbors(Vector2Int position, ChessGrid grid, List<Vector2Int> neighbors)
        {
            Vector2Int[] queenMoves =
            {
                new Vector2Int(-1, -1),
                new Vector2Int(0, -1),
                new Vector2Int(1, -1),
                new Vector2Int(-1, 0),
                new Vector2Int(1, 0),
                new Vector2Int(-1, 1),
                new Vector2Int(0, 1),
                new Vector2Int(1, 1)
            };
            FindNeighbords(position, grid, neighbors, queenMoves);
        }
    }
}


