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

    public void IncrementAndInterpolate(Action<GameStateInterpolateData> applyState, Func<bool> notInAcceptingThreshold = null)
    {
        throw new NotImplementedException();
    }

    public void Reset()
    {
        throw new NotImplementedException();
    }

    public void Store(IReadOnlyList<GameStateUpdate> updates, Func<GameStateUpdate, int> findIdx)
    {
        throw new NotImplementedException();
    }
}