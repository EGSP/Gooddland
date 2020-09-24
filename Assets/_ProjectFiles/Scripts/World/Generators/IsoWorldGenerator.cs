using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Gasanov.Utils.GameObjectUtilities;
using Gasanov.Utils.GizmosUtilities;
using Gasanov.Utils.MeshUtilities;
using Gasanov.Extensions.Primitives;

using Sirenix.OdinInspector;
using Sirenix.Serialization;

using UnityEngine;

using World.Data;
using Gasanov.Extensions.Collections;
using Gasanov.Utils.SaveUtilities;
using TinkerWorX.AccidentalNoiseLibrary;
using Unity.Burst;
using World.Entities;
using Random = System.Random;

namespace World.Generators
{
    [RequireComponent(typeof(IsoVisualGenerator))]
    [RequireComponent(typeof(MapGenerator))]
    public class IsoWorldGenerator : SerializedMonoBehaviour
    {
        [TitleGroup("World settings")] [Range(0, 60)]
        [Tooltip("Возводится в квадрат и получается число ячеек мира.")]
        public int worldSize;
        [BoxGroup("World settings/Island size")] [Range(1, 30)]
        public int islandLength;
        [BoxGroup("World settings/Island size")] [Range(1, 30)]
        public int islandWidth;

        public List<Biome> Biomes { get; private set; }
        

        [TitleGroup("Test objects")]
        public MeshRenderer textureMesh0;
        public MeshRenderer textureMesh1;
        public MeshRenderer textureMesh2;
        public MeshRenderer textureMesh3;

        private IsoVisualGenerator isoVisualGenerator;
        private MapGenerator mapGenerator;
        
        /// <summary>
        /// Сид карты аномалей.
        /// </summary>
        private int anomalyMapSeed;

        /// <summary>
        /// Сид карты температуры.
        /// </summary>
        private int heatMapSeed;
        
        public void Init()
        {
            isoVisualGenerator = GetComponent<IsoVisualGenerator>();
            
            if(isoVisualGenerator == null)
                throw new Exception($"Нужно добавить компонент IsoVisualGenerator на объект " +
                                    $"с IsoWorldGenerator");
            
            mapGenerator = GetComponent<MapGenerator>();
            
            if(mapGenerator == null)
                throw new Exception($"Нужно добавить компонент TileGenerator на объект " +
                                    $"с IsoWorldGenerator");
            
            InitBiomes();
        }

        private void InitBiomes()
        {
            Biomes = new List<Biome>();
            Biomes.Add(new Biome()
            {
                Name = "Sea",
                HeightRange = new Vector2(0,0.1f),
                TileColor = new Color(0.09f,0.24f,0.40f),
                TileTypeFunction = (x) => TileType.Liquid
            });
            
            Biomes.Add(new Biome()
            {
                Name = "Shore",
                HeightRange = new Vector2(0.1f,0.2f),
                TileColor = new Color(0.09f,0.34f,0.60f),
                TileTypeFunction = (x) => TileType.Liquid
            });
            
            Biomes.Add(new Biome()
            {
                Name = "Land",
                HeightRange = new Vector2(0.2f,1),
                TileColor = new Color(0.44f,0.88f,0.43f),
                TileTypeFunction = (x) => TileType.Solid
            });
        }

        private List<TileGroup> CreateTileGroups()
        {
            var groupList = new List<TileGroup>();
            
            var solidGr = new TileGroup(x =>
                x.TileData.Type == TileType.Solid, "solid");
            
            var liquidGr = new TileGroup(x=>
                x.TileData.Type == TileType.Liquid, "liquid");
            
            groupList.Add(solidGr);
            groupList.Add(liquidGr);
            return groupList;
        }

        /// <summary>
        /// Ищет непроверенные тайлы
        /// </summary>
        /// <returns></returns>
        private Tile<TileWorldData> SearchCheckMap(Grid<Tile<TileWorldData>> tileMap, int[,] checkMap)
        {
            var length = checkMap.GetLength(0);
            var height = checkMap.GetLength(1);
            for (var x = 0; x < length; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    if(checkMap[x,y] != 1)
                        return tileMap[x, y];
                }
            }

            return null;
        }

        /// <summary>
        /// Получение биомов, в которые входит точка.
        /// </summary>
        private List<TBiome> CheckBiomePoint<TBiome>(List<TBiome> biomes, float point)
            where TBiome : Biome
        {
            if(point < 0)
                print($"{point} less then zero");

            if (point > 1)
                print($"{point} bigger then zero");
            
            var biomeList = new List<TBiome>();

            // Проверка на отрезке по оси Х
            for (var i = 0; i < biomes.Count; i++)
            {
                var inBounds = InLineBounds(point, biomes[i].HeightRange);
                
                if(inBounds)
                    biomeList.Add(biomes[i]);
            }

            return biomeList;
        }

        /// <summary>
        /// Получает ближайший биом по графику
        /// </summary>
        /// <returns></returns>
        private TBiome GetNearestBiome<TBiome>(List<TBiome> biomes, float point)
            where TBiome : Biome
        {
            var nearest = biomes.OrderBy(x =>
            {
                return point.ToNormalized(x.HeightRange.x, x.HeightRange.y);
            });

            if (nearest.Count() == 0)
                return biomes.First();
            
            return nearest.First();
        }
        
        public bool InLineBounds(float point, Vector2 range)
        {
            // Находится на отрезке в одномерном пространстве
            if (range.x <= point && point <= range.y)
            {
                return true;
            }

            return false;
        }
        
        // Отрисовывет тайлы. 
        private void PrintTileMap(MeshRenderer meshRenderer,Grid<Tile<TileWorldData>> tileMap)
        {
            meshRenderer.material.mainTexture = WorldTextureGenerator.TileMapToTexture(tileMap.ToArray2D(),
                (tile) =>
                {
                    return tile.TileData.Color;
                });
        }
        
        // Отрисовывает числовые сетки.
        private void PrintFloatGrid(MeshRenderer meshRenderer,Grid<float> floatGrid)
        {
            meshRenderer.material.mainTexture = WorldTextureGenerator.
                FloatGridToTexture(floatGrid, islandLength, islandWidth);
        }
        
        private void PrintFloatGrid(MeshRenderer meshRenderer,Grid<float> floatGrid,
            Color a, Color b)
        {
            meshRenderer.material.mainTexture = WorldTextureGenerator.
                FloatGridToTexture(floatGrid, islandLength, islandWidth, a,b);
        }
        

        // Отрисовывает группы тайлов.
        private void PrintGroupMap(MeshRenderer meshRenderer,List<TileGroup> groups)
        {
            meshRenderer.material.mainTexture = WorldTextureGenerator.
                TileGroupsToTexture(groups, islandLength, islandWidth);
        }
        
        

        [Button("VisualGenerator")]
        private void TestMethod()
        {
            Init();
            
            isoVisualGenerator.GenerateMesh(islandLength, islandWidth);
        }

        [Button("TileGenerator")]
        private void TestGeneratorMethod()
        {
            Init();
            
            var tileMap = new Grid<Tile<TileWorldData>>(islandLength, islandWidth);
            
            var heightMap = mapGenerator.GenerateHeightMap(islandLength, islandWidth);
            
            tileMap.ForEachSet((x,y) =>
            {
                var tile = new Tile<TileWorldData>(x,y);

                // Установка высоты
                tile.TileData.Height = heightMap[x, y];
                
                // Установка биома
                var biomeList = CheckBiomePoint(Biomes, tile.TileData.Height);
                var nearestBiome = GetNearestBiome(biomeList, tile.TileData.Height);
                tile.TileData.Color = nearestBiome.TileColor;

                return tile;
            });
            
            
            PrintTileMap(textureMesh0,tileMap);
        }

        [Button("Visual + Tile generators")]
        private void TestVisualPlusTileMap()
        {
            Init();
            
            var tileMap = new Grid<Tile<TileWorldData>>(islandLength, islandWidth);
            
            var visualGrid = isoVisualGenerator.GenerateMesh(islandLength,islandWidth);

            var heightMap = mapGenerator.ApplySphereMask(
                mapGenerator.GenerateHeightMap(islandLength, islandWidth));

            tileMap.ForEachSet((x,y) =>
            {
                var tile = new Tile<TileWorldData>(x,y);

                // Установка высоты
                tile.TileData.Height = heightMap[x, y];
                
                // Установка биома
                var biomeList = CheckBiomePoint(Biomes, tile.TileData.Height);
                var nearestBiome = GetNearestBiome(biomeList, tile.TileData.Height);
                tile.TileData.Color = nearestBiome.TileColor;
                tile.TileData.Type = nearestBiome.GetTileType(tile);

                visualGrid[x, y].GetProxy()
                    .SetColor("_Color", tile.TileData.Color)
                    .Apply();

                return tile;
            });

            // Линкуем все тайлы между собой
            tileMap = LinkedGrid<Tile<TileWorldData>>.ToLinkedGrid(tileMap);

            var checkMap = new int[tileMap.Width, tileMap.Height];
            
            var groupsTemplate = CreateTileGroups();
            var groups = new List<TileGroup>();

            Tile<TileWorldData> entryTile = null;
            var exc = 0;
            // Пока не все тайлы обработаны
            while ((entryTile = SearchCheckMap(tileMap,checkMap)) != null)
            {
                if(++exc > 1_00)
                    throw new StackOverflowException();
                
                // Группа, которой подходит входной тайл
                var groupTemplate = groupsTemplate.FirstOrDefault(x =>
                    x.TileValidateFunction(entryTile));

                if (groupTemplate == null)
                {
                    Debug.LogError("Тайл не подошел ни одной группе!");
                    break;    
                }
                
                // Создаем новую группу из шаблона
                var group = groupTemplate.CreateTemplate();
                
                // Группируем оставшиеся тайлы
                group.Group(entryTile, checkMap);
                
                groups.Add(group);
            }
            
            PrintGroupMap(textureMesh0,groups);
            
            var heatMap = mapGenerator.GenerateHeatMap(islandLength, islandWidth);
            
        }

        [Button("Anomaly map generator")]
        private void TestGenerateAnomalyMap()
        {
            Init();
            // Для тестов используются размеры острова, а не мира
            var anomalyMap = mapGenerator.GenerateAnomalyMap(worldSize, worldSize);
            
            PrintFloatGrid(textureMesh0, anomalyMap, 
                new Color(0.031f, 0.109f, 0.082f), new Color(1, 1, 0.247f) );
        }

        [Button("Temperature map generator")]
        private void TestGenerateTemperatureMap()
        {
            Init();

            var temperatureMap = mapGenerator.GenerateHeatMap(worldSize, worldSize);
            
            PrintFloatGrid(textureMesh1, temperatureMap,
                new Color(0.105f, 0.113f, 0.160f), new Color(0.615f, 0.376f, 0.125f) );
        }
        

        [Button("Extra map generator")]
        private void TestGenerateExtraMap()
        {
            Init();

            var extraMap = mapGenerator.ApplySphereMask(
                mapGenerator.GenerateExtraMap(islandLength, islandWidth));
            
            PrintFloatGrid(textureMesh2, extraMap);
        }
        
        [Button("Height map generator")]
        private void TestGenerateHeightMap()
        {
            Init();

            var heightMap = mapGenerator.ApplySphereMask(
                mapGenerator.GenerateHeightMap(islandLength, islandWidth));
            
            PrintFloatGrid(textureMesh3, heightMap);
        }

        [Button("Generate island info")]
        private void TestGenerateIslandInfo(int x, int y)
        {
            if(Mathf.Abs(x)>worldSize || Mathf.Abs(y)>worldSize)
                throw new Exception($"Island out of world bounds! {x}:{y}");
            
            Init();
            
            var island = new Island(x,y);
            var seed = island.GetSeed();

            var anomalyMap = mapGenerator.GenerateAnomalyMap(worldSize, worldSize,
                anomalyMapSeed);

            var temperatureMap = mapGenerator.GenerateHeatMap(worldSize, worldSize,
                heatMapSeed);
            
            var heightMap = mapGenerator.ApplySphereMask(
                mapGenerator.GenerateHeightMap(islandLength, islandWidth, seed+anomalyMapSeed));
            
            var extraMap = mapGenerator.ApplySphereMask(
                mapGenerator.GenerateExtraMap(islandLength, islandWidth, seed+anomalyMapSeed));
            
            PrintFloatGrid(textureMesh0, anomalyMap, 
                new Color(0.031f, 0.109f, 0.082f), new Color(1, 1, 0.247f) );
            PrintFloatGrid(textureMesh1, temperatureMap,
                new Color(0.105f, 0.113f, 0.160f), new Color(0.615f, 0.376f, 0.125f) );
            PrintFloatGrid(textureMesh2, extraMap);
            PrintFloatGrid(textureMesh3, heightMap);
        }
        
        [Button("Load properties")]
        private void LoadProperties()
        {
            var pfp = SaveUtils.GetProxy("World/config");

            pfp.GetInt("anomalyMapSeed", ref anomalyMapSeed)
                .GetInt("heatMapSeed", ref heatMapSeed);
        }
        
        private void OnDrawGizmosSelected()
        {
            
        }
    }

}