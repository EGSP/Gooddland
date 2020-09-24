using UnityEngine;

namespace World
{
    public class Basis
    {
        public Basis()
        {
            Matrix = new Matrix4x4();
            
            Translation = Vector3.one;
            Rotation = Quaternion.identity;
            Scale = Vector3.one;
            
            SetTRS(Translation, Rotation, Scale);
        }
        
        /// <summary>
        /// Базис пространства, к которому принадлежит актор
        /// </summary>
        public Matrix4x4 Matrix
        {
            get => _matrix;
            set => _matrix = value;
        }
        private Matrix4x4 _matrix;

        protected Vector3 Translation { get; private set; }
        protected Quaternion Rotation { get; private set; }
        protected Vector3 Scale { get; private set; }
        
    
        /// <summary>
        /// Устанавливает translation, rotation, scale базису актора 
        /// </summary>
        /// <param name="translation"></param>
        /// <param name="rotation"></param>
        /// <param name="scale"></param>
        public void SetTRS(Vector3 translation, Quaternion rotation, Vector3 scale)
        {
            Translation = translation;
            Rotation = rotation;
            Scale = scale;
        
            _matrix.SetTRS(Translation, Rotation, Scale);
        }

        public void SetBasisTranslation(Vector3 translation)
        {
            Translation = translation;
            _matrix.SetTRS(Translation, Rotation, Scale);
        }
    
        public void SetBasisRotation(Quaternion rotation)
        {
            Rotation = rotation;
            _matrix.SetTRS(Translation, Rotation,Scale);
        }

        public void SetBasisScale(Vector3 scale)
        {
            Scale = scale;
            _matrix.SetTRS(Translation,Rotation,Scale);
        }

        /// <summary>
        /// Изменяет значение по ссылке.
        /// Также возвращает значение.
        /// </summary>
        public Vector3 GetPosition(Vector3 position)
        {
           return Matrix.MultiplyVector(position);
        }
    }
}