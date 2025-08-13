using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyNetworkInterpolator : INetworkInterpolator<EnemyStateInterpolateData, GameStateUpdate>
{
    private NetworkInterpolationBuffer<EnemyStateInterpolateData> _buffer;
    private int _serverSequence = int.MaxValue;
    public EnemyNetworkInterpolator(int bufferSize)
    {
        _buffer = new(bufferSize);
    }
    public void Store(IReadOnlyList<GameStateUpdate> updates, Func<GameStateUpdate, int> findIdx)
    {
        bool inInitializing = _serverSequence == int.MaxValue;
        foreach (var update in updates)
        {
            int idx = findIdx(update);
            if (idx > -1)
            {
                if ((inInitializing && update.ServerSequence < _serverSequence) || (Mathf.Abs(update.ServerSequence - _serverSequence) > _buffer.Capacity))
                {
                    _serverSequence = update.ServerSequence - 1;
                    _buffer.SetMinTickToKeep(_serverSequence);
                    _buffer.Clear();
                }
                if (update.ServerSequence >= _serverSequence)
                {
                    _buffer.Add(new EnemyStateInterpolateData()
                    {
                        ServerSequence = update.ServerSequence,
                        PositionX = update.EnemyStates[idx].PositionX,
                        PositionY = update.EnemyStates[idx].PositionY,
                        Health = update.EnemyStates[idx].CurrentHealth
                    });
                }
            }
        }
    }
    public void IncrementAndInterpolate(Action<EnemyStateInterpolateData> applyState, Func<bool> notInAcceptingThreshold = null)
    {
        _serverSequence += 1;
        _buffer.SetMinTickToKeep(_serverSequence);
        if (_buffer.Poll(_serverSequence, out EnemyStateInterpolateData result))
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