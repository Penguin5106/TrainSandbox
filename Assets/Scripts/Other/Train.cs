using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Train : MonoBehaviour , IClickable
{
    Vector2Int position;

    private List<Station> timetable { get; set; }

    private List<Vector2Int> temporaryPath;
    private List<Vector2Int> path;

    private int temporaryPathPosition;
    private int pathPosition;

    public Train(int xPos, int yPos)
    {
        position = new Vector2Int(xPos, yPos);
    }

    public Train()
    {
        position = new Vector2Int(0, 0);
    }

    public List<Station> GetTimetable()
    {
        return timetable;
    }

    public void setPosition(int xPos, int yPos)
    {
        position =  new Vector2Int(xPos, yPos);
        gameObject.transform.position = new  Vector3(position.x, position.y, 0);
    }
    
    public void setPosition(Vector2Int newPos)
    {
        position =  newPos;
        gameObject.transform.position = new  Vector3(position.x, position.y, 0);
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
        temporaryPathPosition++;

        setPosition(temporaryPath[temporaryPathPosition]);
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

        //implement A* algorithm here

        pathfindStats[start[0], start[1]] = new Vector2(0, (float)optimisedMagnitude(start - goal));

        bool pathFound = false;
        Vector2Int ClosestPoint = start;
        HashSet<Vector2Int> openSet = new HashSet<Vector2Int> { ClosestPoint };

        while (!pathFound)
        {
            
            // gather connections to (optimistic) closest point
            int connectionsIndex = 0;
            while (connectionsIndex < railGrid[ClosestPoint[0], ClosestPoint[1]].connections.Length)
            {
                if (railGrid[ClosestPoint[0], ClosestPoint[1]].connections[connectionsIndex])
                {
                    // if the 2 rail pieces are connected update the new ones stats for both path cost and if necessary the heuristic
                    switch(connectionsIndex)
                    {
                        case (int)Directions.Up:

                            if (ClosestPoint[0] + 1 < railGrid.GetLength(0) &&
                                railGrid[ClosestPoint.y + 1, ClosestPoint.x].connections[(int)Directions.Down])
                            {
                                float newLength = generatePathLength(pathfindStats[ClosestPoint.y, ClosestPoint.x], 1);
                                
                                if (newLength < pathfindStats[ClosestPoint.y + 1, ClosestPoint.x].x)
                                {
                                    pathfindStats[ClosestPoint.y + 1, ClosestPoint.x].x = newLength;
                                    
                                    openSet.Add(new Vector2Int(ClosestPoint.y + 1, ClosestPoint.x));
                                }

                                if (pathfindStats[ClosestPoint.y + 1, ClosestPoint.x].y.Equals(0))
                                {
                                    pathfindStats[ClosestPoint.y + 1, ClosestPoint.x].y =
                                        (float)optimisedMagnitude(goal - new Vector2Int(ClosestPoint.y + 1, ClosestPoint.x));
                                }
                                /*pathfindStats[ClosestPoint.y + 1, ClosestPoint.x] = generateNewStats(
                                    new Vector2Int(ClosestPoint.y + 1, ClosestPoint.x), goal,
                                    pathfindStats[ClosestPoint.y + 1, ClosestPoint.x],
                                    pathfindStats[ClosestPoint.y, ClosestPoint.x], 1);
                                */
                            }

                            break;

                        case (int)Directions.Down:

                            if (ClosestPoint[0] - 1 > 0 && railGrid[ClosestPoint.y - 1, ClosestPoint.x].connections[(int)Directions.Up])
                            { 
                                float newLength = generatePathLength(pathfindStats[ClosestPoint.y, ClosestPoint.x], 1);
                                
                                if (newLength < pathfindStats[ClosestPoint.y - 1, ClosestPoint.x].x)
                                {
                                    pathfindStats[ClosestPoint.y - 1, ClosestPoint.x].x = newLength;
                                    
                                    openSet.Add(new Vector2Int(ClosestPoint.y - 1, ClosestPoint.x));
                                }

                                if (pathfindStats[ClosestPoint.y - 1, ClosestPoint.x].y.Equals(0))
                                {
                                    pathfindStats[ClosestPoint.y - 1, ClosestPoint.x].y =
                                        (float)optimisedMagnitude(goal - new Vector2Int(ClosestPoint.y - 1, ClosestPoint.x));
                                }
                                
                            }

                            break;
                        
                        case (int)Directions.Left:

                            if (ClosestPoint[1] - 1 > 0 && railGrid[ClosestPoint.y, ClosestPoint.x - 1].connections[(int)Directions.Right])
                            { 
                                float newLength = generatePathLength(pathfindStats[ClosestPoint.y, ClosestPoint.x], 1);
                                
                                if (newLength < pathfindStats[ClosestPoint.y, ClosestPoint.x - 1].x)
                                {
                                    pathfindStats[ClosestPoint.y, ClosestPoint.x - 1].x = newLength;
                                    
                                    openSet.Add(new Vector2Int(ClosestPoint.y, ClosestPoint.x - 1));
                                }

                                if (pathfindStats[ClosestPoint.y, ClosestPoint.x - 1].y.Equals(0))
                                {
                                    pathfindStats[ClosestPoint.y, ClosestPoint.x - 1].y =
                                        (float)optimisedMagnitude(goal - new Vector2Int(ClosestPoint.y, ClosestPoint.x - 1));
                                } 
                            }

                            break;
                        
                        case (int)Directions.Right:

                            if (ClosestPoint[1] + 1 < railGrid.GetLength(1) &&
                                railGrid[ClosestPoint.y, ClosestPoint.x + 1].connections[(int)Directions.Left])
                            {
                                float newLength = generatePathLength(pathfindStats[ClosestPoint.y, ClosestPoint.x], 1);
                                
                                if (newLength < pathfindStats[ClosestPoint.y, ClosestPoint.x + 1].x)
                                {
                                    pathfindStats[ClosestPoint.y, ClosestPoint.x + 1].x = newLength;
                                    
                                    openSet.Add(new Vector2Int(ClosestPoint.y, ClosestPoint.x + 1));
                                }

                                if (pathfindStats[ClosestPoint.y, ClosestPoint.x + 1].y.Equals(0))
                                {
                                    pathfindStats[ClosestPoint.y, ClosestPoint.x + 1].y =
                                        (float)optimisedMagnitude(goal - new Vector2Int(ClosestPoint.y, ClosestPoint.x + 1));
                                } 
                            }

                            break;

                        case (int)Directions.UpLeft:

                            if (ClosestPoint[0] + 1 < railGrid.GetLength(0) && ClosestPoint[1] - 1 > 0 &&
                                railGrid[ClosestPoint.y + 1, ClosestPoint.x - 1].connections[(int)Directions.DownRight])
                            {
                                float newLength = generatePathLength(pathfindStats[ClosestPoint.y, ClosestPoint.x], 1.4f);
                                
                                if (newLength < pathfindStats[ClosestPoint.y + 1, ClosestPoint.x - 1].x)
                                {
                                    pathfindStats[ClosestPoint.y + 1, ClosestPoint.x - 1].x = newLength;
                                    
                                    openSet.Add(new Vector2Int(ClosestPoint.y + 1, ClosestPoint.x - 1));
                                }

                                if (pathfindStats[ClosestPoint.y + 1, ClosestPoint.x - 1].y.Equals(0))
                                {
                                    pathfindStats[ClosestPoint.y + 1, ClosestPoint.x - 1].y =
                                        (float)optimisedMagnitude(goal - new Vector2Int(ClosestPoint.y + 1, ClosestPoint.x - 1));
                                }
                            }
                            
                            break;
                        case (int)Directions.UpRight:

                            if (ClosestPoint[0] + 1 < railGrid.GetLength(0) &&
                                ClosestPoint[1] + 1 < railGrid.GetLength(1) &&
                                railGrid[ClosestPoint.y + 1, ClosestPoint.x + 1].connections[(int)Directions.DownLeft])
                            {
                                float newLength = generatePathLength(pathfindStats[ClosestPoint.y, ClosestPoint.x], 1.4f);
                                
                                if (newLength < pathfindStats[ClosestPoint.y + 1, ClosestPoint.x + 1].x)
                                {
                                    pathfindStats[ClosestPoint.y + 1, ClosestPoint.x + 1].x = newLength;
                                    
                                    openSet.Add(new Vector2Int(ClosestPoint.y + 1, ClosestPoint.x + 1));
                                }

                                if (pathfindStats[ClosestPoint.y + 1, ClosestPoint.x + 1].y.Equals(0))
                                {
                                    pathfindStats[ClosestPoint.y + 1, ClosestPoint.x + 1].y =
                                        (float)optimisedMagnitude(goal - new Vector2Int(ClosestPoint.y + 1, ClosestPoint.x + 1));
                                }
                            }

                            break;
                        case (int)Directions.DownLeft:

                            if (ClosestPoint[0] - 1 > 0 && ClosestPoint[1] - 1 > 0 &&
                                railGrid[ClosestPoint.y - 1, ClosestPoint.x - 1].connections[(int)Directions.UpRight])
                            {
                                float newLength = generatePathLength(pathfindStats[ClosestPoint.y, ClosestPoint.x], 1.4f);
                                
                                if (newLength < pathfindStats[ClosestPoint.y - 1, ClosestPoint.x - 1].x)
                                {
                                    pathfindStats[ClosestPoint.y - 1, ClosestPoint.x - 1].x = newLength;
                                    
                                    openSet.Add(new Vector2Int(ClosestPoint.y - 1, ClosestPoint.x - 1));
                                }

                                if (pathfindStats[ClosestPoint.y - 1, ClosestPoint.x - 1].y.Equals(0))
                                {
                                    pathfindStats[ClosestPoint.y - 1, ClosestPoint.x - 1].y =
                                        (float)optimisedMagnitude(goal - new Vector2Int(ClosestPoint.y - 1, ClosestPoint.x - 1));
                                }
                            }
                            
                            break;
                        case (int)Directions.DownRight:

                            if (ClosestPoint[0] - 1 > 0 && ClosestPoint[1] + 1 < railGrid.GetLength(1) &&
                                railGrid[ClosestPoint.y - 1, ClosestPoint.x + 1].connections[(int)Directions.UpLeft])
                            {
                                float newLength = generatePathLength(pathfindStats[ClosestPoint.y, ClosestPoint.x], 1.4f);
                                
                                if (newLength < pathfindStats[ClosestPoint.y - 1, ClosestPoint.x + 1].x)
                                {
                                    pathfindStats[ClosestPoint.y - 1, ClosestPoint.x + 1].x = newLength;
                                    
                                    openSet.Add(new Vector2Int(ClosestPoint.y - 1, ClosestPoint.x + 1));
                                }

                                if (pathfindStats[ClosestPoint.y - 1, ClosestPoint.x + 1].y.Equals(0))
                                {
                                    pathfindStats[ClosestPoint.y - 1, ClosestPoint.x + 1].y =
                                        (float)optimisedMagnitude(goal - new Vector2Int(ClosestPoint.y - 1, ClosestPoint.x + 1));
                                }
                            }
                            
                            break;
                        default:
                            // if you get here something went wrong
                            break;
                    }
                    
                }


                connectionsIndex++;
            }
            
            openSet.Remove(ClosestPoint);
            if (openSet.Count != 0)
            {
                Vector2 ClosestPointStats = new Vector2(9999, 9999);
                foreach (Vector2Int Node in openSet)
                {

                    if (pathfindStats[Node.y, Node.x].x + pathfindStats[Node.y, Node.x].y < ClosestPointStats.x + ClosestPointStats.y || 
                        (pathfindStats[Node.y, Node.x].x + pathfindStats[Node.y, Node.x].y == ClosestPointStats.x + ClosestPointStats.y &&  pathfindStats[Node.y, Node.x].y < ClosestPointStats.y))
                    {
                        ClosestPoint = Node;
                        ClosestPointStats = pathfindStats[Node.y, Node.x];
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

                return null;
            }

            
        }
        
        //walkback to find path
        while (shortestPath[^1] != start)
        {
            float shortestDistance = pathfindStats[ClosestPoint.y, ClosestPoint.x].x;
            Vector2Int nextPathStep = ClosestPoint;
            int connectionIndex = 0;
            while (connectionIndex < railGrid[ClosestPoint[0], ClosestPoint[1]].connections.Length)
            {
                if (railGrid[ClosestPoint[0], ClosestPoint[1]].connections[connectionIndex])
                {
                    switch(connectionIndex)
                    {
                        case (int)Directions.Up:

                            if (ClosestPoint[0] + 1 < railGrid.GetLength(0) &&
                                railGrid[ClosestPoint.y + 1, ClosestPoint.x].connections[(int)Directions.Down] && !pathfindStats[ClosestPoint.y + 1, ClosestPoint.x].y.Equals(0))
                            {
                                if (pathfindStats[ClosestPoint.y + 1, ClosestPoint.x].x < shortestDistance)
                                {
                                    shortestDistance = pathfindStats[ClosestPoint.y + 1, ClosestPoint.x].x;
                                    nextPathStep = new Vector2Int(ClosestPoint.y + 1, ClosestPoint.x);
                                }
                            }

                            break;

                        case (int)Directions.Down:

                            if (ClosestPoint[0] - 1 > 0 && railGrid[ClosestPoint.y - 1, ClosestPoint.x].connections[(int)Directions.Up] && !pathfindStats[ClosestPoint.y - 1, ClosestPoint.x].y.Equals(0))
                            { 
                                if (pathfindStats[ClosestPoint.y - 1, ClosestPoint.x].x < shortestDistance)
                                {
                                    shortestDistance = pathfindStats[ClosestPoint.y - 1, ClosestPoint.x].x;
                                    nextPathStep = new Vector2Int(ClosestPoint.y - 1, ClosestPoint.x);
                                }
                            }

                            break;
                        
                        case (int)Directions.Left:

                            if (ClosestPoint[1] - 1 > 0 && railGrid[ClosestPoint.y, ClosestPoint.x - 1].connections[(int)Directions.Right] && !pathfindStats[ClosestPoint.y, ClosestPoint.x - 1].y.Equals(0))
                            { 
                                if (pathfindStats[ClosestPoint.y, ClosestPoint.x - 1].x < shortestDistance)
                                {
                                    shortestDistance = pathfindStats[ClosestPoint.y, ClosestPoint.x - 1].x;
                                    nextPathStep = new Vector2Int(ClosestPoint.y, ClosestPoint.x - 1);
                                }
                            }

                            break;
                        
                        case (int)Directions.Right:

                            if (ClosestPoint[1] + 1 < railGrid.GetLength(1) &&
                                railGrid[ClosestPoint.y, ClosestPoint.x + 1].connections[(int)Directions.Left] && !pathfindStats[ClosestPoint.y, ClosestPoint.x + 1].y.Equals(0))
                            {
                                if (pathfindStats[ClosestPoint.y, ClosestPoint.x + 1].x < shortestDistance)
                                {
                                    shortestDistance = pathfindStats[ClosestPoint.y, ClosestPoint.x + 1].x;
                                    nextPathStep = new Vector2Int(ClosestPoint.y, ClosestPoint.x + 1);
                                }
                            }

                            break;

                        case (int)Directions.UpLeft:

                            if (ClosestPoint[0] + 1 < railGrid.GetLength(0) && ClosestPoint[1] - 1 > 0 &&
                                railGrid[ClosestPoint.y + 1, ClosestPoint.x - 1].connections[(int)Directions.DownRight] && !pathfindStats[ClosestPoint.y + 1, ClosestPoint.x - 1].y.Equals(0))
                            {
                                if (pathfindStats[ClosestPoint.y + 1, ClosestPoint.x - 1].x < shortestDistance)
                                {
                                    shortestDistance = pathfindStats[ClosestPoint.y + 1, ClosestPoint.x - 1].x;
                                    nextPathStep = new Vector2Int(ClosestPoint.y + 1, ClosestPoint.x - 1);
                                }
                            }
                            
                            break;
                        case (int)Directions.UpRight:

                            if (ClosestPoint[0] + 1 < railGrid.GetLength(0) &&
                                ClosestPoint[1] + 1 < railGrid.GetLength(1) &&
                                railGrid[ClosestPoint.y + 1, ClosestPoint.x + 1].connections[(int)Directions.DownLeft] && !pathfindStats[ClosestPoint.y + 1, ClosestPoint.x + 1].y.Equals(0))
                            {
                                if (pathfindStats[ClosestPoint.y + 1, ClosestPoint.x + 1].x < shortestDistance)
                                {
                                    shortestDistance = pathfindStats[ClosestPoint.y + 1, ClosestPoint.x + 1].x;
                                    nextPathStep = new Vector2Int(ClosestPoint.y + 1, ClosestPoint.x + 1);
                                }
                            }

                            break;
                        case (int)Directions.DownLeft:

                            if (ClosestPoint[0] - 1 > 0 && ClosestPoint[1] - 1 > 0 &&
                                railGrid[ClosestPoint.y - 1, ClosestPoint.x - 1].connections[(int)Directions.UpRight] && !pathfindStats[ClosestPoint.y - 1, ClosestPoint.x - 1].y.Equals(0))
                            {
                                if (pathfindStats[ClosestPoint.y - 1, ClosestPoint.x - 1].x < shortestDistance)
                                {
                                    shortestDistance = pathfindStats[ClosestPoint.y - 1, ClosestPoint.x - 1].x;
                                    nextPathStep = new Vector2Int(ClosestPoint.y - 1, ClosestPoint.x - 1);
                                } 
                            }
                            
                            break;
                        case (int)Directions.DownRight:

                            if (ClosestPoint[0] - 1 > 0 && ClosestPoint[1] + 1 < railGrid.GetLength(1) &&
                                railGrid[ClosestPoint.y - 1, ClosestPoint.x + 1].connections[(int)Directions.UpLeft] && !pathfindStats[ClosestPoint.y - 1, ClosestPoint.x + 1].y.Equals(0))
                            {
                                if (pathfindStats[ClosestPoint.y - 1, ClosestPoint.x + 1].x < shortestDistance)
                                {
                                    shortestDistance = pathfindStats[ClosestPoint.y - 1, ClosestPoint.x + 1].x;
                                    nextPathStep = new Vector2Int(ClosestPoint.y - 1, ClosestPoint.x + 1);
                                } 
                            }
                            
                            break;
                        default:
                            // if you get here something went wrong
                            break;
                    }
                }

                connectionIndex++;
            }
            shortestPath.Add(nextPathStep);
            ClosestPoint = nextPathStep;
        }
        return shortestPath;
    }

    private bool generatePath()
    {
        Tile[,] grid = GameEngine.GetInstance().railGrid;

        Vector2Int pathfindStart = position;
        
        path.Clear();

        int iteration = 0;
        
        foreach (Station station in timetable)
        {
            Vector2Int pathfindGoal = station.gridPosition;

            if (iteration == 0)
            {
                temporaryPath = pathfind(grid, pathfindStart, pathfindGoal);
            }
            else
            {
                path.Concat(pathfind(grid, pathfindStart, pathfindGoal) ?? new List<Vector2Int>());

                if (path.Count == 0)
                {
                    return false;
                }
            }
            
            iteration++;
            pathfindStart = pathfindGoal;
        }
        return true;
    }

    public bool SetNewTimetable(List<Station> stations)
    {
        timetable = stations;
        
        return generatePath();
    }

    public void Click()
    {
        UIManager.GetInstance().SetUIPanelActive(UIPanel.Train);
    }
}
