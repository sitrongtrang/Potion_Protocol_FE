using System;

public interface IBasicState<TOwner>
    where TOwner : class
{
    void Enter(TOwner owner, object[] enterParameters = null);
    void Execute(TOwner owner);
    void Exit(TOwner owner);        
}