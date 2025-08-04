using System;
using System.Collections.Generic;
using UnityEngine;

public class AlchemyNetworkInterpolator : INetworkInterpolator<AlchemyStateInterpolateData, GameStateUpdate>
{
    private NetworkInterpolationBuffer<AlchemyStateInterpolateData> _buffer;
    private int _serverSequence = int.MaxValue;
    public AlchemyNetworkInterpolator(int bufferSize)
    {
        _buffer = new(bufferSize);
    }
    public void Store(IReadOnlyList<GameStateUpdate> updates, Func<GameStateUpdate, int> findIdx)
    {
        bool inInitializing = _serverSequence == int.MaxValue;
        foreach (var update in updates)
        {
            AlchemyState alchemyState = update.AlchemyState;
            if (alchemyState != null)
            {
                if (inInitializing)
                {
                    if (update.ServerSequence < _serverSequence)
                    {
                        _serverSequence = update.ServerSequence - 1;
                    }
                }
                else // THIS PART IS A LITTLE IFFY
                {
                    if (update.ServerSequence - _serverSequence > _buffer.Capacity && _buffer.IsEmpty())
                    {
                        _serverSequence = update.ServerSequence - 1;
                    }
                }
                if (update.ServerSequence >= _serverSequence)
                {
                    _buffer.Add(new AlchemyStateInterpolateData()
                    {
                        ServerSequence = update.ServerSequence,
                        CraftTime = alchemyState.CraftTime,
                        CraftMaxTime = alchemyState.CraftMaxTime,
                        IsCrafting = alchemyState.IsCrafting,
                        ItemIds = alchemyState.ItemIds
                    });
                }
            }
        }
    }
    public void IncrementAndInterpolate(Action<AlchemyStateInterpolateData> applyState, Func<bool> notInAcceptingThreshold = null)
    {
        _serverSequence += 1;
        _buffer.SetMinTickToKeep(_serverSequence);
        if (_buffer.Poll(_serverSequence, out AlchemyStateInterpolateData result))
        {
            applyState(result);
        }
    }

    public void Reset()
    {
        _serverSequence = int.MaxValue;
        _buffer.Clear();
    }
}