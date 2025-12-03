using System;
using UnityEngine;

public enum Directions
{
    UpLeft = 0, Up = 1, UpRight = 2, Left = 3, Right = 4, DownLeft = 5, Down = 6, DownRight = 7
}

public class Tile : MonoBehaviour , IClickable
{
    protected internal bool[] connections { get; protected set; }
    
    public Vector2Int gridPosition;

    public Tile()
    {
        connections = new bool[8];

        for (int i = 0; i < connections.Length; i++)
        {
            connections[i] = false;
        }
    }

    public void Click()
    {
        Debug.Log("tile clicked");
    }
}
