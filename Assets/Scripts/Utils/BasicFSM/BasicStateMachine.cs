using System;
using System.Collections.Generic;

public class BasicStateMachine<TOwner, TStateEnum>
    where TOwner : class
    where TStateEnum : Enum
{
    public IBasicState<TOwner> CurrentState { get; private set; }
    public TStateEnum CurrentStateEnum { get; private set; }
    private readonly TOwner _owner;
    private readonly Dictionary<TStateEnum, IBasicState<TOwner>> _states;

    public BasicStateMachine(TOwner owner)
    {
        _owner = owner ?? throw new ArgumentNullException(nameof(owner));
        _states = new Dictionary<TStateEnum, IBasicState<TOwner>>();
    }

    public void AddState(TStateEnum stateEnum, IBasicState<TOwner> state)
    {
        _states[stateEnum] = state;
    }

    public void ChangeState(TStateEnum newStateEnum, object[] parameters = null)
    {
        if (!_states.TryGetValue(newStateEnum, out var newState))
            throw new ArgumentException($"State {newStateEnum} not registered");

        CurrentStateEnum = newStateEnum;
        CurrentState?.Exit(_owner);
        CurrentState = newState;
        CurrentState.Enter(_owner, parameters);
    }

    public void Execute()
    {
        CurrentState?.Execute(_owner);
    }
}