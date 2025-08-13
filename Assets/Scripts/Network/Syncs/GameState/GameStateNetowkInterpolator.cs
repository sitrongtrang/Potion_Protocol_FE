using System;
using System.Collections.Generic;
using System.Linq;
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
            if ((inInitializing && update.ServerSequence < _serverSequence) || (Mathf.Abs(update.ServerSequence - _serverSequence) > 1))
            {
                _serverSequence = update.ServerSequence - 1;
                _buffer.SetMinTickToKeep(_serverSequence);
                _buffer.Clear();
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

                // var station = CreateEntityMap(
                //     update.StationStates,
                //     s => s.StationId,
                //     s => s.StationType,
                //     s => new Vector2(s.PositionX, s.PositionY)
                // );

                var requiredRecipe = update.RequiredRecipeIds.ToList();

                Dictionary<string, int> score = new();
                Dictionary<string, string[]> inventory = new();
                Dictionary<string, int[]> inventoryIndices = new();

                foreach (var player in update.PlayerStates)
                {
                    if (score.ContainsKey(player.PlayerId))
                    {
                        score[player.PlayerId] = player.Score;
                    }
                    else
                    {
                        score.Add(player.PlayerId, player.Score);
                    }

                    if (inventory.ContainsKey(player.PlayerId))
                    {
                        inventory[player.PlayerId] = player.InventoryItemTypes;
                    }
                    else
                    {
                        inventory.Add(player.PlayerId, player.InventoryItemTypes);
                    }

                    if (inventoryIndices.ContainsKey(player.PlayerId))
                    {
                        inventoryIndices[player.PlayerId] = player.InventoryItemIndicies;
                    }
                    else
                    {
                        inventoryIndices.Add(player.PlayerId, player.InventoryItemIndicies);
                    }
                }

                _buffer.Add(new GameStateInterpolateData()
                {
                    ItemIds = item,
                    EnemyIds = enemy,
                    ItemSourceIds = itemSource,
                    // StationIds = station,
                    RequiredRecipeIds = requiredRecipe,
                    PlayerScores = score,
                    PlayerInventories = inventory,
                    PlayerInventoriesIndices = inventoryIndices,
                    ServerSequence = update.ServerSequence,
                    TimeLeft = update.CurrentGameTime
                });
            
            }
        }
    }
    public void IncrementAndInterpolate(Action<GameStateInterpolateData> applyState, Func<bool> notInAcceptingThreshold = null)
    {
        if (_serverSequence == int.MaxValue) return;
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