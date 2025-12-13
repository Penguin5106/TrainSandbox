using UnityEngine;

public class Station : Rail
{
    private string Stationname;
    public Station(Directions[] connections, string name) : base(connections)
    {
        this.name = name;
    }

    public void SetName(string stationname)
    {
        this.Stationname =  stationname;
    }

    public string GetName()
    {
        return Stationname;
    }

    public override void Click()
    {
        UIManager uiManager = UIManager.GetInstance();
        uiManager.SetUIPanelActive(UIPanel.Station);
        uiManager.SetStationText(Stationname);
    }
}
