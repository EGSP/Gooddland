using Gasanov.Eppd.Data;
using Gasanov.Eppd.Entities;
using UnityEngine;

namespace World.Data
{
    public class Island : DataBlock
    {
        public Island(int x, int y)
        {
            X = x;
            Y = y;

            Length = 0;
            Width = 0;
        }
        
        /// <summary>
        /// Положение острова на карте.
        /// </summary>
        public int X { get; private set; }
        
        /// <summary>
        /// Положение острова на карте.
        /// </summary>
        public int Y { get; private set; }
        
        public int Length { get; private set; }
        public int Width { get; private set; }

        /// <summary>
        /// Модификатор аномалии этого острова.
        /// </summary>
        public float AnomalyModifier { get; set; }
        
        /// <summary>
        /// Модификатор температуры этого острова.
        /// </summary>
        public float HeatModifier { get; set; }

        /// <summary>
        /// Получение сида острова.
        /// </summary>
        public int GetSeed()
        {
            return new Vector2(X,Y).GetHashCode();
        }

        /// <summary>
        /// Установка размеров острова.
        /// </summary>
        public void SetSizes(int length, int width)
        {
            if (Length == 0)
                Length = length;

            if (Width == 0)
                Width = width;
        }
        

        public override object Clone()
        {
            var clone = new Island(X,Y);
            clone.AnomalyModifier = AnomalyModifier;
            clone.HeatModifier = HeatModifier;
            clone.SetSizes(Length,Width);

            return clone;
        }
    }
}