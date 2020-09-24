using System;
using System.Collections.Generic;
using Gasanov.Eppd.Data;
using Gasanov.Extensions.Primitives;
using Sirenix.Serialization;
using UnityEngine;
using World.Data;

namespace World.Data
{
    public class Biome
    {
        public Biome()
        {
            Name = "default";
        }
        
        [OdinSerialize]
        public string Name { get; set; }
        
        [OdinSerialize]
        public Vector2 HeightRange { get; set; }

        public Color TileColor = Color.magenta;

        /// <summary>
        /// Функция определения типа тайла.
        /// </summary>
        public Func<Tile<TileWorldData>, TileType> TileTypeFunction { get; set; }

        /// <summary>
        /// Получение типа тайла. Если функция не задана, то значение будет всегда TileType(0)
        /// </summary>
        public TileType GetTileType(Tile<TileWorldData> tile)
        {
            if (TileTypeFunction != null)
                return TileTypeFunction(tile);
            
            return 0;
        }
        
    }
}