using UnityEngine;

namespace Gasanov.Eppd.Data.Realizations
{
    public class TransformData: DataBlock
    {
        public Transform Transform;
        
        public override object Clone()
        {
            throw new System.NotImplementedException();
        }
    }
}