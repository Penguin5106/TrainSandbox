using UnityEngine;
using System.Collections.Generic;

public class GameEngine : MonoBehaviour 
{
    private static GameEngine instance; 
    
    [SerializeField] private const int GridWidth = 10;
    [SerializeField] private const int GridHeight = 10;
    private const float TileOffset = 0.5f;
    
    public Tile[,] railGrid;
    private List<GameObject> Trains;
    
    //[SerializeField] private GameObject railPrefab;
    //[SerializeField] private GameObject trainPrefab;
    //[SerializeField] private GameObject tilePrefab;

    [SerializeField] private List<Spawner> spawners;

    

    private void Start()
    {
        instance = this;
        
        Trains = new List<GameObject>();
        railGrid = new Tile[GridHeight, GridWidth];

        foreach (Spawner spawner in spawners)
        {
            if (spawner is RailSpawner) continue;
            
            if (spawner is TileSpawner)
            {
                var tileSpawner = spawner as TileSpawner;

                for (int i = 0; i < GridHeight; i++)
                {
                    for (int j = 0; j < GridWidth; j++)
                    {
                        railGrid[i, j] = tileSpawner?.Spawn(gameObject).GetComponent<Tile>();
                        railGrid[i, j].transform.position = (new Vector3(j * TileOffset, i * TileOffset, 0));
                    }
                }
            }
        }
    }

    public static GameEngine GetInstance()
    {
        return instance;
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
            if (spawner is StationSpawner) continue;
            
            if (spawner is RailSpawner)
            { 
                Destroy(railGrid[position.x, position.y].gameObject);
                
                GameObject rail = spawner.Spawn(gameObject);
            
                railGrid[position.x, position.y] = rail.GetComponent<Rail>();
                
                rail.transform.position = new Vector3(position.x * TileOffset, position.y * TileOffset, 0);
                
                rail.GetComponent<Rail>().setConnections(connections);
                
            }
        }
    }

    public void TileToRail(Tile tile, Directions[] connections)
    {
        Vector2Int position = GetTilePosition(tile);

        if (position == new Vector2Int(-1, -1))
        {
            return;
        }
        
        AddRail(position, connections);
    }

    public void SetRailAttributes(Rail rail, bool occupied, Directions[] connections)
    {

    }

    public void DeleteRail(Vector2Int position)
    {
        Destroy(railGrid[position.x, position.y].gameObject);

        foreach (ISpawner spawner in spawners)
        {
            if (spawner is RailSpawner) continue;
            
            if (spawner is TileSpawner)
            {
                GameObject tileObject = spawner.Spawn(gameObject);
                
                railGrid[position.x, position.y] = tileObject.GetComponent<Tile>();
            }
        }

    }
    
    public void DeleteRail(Rail rail)
    {
        Vector2Int position = GetTilePosition(rail);

        if (position == new Vector2Int(-1, -1))
        {
            return;
        }
        
        Destroy(railGrid[position.x, position.y].gameObject);

        foreach (ISpawner spawner in spawners)
        {
            if (spawner is RailSpawner) continue;
            
            if (spawner is TileSpawner)
            {
                GameObject tileObject = spawner.Spawn(gameObject);
                
                railGrid[position.x, position.y] = tileObject.GetComponent<Tile>();
            }
        }

    }

    private Vector2Int GetTilePosition(Tile tile)
    {
        Vector2Int position = new Vector2Int( -1, -1 );
        
        for (int i = 0; i < GridHeight; i++)
        {
            for (int j = 0; j < GridWidth; j++)
            {
                if (ReferenceEquals(railGrid[i, j], tile))
                {
                    position = new Vector2Int(i, j);
                }
            }
        }
        
        return position;
    }
    private void SimulateOneTurn()
    {
        foreach (GameObject train in Trains)
        {
            train.GetComponent<Train>().move();
        }
    }
    
}
