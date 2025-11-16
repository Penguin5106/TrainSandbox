using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Train
{
    Vector2 position;

    private List<Station> timetable { get; set; }

    private List<Vector2> TemporaryPath;
    private List<Vector2> path;

    private int pathPosition;

    public Train(int xPos, int yPos)
    {
        position = new Vector2(xPos, yPos);
    }

    public void move()
    {
        pathPosition++;

        position =  new Vector2(path[pathPosition].x, path[pathPosition].y);
    }

    private List<Vector2> pathfind(Tile[,] railGrid, Vector2 start, Vector2 goal)
    {
        List<Vector2> shortestPath = new List<Vector2>();
        
        
        
        //implement A* algorithm here
        
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
