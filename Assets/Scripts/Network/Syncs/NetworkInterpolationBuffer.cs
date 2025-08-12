using System;
using System.Collections.Generic;

public class NetworkInterpolationBuffer<TServerState>
    where TServerState : IServerStateSnapshot, IComparable<TServerState>
{
    private readonly SortedList<int, TServerState> _serverStateBuffer;
    private readonly int _capacity;

    private int _minTickToKeep = int.MinValue;
    public int Capacity => _capacity;

    // DEBUG
    public int? OldestTick => _serverStateBuffer.Count > 0 ? _serverStateBuffer.Keys[0] : (int?)null;
    public int? LatestTick => _serverStateBuffer.Count > 0 ? _serverStateBuffer.Keys[^1] : (int?)null;

    public NetworkInterpolationBuffer(int capacity)
    {
        _capacity = capacity;
        _serverStateBuffer = new();
    }

    public void SetMinTickToKeep(int tick)
    {
        _minTickToKeep = tick;
    }

    public void Add(TServerState serverState)
    {
        int seq = serverState.ServerSequence;

        if (_serverStateBuffer.ContainsKey(seq))
            return;

        if (_serverStateBuffer.Count >= _capacity)
        {
            int oldestTick = _serverStateBuffer.Keys[0];
            if (oldestTick < _minTickToKeep)
            {
                _serverStateBuffer.RemoveAt(0);
            }
            else
            {
                return; // Can't evict safely
            }
        }

        _serverStateBuffer.Add(seq, serverState);
    }

    public TServerState Peek()
    {
        return TryPeek(out var result) ? result : default;
    }

    public bool Poll(int expectedSequence, out TServerState result)
    {
        result = default;

        while (TryPeek(out var head))
        {
            int seq = head.ServerSequence;

            if (seq < expectedSequence)
            {
                TryPop(out _); // discard
            }
            else if (seq == expectedSequence)
            {
                TryPop(out result);
                return true;
            }
            else
            {
                break;
            }
        }

        return false;
    }

    public bool IsEmpty() => _serverStateBuffer.Count == 0;

    public void Clear() => _serverStateBuffer.Clear();

    private bool TryPeek(out TServerState result)
    {
        if (_serverStateBuffer.Count > 0)
        {
            result = _serverStateBuffer.Values[0];
            return true;
        }
        result = default;
        return false;
    }

    private bool TryPop(out TServerState result)
    {
        if (_serverStateBuffer.Count > 0)
        {
            int firstKey = _serverStateBuffer.Keys[0];
            result = _serverStateBuffer[firstKey];
            _serverStateBuffer.RemoveAt(0);
            return true;
        }
        result = default;
        return false;
    }
}
