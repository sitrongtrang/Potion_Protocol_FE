using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNetworkInterpolator : INetworkInterpolator<PlayerStateInterpolateData, GameStateUpdate>
{
    private NetworkInterpolationBuffer<PlayerStateInterpolateData> _buffer;
    private int _serverSequence = int.MaxValue;
    public PlayerNetworkInterpolator(int bufferSize)
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
                    _buffer.Add(new PlayerStateInterpolateData()
                    {
                        ServerSequence = update.ServerSequence,
                        PositionX = update.PlayerStates[idx].PositionX,
                        PositionY = update.PlayerStates[idx].PositionY,
                        IsAttacking = update.PlayerStates[idx].IsAttacking
                    });
                }
            }
        }
    }
    public void IncrementAndInterpolate(Action<PlayerStateInterpolateData> applyState, Func<bool> notInAcceptingThreshold = null)
    {
        _serverSequence += 1;
        _buffer.SetMinTickToKeep(_serverSequence);
        if (_buffer.Poll(_serverSequence, out PlayerStateInterpolateData result))
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