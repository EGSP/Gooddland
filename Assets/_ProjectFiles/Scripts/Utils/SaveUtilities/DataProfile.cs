using System;

namespace Gasanov.Utils.SaveUtilities
{
    [Serializable]
    public sealed class DataProfile
    {
        public DataProfile(string name)
        {
            Name = name;
        }
        
        /// <summary>
        /// Имя профиля.
        /// </summary>
        public readonly string Name;
    }
}