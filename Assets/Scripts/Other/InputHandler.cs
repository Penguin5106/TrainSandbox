using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    private static InputHandler instance;
    [SerializeField] private Camera playerCamera;
    private IClickable clickable;

    private void Start()
    {
        instance = this;
    }

    public static InputHandler GetInstance()
    {
        return instance ?? new InputHandler();
    }
    public void OnClick(InputAction.CallbackContext context)
    {
        if (context.ReadValueAsButton())
        {
            return;
        }

        Vector2 mousePos = playerCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        if (hit)
        {
            clickable = hit.collider.GetComponent<IClickable>() ?? clickable;
            Debug.Log("clickable is " + clickable.GetType().Name);
            
            clickable?.Click();
            return;
        }
        
        UIManager.GetInstance().SetUIPanelActive(UIPanel.None);
        clickable = null;
        
    }

    public void SpawnTrainOnRail()
    {
        if (clickable is Rail rail)
        {
            GameEngine.GetInstance().AddTrain(rail);
        }
    }

    public void ToggleRailConnection(int direction)
    {
        if (clickable is Rail rail)
        {
            bool[] newConnections = rail.getConnections();
            
            newConnections[direction] = !newConnections[direction];

            if (!newConnections.Contains(true))
            {
                clickable = GameEngine.GetInstance().DeleteRail(rail);
                return;
            }
            
            rail.setConnections(newConnections);
            return;
        }

        if (clickable is Tile tile)
        {
            Debug.Log(direction);
            clickable = GameEngine.GetInstance().TileToRail(tile, new Directions[]{(Directions)direction});
        }
        
    }

    public void SetStationName(string stationName)
    {
        if (clickable is Station station)
        {
            if (stationName != "")
            {
                station.SetName(stationName);
                return;
            }
            
            if (!station.connections.Contains(true))
            {
                clickable = GameEngine.GetInstance().DeleteRail(station);
                return;
            }

            clickable = GameEngine.GetInstance().StationToRail(station);
            return;
        }

        if (clickable is Rail rail)
        {
            clickable = GameEngine.GetInstance().RailToStation(rail,  stationName);
            return;
        }
    }

    public void clearTimetable()
    {
        if (clickable is Train train)
        {
            train.SetNewTimetable(new List<Station>());
        }
    }

    public void AddStationByName(string stationName)
    {
        if (clickable is Train train)
        {
            train.AddStationToTimetable(GameEngine.GetInstance().GetStationByName(stationName));
        }
    }
}
