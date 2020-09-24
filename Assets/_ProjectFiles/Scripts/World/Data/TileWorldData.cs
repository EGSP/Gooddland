using Gasanov.Eppd.Data;
using UnityEngine;

namespace World.Data
{
    public class TileWorldData : DataBlock
    {
        /// <summary>
        /// Тип тайла. Жидкость или твердая поверхность.
        /// </summary>
        public TileType Type { get; set; }
        
        /// <summary>
        /// Биом, в котором находится тайл.
        /// </summary>
        public Biome ParentBiome { get; set; }
        
        /// <summary>
        /// Высота поверхности тайла.
        /// </summary>
        public float Height { get; set; }

        public Color Color { get; set; }
        
       
        
        public override object Clone()
        {
            throw new System.NotImplementedException();
        }

        public override string ToString()
        {
            return $"{Type}";
        }
    }

    public enum TileType
    {
        Solid = 0,
        Liquid = 1
    }
}