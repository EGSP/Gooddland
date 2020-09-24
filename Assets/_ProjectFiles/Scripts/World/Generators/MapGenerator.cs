using System.Data.SqlTypes;
using Sirenix.OdinInspector;
using TinkerWorX.AccidentalNoiseLibrary;
using UnityEngine;
using World.Data;
using Gasanov.Extensions.Collections;

namespace World.Generators
{
    public class MapGenerator : SerializedMonoBehaviour
    {
        // Независимо от размера карты, она будет выглядеть как карта 30 на 30
        public const float SampleModifier = 0.033f; 
        
        [FoldoutGroup("Noises")] 
        [SerializeField] private double radius;
        [BoxGroup("Noises/Height noise")] 
        [SerializeField] private FractalType heightFractalType;
        [BoxGroup("Noises/Height noise")]
        [SerializeField] private int heightOctaves; // 2
        [BoxGroup("Noises/Height noise")]
        [SerializeField] private double heightFrequency; // 3
        [BoxGroup("Noises/Height noise")]
        [SerializeField] private double heightLacunarity; // 1

        [BoxGroup("Noises/Heat noise")] 
        [SerializeField] private FractalType heatFractalType;
        [BoxGroup("Noises/Heat noise")]
        [SerializeField] private int heatOctaves;
        [BoxGroup("Noises/Heat noise")]
        [SerializeField] private double heatFrequency;
        [BoxGroup("Noises/Heat noise")]
        [SerializeField] private double heatLacunarity;
        [BoxGroup("Noises/Heat noise")]
        [SerializeField][Range(-1,1)] private double contrast;
        
        [BoxGroup("Noises/Anomaly noise")] 
        [SerializeField] private FractalType anomalyFractalType; // billow 
        [BoxGroup("Noises/Anomaly noise")]
        [SerializeField] private int anomalyOctaves; // 4
        [BoxGroup("Noises/Anomaly noise")]
        [SerializeField] private double anomalyFrequency; // 3.26
        [BoxGroup("Noises/Anomaly noise")]
        [SerializeField] private double anomalyLacunarity; // 7.1

        
        /// <summary>
        /// Карта высот.
        /// </summary>
        private Grid<float> heightMap;

        /// <summary>
        /// Карта температуры.
        /// </summary>
        private Grid<float> heatMap;

        /// <summary>
        /// Карта аномалий.
        /// </summary>
        private Grid<float> anomalyMap;

        public Grid<float> GenerateHeightMap(int mapLength, int mapWidth, int seed = -1)
        {
            if (seed == -1)
                seed = Random.Range(0, int.MaxValue);
            
            var lengthSample = mapLength * SampleModifier;
            var widthSample = mapWidth * SampleModifier;
            
            heightMap = new Grid<float>(mapLength, mapWidth);
            
            var heightGenerator = new ImplicitFractal(
                heightFractalType, 
                BasisType.Simplex, 
                InterpolationType.Quintic,
                heightOctaves, heightFrequency, heightLacunarity){Seed = seed};

            heightMap.ForEachSet((x,y) =>
            {
                // Координаты шума будут не рядом, а на расстоянии (1/worldXXX)
                var xSample = x * lengthSample / (float) mapLength;
                var ySample = y * lengthSample/ (float) mapWidth;

                var height = (float)heightGenerator.Get(xSample, ySample);

                // Перевод из -1 -- 1 в 0 -- 1 
                height = height * 0.5f + 0.5f;
                
                return height;
            });

            return heightMap;
        }

        public Grid<float> GenerateHeatMap(int mapLength, int mapWidth, int seed = -1)
        {
            if (seed == -1)
                seed = Random.Range(0, int.MaxValue);
            
            var lengthSample = mapLength * SampleModifier;
            var widthSample = mapWidth * SampleModifier;
            
            heatMap = new Grid<float>(mapLength, mapWidth);

            var gradientGenerator = new ImplicitGradient(1, 1, 0, 1,
                1, 1, 1, 1,
                1, 1, 1, 1){Seed = seed};
            
            var fractalGenerator = new ImplicitFractal(heatFractalType, BasisType.Simplex,
                InterpolationType.Quintic, heatOctaves, heatFrequency,heatLacunarity){Seed = seed};
            
            var combiner = new ImplicitCombiner(CombinerType.Multiply);
            combiner.AddSource(gradientGenerator);
            combiner.AddSource(fractalGenerator);
            
            heatMap.ForEachSet((x,y) =>
            {
                // Сэмплируем шум с небольшими интервалами
                // Координаты шума будут не рядом, а на расстоянии (1/worldXXX)
                var s = x * lengthSample / (float) mapLength;
                var t = y * widthSample / (float) mapWidth;

                float nx, ny, nz, nw;
                SphereCoordinates(out nx, out ny, out nz, out nw, s, t);

                var heat = (float) combiner.Get(nx,ny,nz, nw);
                
                heat = heat * 0.5f + 0.5f;
                
                var newHeat = (float)Contrast(heat, contrast);
                
                return newHeat;
            });

            return heatMap;
        }

        public Grid<float> GenerateAnomalyMap(int mapLength, int mapWidth, int seed = -1)
        {
            if (seed == -1)
                seed = Random.Range(0, int.MaxValue);
            
            var lengthSample = mapLength * SampleModifier;
            var widthSample = mapWidth * SampleModifier;
            
            anomalyMap = new Grid<float>(mapLength,mapWidth);

            var anomalyGenerator = new ImplicitFractal(
                anomalyFractalType, 
                BasisType.Simplex, 
                InterpolationType.Quintic,
                anomalyOctaves, anomalyFrequency, anomalyLacunarity){Seed = seed};

            var secondaryGenerator = new ImplicitFractal(
                FractalType.FractionalBrownianMotion,
                BasisType.Simplex,
                InterpolationType.Quintic,
                2,anomalyFrequency*0.7D,.9f){Seed = seed};
            
            anomalyMap.ForEachSet((x,y) =>
            {
                // Сэмплируем шум с небольшими интервалами
                // Координаты шума будут не рядом, а на расстоянии (1/worldXXX)
                var s = x * lengthSample / (float) mapLength;
                var t = y * widthSample/ (float) mapWidth;
                
                float nx, ny, nz, nw;
                SphereCoordinates(out nx, out ny, out nz, out nw, s, t);

                var value = (float) anomalyGenerator.Get(nx, ny, nz, nw);
                value = value * 0.5f + 0.5f;

                var secondaryValue = (float) secondaryGenerator.Get(s,t);
                secondaryValue = secondaryValue * 0.5f + 0.5f;

                return value * secondaryValue;
            });

            return anomalyMap;
        }
        
        public Grid<float> GenerateExtraMap(int mapLength, int mapWidth, int seed = -1)
        {
            if (seed == -1)
                seed = Random.Range(0, int.MaxValue);
            
            var lengthSample = mapLength * SampleModifier;
            var widthSample = mapWidth * SampleModifier;
            
            heightMap = new Grid<float>(mapLength, mapWidth);
            
            var heightGenerator = new ImplicitFractal(
                heightFractalType, 
                BasisType.Simplex, 
                InterpolationType.Quintic,
                2, 2, 0.45D){Seed = seed};

            heightMap.ForEachSet((x,y) =>
            {
                // Координаты шума будут не рядом, а на расстоянии (1/worldXXX)
                var xSample = x * lengthSample / (float) mapLength;
                var ySample = y * lengthSample/ (float) mapWidth;

                var height = (float)heightGenerator.Get(xSample, ySample);

                // Перевод из -1 -- 1 в 0 -- 1 
                height = height * 0.5f + 0.5f;
                
                return height;
            });

            return heightMap;
        }

        /// <summary>
        /// Добавляет сферическую маску к сетке чисел.
        /// Возвращает ссылку на source.
        /// </summary>
        public Grid<float> ApplySphereMask(Grid<float> source)
        {
            var sphereMask = new ImplicitSphere(.5f,.5f){Radius = radius};
            source.ForEach((x,y, sourceValue) =>
            {
                var xSample = x / (float) source.Width;
                var ySample = y / (float) source.Height;
                
                source[x,y] *= (float)sphereMask.Get(xSample, ySample);
            });

            return source;
        }
        
        /// <summary>
        /// Сворачивает координаты по двум осям.
        /// </summary>
        private void SphereCoordinates(out float sx, out float sy, out float sz, out float sw,
            float sSample, float tSample)
        {
            //Пределы шума
            float x1 = 0, x2 = 1;
            float y1 = 0, y2 = 1;               
            var dx = (x2 - x1);
            var dy = (y2 - y1);
            
            // Вычисляем четырехмерные координаты
            sx = x1 + Mathf.Cos (sSample*2*Mathf.PI) * dx/(2*Mathf.PI);
            sy = y1 + Mathf.Cos (tSample*2*Mathf.PI) * dy/(2*Mathf.PI);
            sz = x1 + Mathf.Sin (sSample*2*Mathf.PI) * dx/(2*Mathf.PI);
            sw = y1 + Mathf.Sin (tSample*2*Mathf.PI) * dy/(2*Mathf.PI);
        }

        /// <summary>
        /// Добавляет контрастность к исходному значению.
        /// </summary>
        /// <param name="contrastValue">Значение контрастности в пределах от -1 до 1</param>
        private double Contrast(double sourceValue, double contrastValue)
        {
            var factor = (259 * (contrastValue * 255 + 255)) / (255 * (259 - contrastValue * 255));
            return (factor * (sourceValue* 255 - 128)+128)/255;
        }
    }
}