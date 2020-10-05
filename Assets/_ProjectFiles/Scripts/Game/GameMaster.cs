using Gasanov.Utils.SaveUtilities;
using UnityEngine;
using World.Data;

namespace Game
{
    public static class GameMaster
    {
        static GameMaster()
        {
            
        }
        
        /// <summary>
        /// Координаты острова игрока.
        /// Создается пустышка с нулевыми данными. Заменятся при загрузке.
        /// </summary>
        public static Island PlayerIsland;

        public static bool PlayerHaveIsland => PlayerIsland != null;
        
        /// <summary>
        /// Попытка загрузить остров игрока.
        /// </summary>
        public static void LoadPlayerIsland()
        {
            var island = Storage.l.LoadEntity<Island>("/World/Player/playerIsland");

            if (island == null)
                return;

            PlayerIsland = island;
            return;
        }

        /// <summary>
        /// Установка острова игрока.
        /// </summary>
        public static void SetPlayerIsland(Island island, bool saveNewIsland = true)
        {
            PlayerIsland = island;
            
            if(saveNewIsland == true)
                SavePlayerIsland(PlayerIsland);
        }
        
        /// <summary>
        /// Сохранение острова игрока.
        /// </summary>
        public static void SavePlayerIsland(Island island)
        {
            Storage.l.SaveEntity<Island>(island,"/World/Player/playerIsland");
        }

        /// <summary>
        /// Получение следующих координат для генерации острова.
        /// </summary>
        public static Vector2Int NextIslandCoordinates(Island startIsland)
        {
            return new Vector2Int(startIsland.X + 1, startIsland.Y);
        }
    }
}