using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Train : MonoBehaviour , IClickable
{
    private Vector2Int position;
    private float TileOffset;
    private List<Station> timetable { get; set; }

    public List<Vector2Int> temporaryPath;
    public List<Vector2Int> path;

    private int temporaryPathPosition = 0;
    private int pathPosition = 0;

    public Train(int xPos, int yPos)
    {
        path = new List<Vector2Int>();
        TileOffset = GameEngine.GetTileOffset();
        position = new Vector2Int(xPos, yPos);
        timetable = new List<Station>();
    }

    public Train()
    {
        path = new List<Vector2Int>();
        TileOffset = GameEngine.GetTileOffset();
        position = new Vector2Int(0, 0);
        timetable = new List<Station>();
    }

    public void Start()
    {
        temporaryPath = new List<Vector2Int>();
        
    }
    
    public List<Station> GetTimetable()
    {
        return timetable;
    }

    public void setPosition(int xPos, int yPos)
    {
        position =  new Vector2Int(xPos, yPos);
        gameObject.transform.position = new  Vector3(position.y * TileOffset, position.x * TileOffset, -1);
    }
    
    public void setPosition(Vector2Int newPos)
    {
        position =  newPos;
        gameObject.transform.position = new  Vector3(position.y * TileOffset, position.x * TileOffset, -1);
    }

    public Vector2Int GetPosition()
    {
        return position;
    }

    public void move()
    {
        if (temporaryPath.Count <= 0)
        {
            if (path.Count <= 0)
            {
                return;
            }
            pathPosition++;

            setPosition(path[pathPosition]);
            return;
        }

        setPosition(temporaryPath[temporaryPathPosition]);
        temporaryPathPosition++;
        
        if (temporaryPathPosition >= temporaryPath.Count)
            temporaryPath.Clear();
    }

    double optimisedMagnitude(Vector2 vec)
    {
        vec = new Vector2(MathF.Abs(vec.x), Mathf.Abs(vec.y));

        double diagonalSteps = MathF.Min(vec.x, vec.y);
        double diagonalLength = diagonalSteps * 1.4;

        double totalLength = diagonalLength + (MathF.Max(vec.x, vec.y) - diagonalSteps);
        
        return totalLength;
    }

    float generatePathLength(Vector2 previousNodeStats, float distanceToPreviousNode)
    {
        return previousNodeStats.x + distanceToPreviousNode;
        
        
    }
    
    private List<Vector2Int> pathfind(Tile[,] railGrid, Vector2Int start, Vector2Int goal)
    {
        // TODO write error handling for if start and goal are not rail or station and if they are unpathable

        List<Vector2Int> shortestPath = new List<Vector2Int>();

        Vector2[,] pathfindStats = new Vector2[railGrid.GetLength(0), railGrid.GetLength(1)];
        
        for (int i=0;i<railGrid.GetLength(0)*railGrid.GetLength(1);i++) pathfindStats[i%railGrid.GetLength(0),i/railGrid.GetLength(0)]= new Vector2(-1, 0);
        //implement A* algorithm here

        pathfindStats[start.y, start.x] = new Vector2(0, (float)optimisedMagnitude(start - goal));

        bool pathFound = false;
        Vector2Int ClosestPoint = start;
        HashSet<Vector2Int> openSet = new HashSet<Vector2Int> { ClosestPoint };

        while (!pathFound)
        {
            
            // gather connections to (optimistic) closest point
            int connectionsIndex = 0;
            while (connectionsIndex < railGrid[ClosestPoint.y, ClosestPoint.x].connections.Length)
            {
                if (railGrid[ClosestPoint.y, ClosestPoint.x].connections[connectionsIndex])
                {
                    // if the 2 rail pieces are connected update the new ones stats for both path cost and if necessary the heuristic
                    
                    Vector2Int vectorDirection = Vector2Int.zero;
                    // set up/down
                    if (connectionsIndex <= 2)
                        vectorDirection.x = 1;
        
                    if (connectionsIndex >= 5)
                        vectorDirection.x = -1;
        
                    // set left/right
                    if (connectionsIndex == 2 || connectionsIndex == 4 || connectionsIndex == 7)
                        vectorDirection.y = 1;
        
                    if (connectionsIndex == 0 || connectionsIndex == 3 || connectionsIndex == 5)
                        vectorDirection.y = -1;

                    float distance = 1;
                    
                    if (MathF.Abs(vectorDirection.x) + MathF.Abs(vectorDirection.y) == 2)
                        distance = 1.4f;
        
                    Vector2Int nextTile = ClosestPoint +  vectorDirection;
        
                    if (0 < nextTile.x && nextTile.x < railGrid.GetLength(1) && 
                        railGrid.GetLength(0) > nextTile.y && nextTile.y > 0 && 
                        railGrid[nextTile.y, nextTile.x].connections[7 - connectionsIndex])
                    {
                        float newLength = generatePathLength(pathfindStats[ClosestPoint.y, ClosestPoint.x], distance);
                                
                        if (pathfindStats[nextTile.y, nextTile.x].x == -1 || newLength < pathfindStats[nextTile.y, nextTile.x].x)
                        {
                            pathfindStats[nextTile.y, nextTile.x].x = newLength;
                                    
                            openSet.Add(new Vector2Int(nextTile.x, nextTile.y));
                        }

                        if (pathfindStats[nextTile.y, nextTile.x].y.Equals(0))
                        {
                            pathfindStats[nextTile.y, nextTile.x].y = (float)optimisedMagnitude(goal - nextTile);
                        }
                    }
                    
                }


                connectionsIndex++;
            }
            
            openSet.Remove(ClosestPoint);
            if (openSet.Count != 0)
            {
                Vector2 ClosestPointStats = new Vector2(9999, 9999);
                foreach (Vector2Int node in openSet)
                {

                    if (pathfindStats[node.y, node.x].x + pathfindStats[node.y, node.x].y < ClosestPointStats.x + ClosestPointStats.y || 
                        (pathfindStats[node.y, node.x].x + pathfindStats[node.y, node.x].y == ClosestPointStats.x + ClosestPointStats.y &&  pathfindStats[node.y, node.x].y <= ClosestPointStats.y))
                    {
                        ClosestPoint = node;
                        ClosestPointStats = pathfindStats[node.y, node.x];
                    }
                            
                }
            }
            if (ClosestPoint == goal)
            {
                pathFound = true;
                break;
            }
            
            if (openSet.Count == 0 )
            {
                // unpathable TODO define error behaviour
                Debug.Log("path cannot be found");
                return new List<Vector2Int>();
            }

            
        }
        
        shortestPath.Add(ClosestPoint);
        
        //walkback to find path
        while (shortestPath[^1] != start)
        {
            float shortestDistance = pathfindStats[ClosestPoint.y, ClosestPoint.x].x;
            
            Vector2Int nextPathStep = ClosestPoint;
            
            int connectionIndex = 0;
            
            while (connectionIndex < railGrid[ClosestPoint.y, ClosestPoint.x].connections.Length)
            {
                if (railGrid[ClosestPoint.y, ClosestPoint.x].connections[connectionIndex])
                {
                    Vector2Int vectorDirection = Vector2Int.zero;
                    // set up/down
                    if (connectionIndex <= 2)
                        vectorDirection.x = 1;
        
                    if (connectionIndex >= 5)
                        vectorDirection.x = -1;
        
                    // set left/right
                    if (connectionIndex == 2 || connectionIndex == 4 || connectionIndex == 7)
                        vectorDirection.y = 1;
        
                    if (connectionIndex == 0 || connectionIndex == 3 || connectionIndex == 5)
                        vectorDirection.y = -1;
        
                    Vector2Int nextTile = ClosestPoint +  vectorDirection;
        
                    if (0 < nextTile.x && nextTile.x < railGrid.GetLength(1) && 
                        railGrid.GetLength(0) > nextTile.y && nextTile.y > 0 && 
                        railGrid[nextTile.y, nextTile.x].connections[7 - connectionIndex] && !pathfindStats[nextTile.y, nextTile.x].y.Equals(0))
                    {
                        if (pathfindStats[nextTile.y, nextTile.x].x < shortestDistance)
                        {
                            shortestDistance = pathfindStats[nextTile.y, nextTile.x].x;
                            nextPathStep = nextTile;
                        }
                    }
                    
                }
                connectionIndex++;
            }
            shortestPath.Add(nextPathStep);
            ClosestPoint = nextPathStep;
        }

        shortestPath.Reverse();
        
        return shortestPath;
    }

    private bool generatePath()
    {
        path.Clear();

        if (timetable.Count == 0)
            return true;
        
        Tile[,] grid = GameEngine.GetInstance().railGrid;

        Vector2Int pathfindStart = position;
        
        int iteration = 0;
        
        foreach (Station station in timetable)
        {
            Vector2Int pathfindGoal = station.gridPosition;

            if (iteration == 0)
            {
                temporaryPath = pathfind(grid, pathfindStart, pathfindGoal) ?? new List<Vector2Int>();
                temporaryPathPosition = 0;
            }
            else
            {
                path = new List<Vector2Int>(path.Concat(new List<Vector2Int>(pathfind(grid, pathfindStart, pathfindGoal))));

                if (path.Count == 0)
                {
                    return false;
                }
            }
            
            iteration++;
            pathfindStart = pathfindGoal;
        }

        pathPosition = 0;
        return true;
    }

    public bool SetNewTimetable(List<Station> stations)
    {
        timetable = stations;
        UIManager.GetInstance().SetTrainInfo(timetable);
        
        return generatePath();
    }

    public bool AddStationToTimetable(Station station)
    {
        timetable.Add(station);
        
        UIManager.GetInstance().SetTrainInfo(timetable);
        
        return generatePath();
    }

    public void Click()
    {
        UIManager ui = UIManager.GetInstance();
        ui.SetUIPanelActive(UIPanel.Train);
        ui.SetTrainInfo(timetable);
    }
}
