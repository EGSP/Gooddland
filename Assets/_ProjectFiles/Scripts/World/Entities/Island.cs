using Gasanov.Eppd.Entities;
using UnityEngine;

namespace World.Entities
{
    public class Island
    {
        public Island(int x, int y)
        {
            X = x;
            Y = y;
        }
        
        /// <summary>
        /// Положение острова на карте.
        /// </summary>
        public int X { get; private set; }
        
        /// <summary>
        /// Положение острова на карте.
        /// </summary>
        public int Y { get; private set; }

        /// <summary>
        /// Модификатор аномалии этого острова.
        /// </summary>
        public int AnomalyModifier { get; set; }

        /// <summary>
        /// Получение сида карты высот острова.
        /// </summary>
        public int GetSeed()
        {
            return new Vector2(X,Y).GetHashCode();
        }
    }
}