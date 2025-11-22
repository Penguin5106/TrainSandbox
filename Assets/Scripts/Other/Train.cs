using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Train
{
    Vector2Int position;

    private List<Station> timetable { get; set; }

    private List<Vector2Int> TemporaryPath;
    private List<Vector2Int> path;

    private int pathPosition;

    public Train(int xPos, int yPos)
    {
        position = new Vector2Int(xPos, yPos);
    }

    public void move()
    {
        pathPosition++;

        position =  new Vector2Int(path[pathPosition].x, path[pathPosition].y);
    }

    double optimisedMagnitude(Vector2 vec)
    {
        vec = new Vector2(MathF.Abs(vec.x), Mathf.Abs(vec.y));

        double diagonalSteps = MathF.Min(vec.x, vec.y);
        double diagonalLength = diagonalSteps * 1.4;

        double totalLength = diagonalLength + (MathF.Max(vec.x, vec.y) - diagonalSteps);
        
        return totalLength;
    }

    private List<Vector2Int> pathfind(Tile[,] railGrid, Vector2Int start, Vector2Int goal)
    {
        // TODO write error handling for if start and goal are not rail or station and if they are unpathable

        List<Vector2Int> shortestPath = new List<Vector2Int>();
        shortestPath.Add(start);
        
        Vector2[,] pathfindStats = new Vector2[railGrid.GetLength(0), railGrid.GetLength(1)];

        //implement A* algorithm here

        pathfindStats[start[0], start[1]] = new Vector2(0, (float)optimisedMagnitude(start - goal));

        bool pathFound = false;

        while (!pathFound)
        {
            Vector2Int ClosestPoint = shortestPath[shortestPath.Count-1];

            List<Vector2Int> tilesToCheck = new List<Vector2Int>();

            int connectionsIndex = 0;
            while (connectionsIndex < railGrid[ClosestPoint[0], ClosestPoint[1]].connections.Length)
            {
                if (railGrid[ClosestPoint[0], ClosestPoint[1]].connections[connectionsIndex])
                {
                    switch(connectionsIndex)
                    {
                        case (int)Directions.Up:

                            if (ClosestPoint[0]+1 < railGrid.GetLength(0) && railGrid[ClosestPoint.y+1, ClosestPoint.x].connections[(int)Directions.Down])
                            { tilesToCheck.Add(new Vector2Int(ClosestPoint.y + 1, ClosestPoint.x)); }

                            break;

                        case (int)Directions.Down:

                            if (ClosestPoint[0] - 1 > 0 && railGrid[ClosestPoint.y - 1, ClosestPoint.x].connections[(int)Directions.Up])
                            { tilesToCheck.Add(new Vector2Int(ClosestPoint.y + -1, ClosestPoint.x)); }

                            break;
                        
                        case (int)Directions.Left:

                            if (ClosestPoint[1] - 1 > 0 && railGrid[ClosestPoint.y, ClosestPoint.x - 1].connections[(int)Directions.Right])
                            { tilesToCheck.Add(new Vector2Int(ClosestPoint.y, ClosestPoint.x - 1)); }

                            break;
                        
                        case (int)Directions.Right:

                            if (ClosestPoint[1] + 1 < railGrid.GetLength(1) && railGrid[ClosestPoint.y, ClosestPoint.x + 1].connections[(int)Directions.Left])
                            { tilesToCheck.Add(new Vector2Int(ClosestPoint.y, ClosestPoint.x + 1)); }

                            break;

                        case (int)Directions.UpLeft:
                            break;
                        case (int)Directions.UpRight:
                            break;
                        case (int)Directions.DownLeft:
                            break;
                        case (int)Directions.DownRight:
                            break;
                        default:
                            break;
                    }
                }


                connectionsIndex++;
            }

        }
        
        return shortestPath;
    }

    private void generatePath()
    {
        Tile[,] grid = GameEngine.GetInstance().railGrid;

        Vector2 pathfindStart = position;
        
        path.Clear();

        int iteration = 0;
        
        foreach (Station station in timetable)
        {
            Vector2 pathfindGoal = station.gridPosition;

            if (iteration == 0)
            {
                TemporaryPath = pathfind(grid, pathfindStart, pathfindGoal);
            }
            else
            {
                path.Concat(pathfind(grid, pathfindStart, pathfindGoal));
            }
            
            iteration++;
            pathfindStart = pathfindGoal;
        }
    }

    public void SetNewTimetable(List<Station> stations)
    {
        timetable = stations;
        
        generatePath();
    }
}
