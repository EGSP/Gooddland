using Gasanov.Core;
using UnityEngine;
using World.Data;
using World.Entities;
using World.Presenters;

namespace Game
{
    public class GameSession : Singleton<GameSession>
    {
        public void InitializeSession()
        {
            var islandObject = new GameObject("Island");
            var islandEntity = islandObject.AddComponent<IslandEntity>();
            islandEntity.IslandData = GameMaster.PlayerIsland.Clone() as Island;
            
            IslandPresenter.Instance.SetModel(islandEntity);
        }
    }
}