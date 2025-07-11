using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OreSpawner : MonoBehaviour
{
    private OreConfig[] _oreConfigs;
    private int[] _maxCap;
    private int[] _currentAmount;
    public void Initialize(List<LevelConfig.OrePerMap> orePerMap)
    {
        _currentAmount = new int[orePerMap.Count];
        _oreConfigs = orePerMap.Select(x => x.Ore).ToArray();
        _maxCap = orePerMap.Select(x => x.MaxAmount).ToArray();
    }

    public int GetRandomAvailableOreIndex()
    {
        int result = -1;
        int count = 0;
        
        for (int i = 0; i < _currentAmount.Length; i++)
        {
            if (_currentAmount[i] < _maxCap[i])
            {
                count++;
                // Randomly replace the result with probability 1/count
                if (UnityEngine.Random.Range(0, count) == 0)
                {
                    result = i;
                }
            }
        }
        
        return result;
    }
}