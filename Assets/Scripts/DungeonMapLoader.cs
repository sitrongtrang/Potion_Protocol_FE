using UnityEngine;

public class DungeonMapLoader : MonoBehaviour, IAnimResourcesLoadable
{
    [SerializeField] private GameObject _fire;
    [SerializeField] private Transform[] _positionsToSpawn;
    public void Load()
    {
        for (int i = 0; i < _positionsToSpawn.Length; i++)
        {
            Instantiate(_fire, _positionsToSpawn[i]);
        }
    }
}
