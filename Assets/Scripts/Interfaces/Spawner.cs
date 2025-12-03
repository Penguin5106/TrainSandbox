using UnityEngine;

public interface ISpawner
{
    GameObject Spawn(GameObject Caller);
}
public class Spawner : MonoBehaviour , ISpawner
{

    [SerializeField] private GameObject PrefabToSpawn;

    public virtual GameObject Spawn(GameObject Caller)
    {
        return Instantiate(PrefabToSpawn);
    }
}