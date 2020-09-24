using System;
using Gasanov.Utils.GameObjectUtilities;
using Gasanov.Utils.MeshUtilities;
using Sirenix.OdinInspector;
using UnityEngine;
using Gasanov.Extensions.Collections;

namespace World.Generators
{
    /// <summary>
    /// Класс отрисовки визуальной части мира
    /// </summary>
    public class IsoVisualGenerator : MonoBehaviour
    {
        [TitleGroup("Cell settings")]
        [BoxGroup("Cell settings/Cell size")] [Range(0,2)]
        [SerializeField] private float tileSizeHorizontal;
        [BoxGroup("Cell settings/Cell size")] [Range(0,2)]
        [SerializeField] private float tileSizeVertical;

        [TitleGroup("Visual settings")]
        [BoxGroup("Visual settings/Materials")]
        [SerializeField] private Material tileMaterial;
        
        /// <summary>
        /// Родителькский объект для всех тайлов
        /// </summary>
        private GameObject tileParent;
        
        /// <summary>
        /// Массив тайлов.
        /// </summary>
        public Grid<RendererObject> TileGridVisual { get; private set; }

        /// <summary>
        /// Устанавливает систему координат. В зависимости от размера тайлов.
        /// </summary>
        public void SetIsoBasis()
        {
            IsoBasis.SetCellSize(tileSizeHorizontal,tileSizeVertical);
        }
        
        /// <summary>
        /// Генерация меша.
        /// </summary>
        /// <param name="worldLength">Длина мира</param>
        /// <param name="worldWidth">Высота мира</param>
        public Grid<RendererObject> GenerateMesh(int worldLength, int worldWidth)
        {
            // Поиск и очистка родительского объекта
            var tileParentTransform = transform.Find("tileParent");
            if (tileParentTransform == null)
            {
                tileParent = new GameObject("tileParent");
                tileParent.transform.parent = transform;
            }
            else
            {
                tileParent = tileParentTransform.gameObject;
            }
            
            // Очищение потерянных объектов
            if (tileParent.transform.childCount > 0)
            {
                GameObjectUtils.DestroyAllChildrens(tileParent.transform);
            }
            
            ClearGrid();
            
            if(TileGridVisual != null)
                throw new Exception("Tile grid visual не очищен перед инициализацией");
            
            TileGridVisual = new Grid<RendererObject>(worldLength, worldWidth,
                tileSizeHorizontal, tileSizeVertical);
            
            
            if (TileGridVisual != null || TileGridVisual.Count != 0)
            {
                // Определяем базис
                SetIsoBasis();
                
                // Генерируем меш
                TileGridVisual.ForEachSet((x,y) =>
                {
                    var cell = MeshUtils.CreateDiamondMesh(tileSizeHorizontal, tileSizeVertical)
                        .TransformToRenderer(tileMaterial,"tileMesh");

                    cell.transform.parent = tileParent.transform;
                    cell.transform.position = IsoBasis.GetIsoVector(x, y);

                    return cell;
                });
            }

            return TileGridVisual;
        }
        
        private void ClearGrid()
        {
            if (TileGridVisual != null && TileGridVisual.Count != 0)
            {
                TileGridVisual.ForEach((x,y,obj) =>
                {
                    GameObjectUtils.SafeDestroy(obj);
                });
            }

            TileGridVisual = null;
        }
        
    }
}