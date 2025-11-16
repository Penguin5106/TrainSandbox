using UnityEngine;
using System.Collections.Generic;

public class GameEngine
{
    private static GameEngine instance; 
    
    [SerializeField] private const int GridWidth = 10;
    [SerializeField] private const int GridHeight = 10;
    
    
    public Tile[,] railGrid;
    private List<Train> Trains;
    
    [SerializeField] GameObject railPrefab;
    [SerializeField] GameObject trainPrefab;
    [SerializeField] GameObject TilePrefab;

    private GameEngine()
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

    public static GameEngine GetInstance()
    {
        return instance ??= new GameEngine();
    }

    public void AddTrain(int[] position)
    {
        
    }

    public void DeleteTrain(Train train)
    {
        
    }

    public void DisplayTrainTimetable(Train train)
    {
        
    }

    public void SetTrainTimetable(Train train)
    {
        
    }

    public void AddRail(int[] position, Directions[] connections)
    {
        
    }

    public void SetRailAttributes(Rail rail, int[] position, bool occupied, Directions[] connections)
    {

    }

    public void DeleteRail(Rail rail)
    {
        
    }

    private void SimulateOneTurn()
    {
        
    }
    
}
