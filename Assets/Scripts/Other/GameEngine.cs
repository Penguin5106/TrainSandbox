using UnityEngine;
using System.Collections.Generic;

public class GameEngine
{
    [SerializeField] private const int GridWidth = 10;
    [SerializeField] private const int GridHeight = 10;
    
    
    public Tile[,] railGrid;
    private List<Train> Trains;
    
    [SerializeField] GameObject railPrefab;
    [SerializeField] GameObject trainPrefab;
    [SerializeField] GameObject TilePrefab;

    public GameEngine()
    {
        Trains = new List<Train>();
        railGrid = new Tile[GridHeight, GridWidth];

        for (int i = 0 ; i < GridHeight; i++)
        {
            for (int j = 0; i < GridWidth; i++)
            {
                railGrid[i,j] = new Tile();
            }
        }

    }

    public void AddTrain(int[] position)
    {
        Trains.Add(new Train(position[0], position[1]));
    }

    public void DeleteTrain(Train train)
    {
        Trains.Remove(train);
    }

    public void addRail(int[] position, Directions[] connections)
    {
        railGrid[position[0], position[1]] = new Rail(connections);
    }

    public void setRailAttributes(int[] position, bool occupied, Directions[] connections)
    {

    }
}
