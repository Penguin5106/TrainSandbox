using UnityEngine;

public class RailSpawner : TileSpawner
{
    public override GameObject Spawn(GameObject Caller)
    {
        GameObject Rail = base.Spawn(Caller);

        return Rail;
    }
}
