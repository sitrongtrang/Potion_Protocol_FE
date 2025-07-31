using System;
using System.Collections.Generic;

public class GameStateNetworkInterpolator : INetworkInterpolator<GameStateInterpolateData, GameStateUpdate>
{
    private NetworkInterpolationBuffer<GameStateInterpolateData> _buffer;
    private int _serverSequence = int.MaxValue;
    public GameStateNetworkInterpolator(int size)
    {
        _buffer = new(size);
    }

    public void Store(IReadOnlyList<GameStateUpdate> updates, Func<GameStateUpdate, int> findIdx)
    {
        bool inInitializing = _serverSequence == int.MaxValue;
        foreach (var update in updates)
        {
            if (inInitializing)
            {
                if (update.ServerSequence < _serverSequence)
                {
                    _serverSequence = update.ServerSequence - 1;
                }
                else // THIS PART IS A LITTLE IFFY
                {
                    if (update.ServerSequence - _serverSequence > _buffer.Capacity && _buffer.IsEmpty())
                    {
                        _serverSequence = update.ServerSequence - 1;
                    }
                }
            }
        }
    }
    public void IncrementAndInterpolate(Action<GameStateInterpolateData> applyState, Func<bool> notInAcceptingThreshold = null)
    {
        
    }

    public void Reset()
    {
        
    }
}