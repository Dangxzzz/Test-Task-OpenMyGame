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
                return null;
            }
            
            List<PathNode> openSet = new List<PathNode>();
            
            HashSet<Vector2Int> closedSet = new HashSet<Vector2Int>();
            
            PathNode startNode = new PathNode(from, null, 0);
            openSet.Add(startNode);
            while (openSet.Count > 0)
            {
                int lowestMoveCountIndex = 0;

                for (int i = 1; i < openSet.Count; i++)
                {
                    if (openSet[i].MoveCount < openSet[lowestMoveCountIndex].MoveCount)
                    {
                        lowestMoveCountIndex = i;
                    }
                }

                PathNode currentNode = openSet[lowestMoveCountIndex];

                if (currentNode.Position == to)
                {
                    return ReversePath(currentNode);
                }

                openSet.RemoveAt(lowestMoveCountIndex);
                closedSet.Add(currentNode.Position);

                var neighbors = FindNeighbors(unit, to, grid, currentNode);

                CheckPaths(neighbors, closedSet, currentNode, openSet);
            }

            return null; 
        }

private static void CheckPaths(List<Vector2Int> neighbors, HashSet<Vector2Int> closedSet, PathNode currentNode, List<PathNode> openSet)
{
    foreach (Vector2Int neighborPos in neighbors)
    {
        if (closedSet.Contains(neighborPos))
        {
            continue;
        }

        int newMoveCount = currentNode.MoveCount + 1;

        PathNode neighborNode = new PathNode(neighborPos, currentNode, newMoveCount);

        bool isNewNeighbor = !openSet.Contains(neighborNode);

        if (isNewNeighbor)
        {
            openSet.Add(neighborNode);
        }
        else
        {
            int existingNeighborIndex = openSet.FindIndex(node => node.Position == neighborNode.Position);
            if (newMoveCount < openSet[existingNeighborIndex].MoveCount)
            {
                openSet[existingNeighborIndex] = neighborNode;
            }
        }
    }
}

private List<Vector2Int> FindNeighbors(ChessUnitType unit, Vector2Int to, ChessGrid grid, PathNode currentNode)
       {
           List<Vector2Int> neighbors = new List<Vector2Int>();

          switch (unit)
         {
        case ChessUnitType.Queen:
            neighbors = GetQueenNeighbors(currentNode.Position, grid);
            break;
        case ChessUnitType.Bishop:
            neighbors = GetBishopNeighbors(currentNode.Position, grid);
            break;
        case ChessUnitType.Rook:
            neighbors = GetRookNeighbors(currentNode.Position, grid);
            break;
        case ChessUnitType.Knight:
            neighbors = GetKnightNeighbors(currentNode.Position, grid);
            break;
        case ChessUnitType.King:
            neighbors = GetKingNeighbors(currentNode.Position, to, grid);
            break;
        case ChessUnitType.Pon:
            neighbors = GetPawnNeighbors(currentNode.Position, grid);
            break;
    }

    return neighbors;
}


private List<Vector2Int> ReversePath(PathNode endNode)
        {
            List<Vector2Int> path = new List<Vector2Int>();
            PathNode currentNode = endNode;

            while (currentNode != null)
            {
                path.Add(currentNode.Position);
                currentNode = currentNode.Parent;
            }

            path.Reverse();
            return path;
        }

        private bool IsValidPosition(Vector2Int position, ChessGrid grid)
        {
            return position.x >= 0 && position.x < grid.Size.x && position.y >= 0 && position.y < grid.Size.y;
        }

        private void CheckNeighbords(Vector2Int position, ChessGrid grid, Vector2Int[] directions, List<Vector2Int> neighbors)
        {
            foreach (var direction in directions)
            {
                Vector2Int neighborPosition = position + direction;

                while (IsValidPosition(neighborPosition, grid))
                {
                    ChessUnit targetPiece = grid.Get(neighborPosition);
                    if (targetPiece != null)
                    {
                        break;
                    }

                    neighbors.Add(neighborPosition);
                    neighborPosition += direction;
                }
            }
        }
        
        private List<Vector2Int> GetQueenNeighbors(Vector2Int position, ChessGrid grid)
        {
            List<Vector2Int> neighbors = new List<Vector2Int>();
            Vector2Int[] directions =
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
            CheckNeighbords(position, grid, directions, neighbors);
            return neighbors;
        }
        
        private List<Vector2Int> GetBishopNeighbors(Vector2Int position, ChessGrid grid)
        {
            List<Vector2Int> neighbors = new List<Vector2Int>();
            Vector2Int[] directions =
            {
                new Vector2Int(-1, -1), 
                new Vector2Int(1, -1),  
                new Vector2Int(-1, 1),  
                new Vector2Int(1, 1)   
            };
            CheckNeighbords(position, grid, directions, neighbors);

            return neighbors;
        }
        
        private List<Vector2Int> GetRookNeighbors(Vector2Int position, ChessGrid grid)
        {
            List<Vector2Int> neighbors = new List<Vector2Int>();
            Vector2Int[] directions =
            {
                new Vector2Int(0, -1), 
                new Vector2Int(-1, 0),  
                new Vector2Int(1, 0),   
                new Vector2Int(0, 1),   

            };
            CheckNeighbords(position, grid, directions, neighbors);
            return neighbors;
        }
        
        private List<Vector2Int> GetKnightNeighbors(Vector2Int position, ChessGrid grid)
        {
            List<Vector2Int> neighbors = new List<Vector2Int>();
            
            Vector2Int[] directions =
            {
                new Vector2Int(-1, -2),
                new Vector2Int(1, -2),
                new Vector2Int(-2, -1),
                new Vector2Int(2, -1),
                new Vector2Int(-2, 1),
                new Vector2Int(2, 1),
                new Vector2Int(-1, 2),
                new Vector2Int(1, 2)
            };

            foreach (var direction in directions)
            {
                Vector2Int neighborPosition = position + direction;

                if (IsValidPosition(neighborPosition, grid))
                {
                    ChessUnit targetPiece = grid.Get(neighborPosition);
                    
                    if (targetPiece == null)
                    {
                        neighbors.Add(neighborPosition);
                    }
                }
            }
            return neighbors;
        }

        
        private List<Vector2Int> GetKingNeighbors(Vector2Int position, Vector2Int target, ChessGrid grid)
        {
            List<Vector2Int> neighbors = new List<Vector2Int>();
    
            Vector2Int[] directions =
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

            List<(Vector2Int, int)> neighborFullScores = new List<(Vector2Int, int)>();

            foreach (var direction in directions)
            {
                Vector2Int neighborPosition = position + direction;
    
                if (IsValidPosition(neighborPosition, grid))
                {
                    ChessUnit targetPiece = grid.Get(neighborPosition);
            
                    if (targetPiece == null)
                    {
                        int fullPathScore = Mathf.Abs(neighborPosition.x - target.x) + Mathf.Abs(neighborPosition.y - target.y);
                        int fullScores = fullPathScore;

                        neighborFullScores.Add((neighborPosition, fullScores));
                    }
                }
            }
            
            neighborFullScores.Sort((x, y) => x.Item2.CompareTo(y.Item2));
            
            foreach (var neighborFullScore in neighborFullScores)
            {
                neighbors.Add(neighborFullScore.Item1);
            }

            return neighbors;
        }

        private List<Vector2Int> GetPawnNeighbors(Vector2Int position, ChessGrid grid)
        {
            List<Vector2Int> neighbors = new List<Vector2Int>();
            Vector2Int forwardPositionUp = position + new Vector2Int(0, 1);
            Vector2Int forwardPositionDown = position + new Vector2Int(0, -1);
            if (IsValidPosition(forwardPositionUp, grid) && grid.Get(forwardPositionUp) == null)
            {
                neighbors.Add(forwardPositionUp);
            }
            if (IsValidPosition(forwardPositionDown, grid) && grid.Get(forwardPositionDown) == null)
            {
                neighbors.Add(forwardPositionDown);
            }
            return neighbors;
        }
        
    }
}