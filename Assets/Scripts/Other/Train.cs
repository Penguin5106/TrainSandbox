using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Train
{
    public int xPos;
    public int yPos;

    private List<Station> timetable { get; set; }

    private List<int[]> path;

    private int pathPosition;

    public Train(int xPos, int yPos)
    {
        this.xPos = xPos;
        this.yPos = yPos;
    }

    public void move()
    {
        pathPosition++;

        xPos = path[pathPosition][0];
        yPos = path[pathPosition][1];
    }

    public void pathfind(Rail[][] railGrid, int[] start, int[] goal)
    {
        List<int[]> shortestPath = new List<int[]>();
        
        
        
    }
}
