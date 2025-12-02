using UnityEngine;

public class Rail : Tile
{

    public bool isOccupied;

    [SerializeField] private GameObject[] spriteRenderers;

    public Rail(Directions[] connections)
    {
        isOccupied = false;

        setConnections(connections);
    }

    public void setConnections(Directions[] connections)
    {
        this.connections = new bool[8];

        for (var i = 0; i < this.connections.Length; i++)
        {
            this.connections[i] = false;
        }

        foreach (var sprite in spriteRenderers)
        {
            sprite.SetActive(false);
        }
        
        foreach (var t in connections)
        {
            this.connections[(int)t] = true;
            
            spriteRenderers[(int)t].SetActive(true);
        }
        
    }

    public void setConnections(bool[] connections)
    {
        if (connections.Length == 8)
        {
            this.connections = connections;
        }
        
        for (int i = 0; i < 8; i++)
        {
            spriteRenderers[i].SetActive(connections[i]);
        }
    }
}
