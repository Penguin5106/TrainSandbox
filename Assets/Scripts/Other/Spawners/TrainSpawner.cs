using UnityEngine;

public class TrainSpawner : Spawner
{
    public override GameObject Spawn(GameObject Caller)
    {
        GameObject train = base.Spawn(Caller);



        return train;
    }
}
