using UnityEngine;
using System.Collections.Generic;

public class GameEngine : MonoBehaviour 
{
    private static GameEngine instance; 
    
    [SerializeField] private const int GridWidth = 10;
    [SerializeField] private const int GridHeight = 10;
    private const float TileOffset = 0.5f;
    
    public Tile[,] railGrid;
    private List<Train> Trains;
    
    private List<Station> Stations;

    [SerializeField] private List<Spawner> spawners;

    

    private void Start()
    {
        instance = this;
        
        Trains = new List<Train>();
        railGrid = new Tile[GridHeight, GridWidth];
        Stations = new List<Station>();

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
                        railGrid[j, i] = tileSpawner?.Spawn(gameObject).GetComponent<Tile>();
                        railGrid[j, i].transform.position = (new Vector3(j * TileOffset, i * TileOffset, 0));
                    }
                }
            }
        }
    }

    public static GameEngine GetInstance()
    {
        return instance;
    }

    public static float GetTileOffset()
    {
        return TileOffset;
    }

    public List<Station> GetStations()
    {
        return Stations;
    }

    public Station GetStationByName(string stationName)
    {
        foreach (Station station in Stations)
        {
            Debug.Log(station.GetName());
            Debug.Log(stationName);
            
            if (station.GetName() ==  stationName)
                return station;
        }
        
        Debug.Log("no station found");
        return null;
    }

    public void AddTrain(Tile tile)
    {
        Vector2Int position = GetTilePosition(tile);
        
        AddTrain(position);
    }
    
    public void AddTrain(Vector2Int position)
    {
        if (!railGrid[position.y, position.x] is Rail)
        {
            return;
        }

        Rail rail = (Rail)railGrid[position.y, position.x];
        if (rail.isOccupied)
        {
            return;
        }
        
        foreach (ISpawner spawner in spawners)
        {
            if (spawner is TrainSpawner)
            {
                GameObject trainObject = spawner.Spawn(gameObject);
                
                Train train = trainObject.GetComponent<Train>();
                train.setPosition(position);
                
                Trains.Add(train);
                
                rail.isOccupied = true;
            }
        }
    }

    public void DeleteTrain(Train train)
    {
        Trains.Remove(train);

        if (railGrid[train.GetPosition().y, train.GetPosition().x] is Rail rail)
        {
            rail.isOccupied = false;
        }
        
        Destroy(train);
    }

    public void SetTrainTimetable(Train train, List<Station> stations)
    {
        bool result = train.SetNewTimetable(stations);

        if (!result)
        {
            Debug.Log("a portion of the path was not able to be generated");
        }
    }

    private Rail AddRail(Vector2Int position, Directions[] connections)
    {
        foreach (ISpawner spawner in spawners)
        {
            if (spawner is StationSpawner) continue;
            
            if (spawner is RailSpawner)
            { 
                Destroy(railGrid[position.y, position.x].gameObject);
                
                GameObject rail = spawner.Spawn(gameObject);
            
                railGrid[position.y, position.x] = rail.GetComponent<Rail>();
                
                rail.transform.position = new Vector3(position.y * TileOffset, position.x * TileOffset, 0);
                
                rail.GetComponent<Rail>().setConnections(connections);

                return (Rail)railGrid[position.y, position.x];
            }
        }
        return null;
    }
    
    private Rail AddRail(Vector2Int position, bool[] connections)
    {
        foreach (ISpawner spawner in spawners)
        {
            if (spawner is StationSpawner) continue;
            
            if (spawner is RailSpawner)
            { 
                Destroy(railGrid[position.y, position.x].gameObject);
                
                GameObject rail = spawner.Spawn(gameObject);
            
                railGrid[position.y, position.x] = rail.GetComponent<Rail>();
                
                rail.transform.position = new Vector3(position.y * TileOffset, position.x * TileOffset, 0);
                
                rail.GetComponent<Rail>().setConnections(connections);

                return (Rail)railGrid[position.y, position.x];
            }
        }
        return null;
    }

    public Rail TileToRail(Tile tile, Directions[] connections)
    {
        Vector2Int position = GetTilePosition(tile);

        if (position == new Vector2Int(-1, -1))
        {
            return null;
        }
        
        return AddRail(position, connections);
    }
    
    public Station RailToStation(Rail rail, string name)
    {
        Vector2Int position = GetTilePosition(rail);

        if (position == new Vector2Int(-1, -1))
        {
            return null;
        }
        
        return AddStation(position, rail.connections, name);
    }

    private Station AddStation(Vector2Int position, bool[] connections, string s)
    {
        foreach (ISpawner spawner in spawners)
        {
            if (spawner is StationSpawner)
            { 
                Destroy(railGrid[position.y, position.x].gameObject);
                
                GameObject station = spawner.Spawn(gameObject);
            
                railGrid[position.y, position.x] = station.GetComponent<Station>();
                
                station.transform.position = new Vector3(position.y * TileOffset, position.x * TileOffset, 0);
                
                station.GetComponent<Rail>().setConnections(connections);
                station.GetComponent<Station>().SetName(s);

                Stations.Add((Station)railGrid[position.y, position.x]);

                return (Station)railGrid[position.y, position.x];
            }
        }
        return null;
    }

    public Rail StationToRail(Station station)
    {
        Vector2Int position = GetTilePosition(station);
        if (position == new Vector2Int(-1, -1))
        {
            return null;
        }

        Stations.Remove(station);

        return AddRail(position, station.connections);
    }

    public void SetRailAttributes(Rail rail, bool occupied, Directions[] connections)
    {

    }

    public Tile DeleteRail(Vector2Int position)
    {
        if (railGrid[position.x, position.y] is Station station)
        {
            Stations.Remove(station);
        }

        Destroy(railGrid[position.x, position.y].gameObject);

        foreach (ISpawner spawner in spawners)
        {
            if (spawner is RailSpawner) continue;
            
            if (spawner is TileSpawner)
            {
                GameObject tileObject = spawner.Spawn(gameObject);
                
                return railGrid[position.x, position.y] = tileObject.GetComponent<Tile>();
            }
        }

        return null;
    }
    
    public Tile DeleteRail(Rail rail)
    {
        Vector2Int position = GetTilePosition(rail);

        if (position == new Vector2Int(-1, -1))
        {
            return null;
        }

        if (railGrid[position.x, position.y] is Station station)
        {
            Stations.Remove(station);
        }

        Destroy(railGrid[position.y, position.x].gameObject);

        foreach (ISpawner spawner in spawners)
        {
            if (spawner is RailSpawner) continue;
            
            if (spawner is TileSpawner)
            {
                GameObject tileObject = spawner.Spawn(gameObject);
                
                tileObject.transform.position = new Vector3(position.y * TileOffset, position.x * TileOffset, 0);
                
                railGrid[position.y, position.x] = tileObject.GetComponent<Tile>();
                return  railGrid[position.y, position.x];
            }
        }
        return  railGrid[position.y, position.x];
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
                    position = new Vector2Int(j, i);
                }
            }
        }
        
        return position;
    }
    private void SimulateOneTurn()
    {
        foreach (Train train in Trains)
        {
            train.move();
        }
    }
    
}
