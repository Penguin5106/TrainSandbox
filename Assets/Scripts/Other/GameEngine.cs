using UnityEngine;
using System.Collections.Generic;

public class GameEngine : MonoBehaviour 
{
    private static GameEngine instance; 
    
    [SerializeField] private const int GridWidth = 10;
    [SerializeField] private const int GridHeight = 10;
    private const float TileOffset = 0.5f;
    
    
    public Tile[,] railGrid;
    private GameObject[,] tileObjectGrid;
    private List<GameObject> Trains;
    
    //[SerializeField] private GameObject railPrefab;
    //[SerializeField] private GameObject trainPrefab;
    //[SerializeField] private GameObject tilePrefab;

    [SerializeField] private List<Spawner> spawners;

    private GameEngine()
    {
        Trains = new List<GameObject>();
        railGrid = new Tile[GridHeight, GridWidth];
        tileObjectGrid = new GameObject[GridHeight, GridWidth];

        for (int i = 0 ; i < GridHeight; i++)
        {
            for (int j = 0; i < GridWidth; i++)
            {
                railGrid[i,j] = new Tile();
            }
        }
        
    }

    private void Start()
    {
        GenerateGrid();
    }

    public static GameEngine GetInstance()
    {
        return instance ??= new GameEngine();
    }

    private void GenerateGrid()
    {
        TileSpawner tileSpawner = null;
        RailSpawner railSpawner = null;
        StationSpawner stationSpawner = null;

        foreach (ISpawner spawner in spawners)
        {
            if (spawner is TileSpawner)
            {
                tileSpawner = (TileSpawner)spawner;
            }

            if (spawner is RailSpawner)
            {
                railSpawner = (RailSpawner)spawner;
            }

            if (spawner is StationSpawner)
            {
                stationSpawner = (StationSpawner)spawner;
            }

        }

        if (tileSpawner != null)
        {
            for (int i = 0; i < railGrid.GetLength(0); i++)
            {
                for (int j = 0; j < railGrid.GetLength(1); j++)
                {
                    if (railGrid[i, j] is Station)
                    {
                        GameObject station = stationSpawner.Spawn(gameObject);

                        tileObjectGrid[i, j] = station;
                        
                        station.transform.position = (new Vector3 (j * TileOffset, i * TileOffset, 0));
                        // TODO set station parameters in the gameobject
                        station.GetComponent<Rail>().setConnections(railGrid[i, j].connections);
                    }

                    if (railGrid[i, j] is Rail)
                    {
                        GameObject rail = railSpawner.Spawn(gameObject);
                        
                        tileObjectGrid[i, j] = rail;
                        
                        rail.transform.position = (new Vector3 (j * TileOffset, i * TileOffset, 0));
                        // TODO set rail parameters in the gameobject
                        rail.GetComponent<Rail>().setConnections(railGrid[i, j].connections);
                    }
                    GameObject tileObject = tileSpawner.Spawn(gameObject);
                    tileObjectGrid[i, j] = tileObject;
                    
                    tileObject.transform.position = (new Vector3 (j * TileOffset, i * TileOffset, 0));
                }
            }
        }

    }

    public void AddTrain(Vector2Int position)
    {
        foreach (ISpawner spawner in spawners)
        {
            if (spawner is TrainSpawner)
            {
                GameObject train = spawner.Spawn(gameObject);

                train.GetComponent<Train>().setPosition(position);
                
                Trains.Add(train);
                
            }
        }
    }

    public void DeleteTrain(GameObject train)
    {
        Trains.Remove(train);
        Destroy(train);
    }

    public void DisplayTrainTimetable(Train train)
    {
        train.GetTimetable();
        // TODO display the timetable retrieved above
    }

    public void SetTrainTimetable(Train train, List<Station> stations)
    {
        bool result = train.SetNewTimetable(stations);

        if (!result)
        {
            Debug.Log("a portion of the path was not able to be generated");
        }
    }

    public void AddRail(Vector2Int position, Directions[] connections)
    {
        foreach (ISpawner spawner in spawners)
        {
            if (spawner is RailSpawner)
            { 
                Destroy(tileObjectGrid[position.x, position.y]);
                
                GameObject rail = spawner.Spawn(gameObject);
            
                tileObjectGrid[position.x, position.y] = rail;
                
                rail.transform.position = new Vector3(position.x, position.y, 0);
                
                railGrid[position.x, position.y] = new Rail(connections);
                
            }
        }
    }

    public void SetRailAttributes(Rail rail, bool occupied, Directions[] connections)
    {

    }

    public void DeleteRail(Vector2Int position)
    {
        railGrid[position.x, position.y] = new Tile();
        Destroy(tileObjectGrid[position.x, position.y]);

        foreach (ISpawner spawner in spawners)
        {
            if (spawner is TileSpawner)
            {
                GameObject tileObject = spawner.Spawn(gameObject);
                
                tileObjectGrid[position.x, position.y] = tileObject;
            }
        }

    }

    private void SimulateOneTurn()
    {
        foreach (GameObject train in Trains)
        {
            GetComponent<Train>().move();
        }
    }
    
}
