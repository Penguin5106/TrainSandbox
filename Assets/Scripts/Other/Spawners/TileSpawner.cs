using UnityEngine;

public class TileSpawner : Spawner
{
    public override GameObject Spawn(GameObject Caller)
    {
        GameObject tile = base.Spawn(Caller);



        return tile;
    }
}
