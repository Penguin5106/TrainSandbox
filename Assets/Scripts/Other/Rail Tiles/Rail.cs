using UnityEngine;

public class Rail : Tile
{

    public bool isOccupied;

    public Rail(Directions[] connections)
    {
        isOccupied = false;

        this.connections = new bool[8];

        for (int i = 0; i < this.connections.Length; i++)
        {
            this.connections[i] = false;
        }

        for (int i = 0; i< connections.Length; i++)
        {
            this.connections[(int)connections[i]] = true;
        }
    }

    public void setConnections(Directions[] connections)
    {
        this.connections = new bool[8];

        for (int i = 0; i < this.connections.Length; i++)
        {
            this.connections[i] = false;
        }

        for (int i = 0; i< connections.Length; i++)
        {
            this.connections[(int)connections[i]] = true;
        }
    }
}
