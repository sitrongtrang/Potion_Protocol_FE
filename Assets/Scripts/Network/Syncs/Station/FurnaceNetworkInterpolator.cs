using System;
using System.Collections.Generic;
using UnityEngine;

public class FurnaceNetworkInterpolator : INetworkInterpolator<FurnaceStateInterpolateData, GameStateUpdate>
{
    private NetworkInterpolationBuffer<FurnaceStateInterpolateData> _buffer;
    private int _serverSequence = int.MaxValue;
    public FurnaceNetworkInterpolator(int bufferSize)
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
                    _buffer.Add(new FurnaceStateInterpolateData()
                    {
                        ServerSequence = update.ServerSequence,
                        CraftTime = update.StationStates[idx].CraftTime,
                        CraftMaxTime = update.StationStates[idx].CraftMaxTime,
                        IsCrafting = update.StationStates[idx].IsCrafting,
                    });
                }
            }
        }
    }
    public void IncrementAndInterpolate(Action<FurnaceStateInterpolateData> applyState, Func<bool> notInAcceptingThreshold = null)
    {
        if (_serverSequence == int.MaxValue) return;
        _serverSequence += 1;
        _buffer.SetMinTickToKeep(_serverSequence);
        if (_buffer.Poll(_serverSequence, out FurnaceStateInterpolateData result))
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