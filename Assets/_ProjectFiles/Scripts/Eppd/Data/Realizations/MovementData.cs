using UnityEngine;

namespace Gasanov.Eppd.Data.Realizations
{
    public class MovementData : DataBlock
    {
        public float MoveSpeed;

        public AreaState AreaState;

        public LayerMask LayerMask;
        
        public override object Clone()
        {
            throw new System.NotImplementedException();
        }
    }

    public enum AreaState
    {
        Ground,
        Air
    }
}