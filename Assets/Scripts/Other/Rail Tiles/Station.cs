using UnityEngine;

public class Station : Rail
{
    private string name { get; set; }
    public Station(Directions[] connections, string name) : base(connections)
    {
        this.name = name;
    }

    public void SetName(string name)
    {
        this.name =  name;
    }
}
