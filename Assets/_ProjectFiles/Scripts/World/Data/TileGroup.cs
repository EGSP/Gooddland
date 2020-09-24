using System;
using System.Collections.Generic;

namespace World.Data
{
    /// <summary>
    /// Класс группировки тайлов по определенному признаку.
    /// </summary>
    public class TileGroup
    {
        public readonly string Name;
        public TileGroup(Func<Tile<TileWorldData>, bool> tileValidateFunction,
            string name)
        {
            Tiles = new List<Tile<TileWorldData>>();

            TileValidateFunction = tileValidateFunction;

            Name = name;
        }
        
        /// <summary>
        /// Тайлы, входящие в группу.
        /// </summary>
        public List<Tile<TileWorldData>> Tiles { get; protected set; }

        /// <summary>
        /// Функция проверки тайла на подходимость к группе.
        /// </summary>
        public Func<Tile<TileWorldData>,bool> TileValidateFunction { get; protected set; }

        /// <summary>
        /// Группирует тайлы, записывая неподошедших в открытый список.
        /// Открытый список всегда должен иметь стартовый элемент.
        /// Неподошедшими тайлами будут граничащие с данной группой тайлы.
        /// </summary>
        public void Group(Tile<TileWorldData> entry, int[,] checkMap)
        {
            var openSet = new List<Tile<TileWorldData>>();
            openSet.Add(entry);
            
            var closedSet = new List<Tile<TileWorldData>>();
            
            var exc = 0;
            // Проходимся по всем тайлам
            for (var i = 0; i < openSet.Count; i++)
            {
                if(++exc > checkMap.Length)
                    throw new StackOverflowException();
                
                var openTile = openSet[i];
                if (TileValidateFunction(openTile))
                {
                    // Тайл подходит группе
                    Tiles.Add(openTile);
                    checkMap[openTile.X, openTile.Y] = 1;
                
                    openSet.Remove(openTile);
                    i--;
                    closedSet.Add(openTile);
                
                    var neighboours = GetNeighbours(openTile);
                
                    for (var n = 0; n < neighboours.Count; n++)
                    {
                        // Если тайл не был рассмотрен ранее.
                        if (closedSet.Contains(neighboours[n]) == false &&
                            openSet.Contains(neighboours[n]) == false)
                            openSet.Add(neighboours[n]);
                    }
                }
            }
        }

        /// <summary>
        /// Группирует тайлы, записывая неподошедших в открытый список.
        /// Открытый список всегда должен иметь стартовый элемент.
        /// Неподошедшими тайлами будут граничащие с данной группой тайлы.
        /// </summary>
        public void GroupBy(Tile<TileWorldData> start, int[,] checkMap,
            Func<Tile<TileWorldData>,bool> validator)
        {
            // Временно сохраняем предыдущий метод проверки.
            var tempValidator = TileValidateFunction;
            TileValidateFunction = validator;
            
            Group(start, checkMap);

            TileValidateFunction = tempValidator;
            return;
        }

        /// <summary>
        /// Получение списка не null соседей и прошедших проверку TileValidateFunction
        /// </summary>
        private List<Tile<TileWorldData>> GetNeighbours(Tile<TileWorldData> tile)
        {
            var neighboursList = new List<Tile<TileWorldData>>();
            
            if(tile.Up != null && TileValidateFunction(tile.Up))
                neighboursList.Add(tile.Up);
            
            if(tile.Right != null && TileValidateFunction(tile.Right))
                neighboursList.Add(tile.Right);
            
            if(tile.Down != null && TileValidateFunction(tile.Down))
                neighboursList.Add(tile.Down);
            
            if(tile.Left != null && TileValidateFunction(tile.Left))
                neighboursList.Add((tile.Left));

            return neighboursList;
        }

        /// <summary>
        /// Создает новый экземпляр группы с такими же настройками, но с пустыми данными.
        /// </summary>
        public TileGroup CreateTemplate()
        {
            var template = new TileGroup(TileValidateFunction, Name);

            return template;
        }
    }
}