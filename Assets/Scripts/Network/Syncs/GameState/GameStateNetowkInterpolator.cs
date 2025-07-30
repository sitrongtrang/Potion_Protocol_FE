using System;
using System.Collections.Generic;

public class GameStateNetworkInterpolator : INetworkInterpolator<GameStateInterpolateData, GameStateUpdate>
{
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