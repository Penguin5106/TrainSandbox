using UnityEngine;

public class StationSpawner : RailSpawner
{
    public override GameObject Spawn(GameObject Caller)
    {
        GameObject station = base.Spawn(Caller);



        return station;
    }
}
