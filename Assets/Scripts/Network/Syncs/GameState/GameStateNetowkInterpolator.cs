using System;
using System.Collections.Generic;

public class GameStateNetworkInterpolator : INetworkInterpolator<GameStateInterpolateData, GameStateUpdate>
{
    public void IncrementAndInterpolate(Action<GameStateInterpolateData> applyState, Action<bool> acceptingThreshold = null)
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