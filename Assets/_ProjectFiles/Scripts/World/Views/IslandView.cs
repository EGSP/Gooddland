using System;
using Gasanov.Core.Mvp;
using Gasanov.Extensions.Collections;
using Gasanov.Utils.GameObjectUtilities;
using Gasanov.Utils.MeshUtilities;
using Sirenix.OdinInspector;
using UnityEngine;
using World.Data;

namespace World.Views
{
    public class IslandView : MonoBehaviour, IView
    {
        [TitleGroup("Tile settings")]
        [BoxGroup("Tile settings/Tile size")] [Range(0,2)]
        [SerializeField] private float tileSizeHorizontal;
        [BoxGroup("Tile settings/Tile size")] [Range(0,2)]
        [SerializeField] private float tileSizeVertical;

        [TitleGroup("Visual settings")]
        [BoxGroup("Visual settings/Materials")]
        [SerializeField] private Material tileMaterial;
        
        /// <summary>
        /// Родителькский объект для всех тайлов
        /// </summary>
        private GameObject tileParent;

        /// <summary>
        /// Массив тайлов в мире.
        /// </summary>
        private Grid<RendererObject> tileGridVisual;
        
        
        public void Enable()
        {
            IsoBasis.SetCellSize(tileSizeHorizontal,tileSizeVertical);
            
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
        }
        
        /// <summary>
        /// Генерация визуальной части острова.
        /// </summary>
        /// <param name="islandLength">Длина мира</param>
        /// <param name="islandWidth">Высота мира</param>
        public Grid<RendererObject> ShowIsland(Island island)
        {
            ClearGrid();
            
            if(tileGridVisual != null)
                throw new Exception("Tile grid visual не очищен перед инициализацией");
            
            tileGridVisual = new Grid<RendererObject>(island.Length, island.Width,
                tileSizeHorizontal, tileSizeVertical);
            
            
            if (tileGridVisual != null || tileGridVisual.Count != 0)
            {
                // Генерируем меш
                tileGridVisual.ForEachSet((x,y) =>
                {
                    var cell = MeshUtils.CreateDiamondMesh(tileSizeHorizontal, tileSizeVertical)
                        .TransformToRenderer(tileMaterial,"tileMesh");

                    cell.transform.parent = tileParent.transform;
                    cell.transform.position = IsoBasis.GetIsoVector(x, y);

                    return cell;
                });
            }

            return tileGridVisual;
        }
        
        /// <summary>
        /// Удаление всех визуальных элементов.
        /// </summary>
        private void ClearGrid()
        {
            if (tileGridVisual != null && tileGridVisual.Count != 0)
            {
                tileGridVisual.ForEach((x,y,obj) =>
                {
                    GameObjectUtils.SafeDestroy(obj);
                });
            }
            
            // Очищение потерянных объектов
            if (tileParent.transform.childCount > 0)
            {
                GameObjectUtils.DestroyAllChildrens(tileParent.transform);
            }

            tileGridVisual = null;
        }

        public void Disable()
        {
            ClearGrid();
        }
    }
}