
using System;
using Gasanov.Eppd.Data;
using Gasanov.Extensions.Collections;

namespace World.Data
{
    public abstract class Tile : DataBlock, IGridLinkable<Tile>
    {
        protected Tile(int x, int y)
        {
            X = x;
            Y = y;
        }
        
        public int X { get; protected set; }
        public int Y { get; protected set; }
        
        public Tile Up { get; set; }
        public Tile Right { get; set; }
        public Tile Down { get; set; }
        public Tile Left { get; set; }

        /// <summary>
        /// Битовая маска по наличию соседей
        /// </summary>
        public int NeighboursBitMask()
        {
            var bitmask = 0;

            if (Up != null)
                bitmask+=1;

            if (Right != null)
                bitmask+=2;

            if (Down != null)
                bitmask+=4;

            if (Left != null)
                bitmask+=8;

            return bitmask;
        }

        public override string ToString()
        {
            return $"{X:Y}";
        }
    }
    
    public class Tile<TDataBlock> : Tile, IGridLinkable<Tile<TDataBlock>> where TDataBlock : DataBlock, new()
    {
        private Tile<TDataBlock> up;
        private Tile<TDataBlock> right;
        private Tile<TDataBlock> down;
        private Tile<TDataBlock> left;

        public Tile(int x, int y) : base(x,y)
        {
            TileData = new TDataBlock();
        }

        public Tile<TDataBlock> Up
        {
            get => up;
            set
            {
                base.Up = value;
                up = value;
            }
        }
        public Tile<TDataBlock> Right
        {
            get => right;
            set
            {
                base.Right = value;
                right = value;
            }
            
        }
        public Tile<TDataBlock> Down
        {
            get => down;
            set
            {
                base.Down = value;
                down = value;
            }
        }
        public Tile<TDataBlock> Left
        {
            get => left;
            set
            {
                base.Left = value;
                left = value;
            }
        }

        /// <summary>
        /// Данные тайла
        /// </summary>
        public TDataBlock TileData { get; private set; }
        
        public override object Clone()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Битовая маска по данным соседей.
        /// Вторым аргументом идет сторона (1-up, 2-right, 3-down, 4-left).
        /// </summary>
        public int NeighboursDataBitMask(Func<Tile<TDataBlock>,int, int> bitmaskFunc)
        {
            var bitmask = 0;
            
            if (Up != null)
                bitmask+=bitmaskFunc(this,1);

            if (Right != null)
                bitmask+=bitmaskFunc(this,2);

            if (Down != null)
                bitmask+=bitmaskFunc(this,4);

            if (Left != null)
                bitmask+=bitmaskFunc(this,8);

            return bitmask;
        }
        
        public override string ToString()
        {
            return base.ToString();
        }
    }
}