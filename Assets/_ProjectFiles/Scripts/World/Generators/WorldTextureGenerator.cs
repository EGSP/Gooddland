using System;
using System.Collections.Generic;
using Gasanov.Eppd.Data;
using Gasanov.Extensions.Collections;
using Gasanov.Utils.RandomUtilities;
using UnityEngine;
using World.Data;

namespace World.Generators
{
    public static class WorldTextureGenerator
    {
        public static Texture2D TileMapToTexture<TDataBlock>
            (Tile<TDataBlock>[,] tileMap, Func<Tile<TDataBlock>, Color> tileColorSelector)
            where TDataBlock : DataBlock, new()
        {
            var length = tileMap.GetLength(0);
            var width = tileMap.GetLength(1);
            
            Color[] pixels;
            Texture2D texture;
            CreateTextureData(out pixels,out texture,length,width);

            for (var x = 0; x < length; x++)
            {
                for (var y = 0; y < width; y++)
                {
                    var tileValue = tileColorSelector(tileMap[x, y]);
                    pixels[x + y * length] = tileValue;
                }
            }
            
            return CompileTexture(pixels,texture);
        }
        
        /// <summary>
        /// Окрашивает текстуру в цвета на основе групп тайлов.
        /// </summary>
        public static Texture2D TileGroupsToTexture(List<TileGroup> groups, int length, int width)
        {
            Color[] pixels;
            Texture2D texture;
            CreateTextureData(out pixels,out texture,length,width);
            
            // Проход по всем группам
            for (var g = 0; g < groups.Count; g++)
            {
                var group = groups[g];
                var groupColor = RandomUtils.Color();
                
                for (var t = 0; t < group.Tiles.Count; t++)
                {
                    var tile = group.Tiles[t];
                    pixels[tile.X + tile.Y * length] = groupColor;
                }
            }

            return CompileTexture(pixels, texture);
        }

        /// <summary>
        /// Создает текстуру на основе числовых значений сетки.
        /// </summary>
        public static Texture2D FloatGridToTexture(Grid<float> grid, int length, int width)
        {
            Color[] pixels;
            Texture2D texture;
            CreateTextureData(out pixels,out texture,length,width);
            
            // Проход по всем группам
            for (var x = 0; x < length; x++)
            {
                for (var y = 0; y < width; y++)
                {
                    var value = grid[x, y];
                    pixels[x + y * width] = Color.Lerp(Color.black, Color.white, value);
                }
            }

            return CompileTexture(pixels, texture);
        }
        
        /// <summary>
        /// Создает текстуру на основе числовых значений сетки.
        /// </summary>
        public static Texture2D FloatGridToTexture(Grid<float> grid, int length, int width,
            Color a, Color b)
        {
            Color[] pixels;
            Texture2D texture;
            CreateTextureData(out pixels,out texture,length,width);
            
            // Проход по всем группам
            for (var x = 0; x < length; x++)
            {
                for (var y = 0; y < width; y++)
                {
                    var value = grid[x, y];
                    pixels[x + y * width] = Color.Lerp(a, b, value);
                }
            }

            return CompileTexture(pixels, texture);
        }

        /// <summary>
        /// Создает пустышки для формирования текстуры.
        /// </summary>
        private static void CreateTextureData(out Color[] pixels, out Texture2D texture2D, int length, int width)
        {
            pixels = new Color[length*width];

            texture2D = new Texture2D(length, width); 
        }

        /// <summary>
        /// Применяет пиксели и некоторые настройки к текстуре.
        /// </summary>
        private static Texture2D CompileTexture(Color[] pixels, Texture2D target)
        {
            target.SetPixels(pixels);
            target.wrapMode = TextureWrapMode.Clamp;
            target.filterMode = FilterMode.Point;
            target.Apply();

            return target;
        }
    }
}