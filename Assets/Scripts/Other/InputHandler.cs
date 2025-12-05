using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    private IClickable clickable;
    
    public void OnClick(InputAction.CallbackContext context)
    {
        if (context.ReadValueAsButton())
        {
            return;
        }

        Vector2 mousePos = playerCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Debug.Log("click at " + mousePos);
        
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        if (hit)
        {
            Debug.Log("hit object" + hit.collider.name);
            clickable ??= hit.collider.GetComponent<IClickable>();
            
            clickable?.Click();
            return;
        }
        
        UIManager.GetInstance().SetUIPanelActive(UIPanel.None);
        
    }

    public void ToggleRailConnection(int direction)
    {
        Debug.Log("Toggle Rail Connection");
        if (clickable is Rail rail)
        {
            bool[] newConnections = rail.getConnections();
            
            newConnections[direction] = !newConnections[direction];

            if (newConnections == new bool[] { false, false, false, false, false, false, false, false })
            {
                GameEngine.GetInstance().DeleteRail(rail);
                return;
            }
            
            rail.setConnections(newConnections);
            return;
        }

        if (clickable is Tile tile)
        {
            Debug.Log(direction);
            GameEngine.GetInstance().TileToRail(tile, new Directions[]{(Directions)direction});
        }
        
    }

    public void SetStationName(string stationName)
    {
        if (clickable is Station station)
        {
            station.name = stationName;
        }
    }
}
