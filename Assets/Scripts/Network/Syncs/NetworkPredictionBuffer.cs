using System.Collections.Generic;
using UnityEngine;

public class NetworkPredictionBuffer<TInput, TState>
    where TInput : IInputSnapshot
    where TState : IStateSnapshot
{
    private readonly Queue<TInput> _inputBuffer;
    public TInput[] InputBufferAsArray => _inputBuffer.ToArray();

    private readonly Queue<TState> _stateBuffer;
    public TState[] StateBufferAsArray => _stateBuffer.ToArray();

    private int _capacity;
    private int _currentInputSequence = -1;
    public int CurrentInputSequence => _currentInputSequence;
    public NetworkPredictionBuffer(int capacity)
    {
        _capacity = capacity;
        _inputBuffer = new Queue<TInput>(capacity);
        _stateBuffer = new Queue<TState>(capacity);
    }

    public int IcrementAndGetCurrentInputSequence()
    {
        _currentInputSequence += 1;
        return _currentInputSequence;
    }

    public void EnqueueInput(TInput input)
    {
        if (_inputBuffer.Count >= _capacity)
            _inputBuffer.Dequeue();

        _inputBuffer.Enqueue(input);
    }

    public void EnqueueState(TState state)
    {
        if (_stateBuffer.Count >= _capacity)
            _stateBuffer.Dequeue();

        _stateBuffer.Enqueue(state);
    }

    public void SetCapacity(int newCapacity)
    {
        if (newCapacity > _capacity)
        {
            _capacity = newCapacity;
        }
    }

    public void ClearStateSnapshot()
    {
        _stateBuffer.Clear();
    }

    public void ClearInputSnapshot()
    {
        _inputBuffer.Clear();
    }
}
