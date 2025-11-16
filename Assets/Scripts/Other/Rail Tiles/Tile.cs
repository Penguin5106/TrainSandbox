using UnityEngine;

public enum Directions
{
    UPLeft = 0, UP = 1, UPRight = 2, Left = 3, Right = 4, DownLeft = 5, Down = 6, DownRight = 7
}

public class Tile
{
    public bool[] connections;
    
    public Vector2 gridPosition;

    public Tile()
    {
        connections = new bool[8];

        for (int i = 0; i < connections.Length; i++)
        {
            connections[i] = false;
        }
    }
}
