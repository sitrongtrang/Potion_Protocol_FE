using System;
using System.Collections.Generic;
using UnityEngine;

public class GameStateNetworkInterpolator : INetworkInterpolator<GameStateInterpolateData, GameStateUpdate>
{
    private NetworkInterpolationBuffer<GameStateInterpolateData> _buffer;
    private int _serverSequence = int.MaxValue;
    public GameStateNetworkInterpolator(int size)
    {
        _buffer = new(size);
    }

    public void Store(IReadOnlyList<GameStateUpdate> updates, Func<GameStateUpdate, int> findIdx = null)
    {
        bool inInitializing = _serverSequence == int.MaxValue;
        foreach (var update in updates)
        {
            if (inInitializing)
            {
                if (update.ServerSequence < _serverSequence)
                {
                    _serverSequence = update.ServerSequence - 1;
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
                    var item = CreateEntityMap(
                        update.ItemStates,
                        s => s.ItemId,
                        s => s.ItemType,
                        s => new Vector2(s.PositionX, s.PositionY)
                    );

                    var enemy = CreateEntityMap(
                        update.EnemyStates,
                        s => s.EnemyId,
                        s => s.EnemyType,
                        s => new Vector2(s.PositionX, s.PositionY)
                    );

                    var itemSource = CreateEntityMap(
                        update.OreStates,
                        s => s.OreId,
                        s => s.OreType,
                        s => new Vector2(s.PositionX, s.PositionY)
                    );

                    foreach (var plant in update.PlantStates)
                    {
                        itemSource[plant.PlantId] = new GameStateInterpolateData.EntityInfo
                        {
                            TypeId = plant.PlantType,
                            Position = new Vector2(plant.PositionX, plant.PositionY)
                        };
                    }

                    var station = CreateEntityMap(
                        update.StationStates,
                        s => s.StationId,
                        s => s.StationType,
                        s => new Vector2(s.PositionX, s.PositionY)
                    );

                    _buffer.Add(new GameStateInterpolateData()
                    {
                        ItemIds = item,
                        EnemyIds = enemy,
                        ItemSourceIds = itemSource,
                        StationIds = station
                    });
                }
            }
        }
    }
    public void IncrementAndInterpolate(Action<GameStateInterpolateData> applyState, Func<bool> notInAcceptingThreshold = null)
    {
        _serverSequence += 1;
        _buffer.SetMinTickToKeep(_serverSequence);
        if (_buffer.Poll(_serverSequence, out GameStateInterpolateData result))
        {
            applyState(result);
        }
    }

    public void Reset()
    {
        _serverSequence = int.MaxValue;
        _buffer.Clear();
    }

    private Dictionary<string, GameStateInterpolateData.EntityInfo> CreateEntityMap<T>(
    T[] states, Func<T, string> getId, Func<T, string> getType, Func<T, Vector2> getPos)
    {
        var dict = new Dictionary<string, GameStateInterpolateData.EntityInfo>(states.Length);
        foreach (var state in states)
        {
            dict.Add(getId(state), new GameStateInterpolateData.EntityInfo
            {
                TypeId = getType(state),
                Position = getPos(state)
            });
        }
        return dict;
    }
}