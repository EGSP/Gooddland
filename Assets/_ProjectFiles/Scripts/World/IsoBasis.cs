using UnityEngine;

namespace World
{
    public static class IsoBasis
    {
        /// <summary>
        /// Размер ячейки по горизонтали
        /// </summary>
        public static float CellHorizontalSize { get; private set; }
        
        /// <summary>
        /// Размер ячейки по вертикали
        /// </summary>
        public static float CellVerticalSize { get; private set; }
        
        /// <summary>
        /// Vector(1,0,0) in Iso
        /// </summary>
        public static Vector3 Right { get; private set; }
        
        /// <summary>
        /// Vector(0,1,0) in Iso
        /// </summary>
        public static Vector3 Up { get; private set; }
        
        /// <summary>
        /// Vector(1,1,0) in Iso
        /// </summary>
        public static Vector3 One { get; private set; }

        public static void SetCellSize(float hor, float ver)
        {
            CellHorizontalSize = hor;
            CellVerticalSize = ver;
            
            InitVectors();
        }
        
        /// <summary>
        /// Инициализация основных векторов
        /// </summary>
        private static void InitVectors()
        {
            Right = new Vector3(CellHorizontalSize/2f,CellVerticalSize/2f);
            Up = new Vector3(-CellHorizontalSize/2f, CellVerticalSize/2);
            One = Right + Up;
        }

        /// <summary>
        /// Получение вектора по двум значениям
        /// </summary>
        public static Vector3 GetIsoVector(float x, float y, float z = 0)
        {
            return x * Right + y * Up;
        }

        /// <summary>
        /// Перевод координат в изометрическую систему. Использует IsoBasis.
        /// </summary>
        public static Vector3 iso(this Vector3 vector3)
        {
            
            return GetIsoVector(vector3.x,vector3.y);
        }
        
        
    }
}