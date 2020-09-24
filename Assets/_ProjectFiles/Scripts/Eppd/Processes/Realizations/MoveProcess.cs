using System.Collections.Generic;
using System.Linq;
using Gasanov.Eppd.Data;
using Gasanov.Eppd.Data.Realizations;
using Gasanov.Eppd.Processors;
using Gasanov.Extensions.Linq;
using UnityEngine;

namespace Gasanov.Eppd.Processes.Realizations
{
    public class MoveProcess : ProcessBase
    {
        private TransformData _transformData;
        private MovementData _movementData;
        
        public override void Dispose()
        {
            throw new System.NotImplementedException();
        }

        public override void Process()
        {
            if (DataNullOrDisposed(_transformData, _movementData))
                return;
            
            Move();
        }

        public override void SearchData(List<DataBlock> data)
        {
            _transformData = data.FindType<TransformData>();
            _movementData = data.FindType<MovementData>();
        }

        private void Move()
        {
            var transform = _transformData.Transform;

            // Move Input
            var horizontalInput = Input.GetAxisRaw("Horizontal");
            var verticalInput = Input.GetAxisRaw("Vertical");

            // Вектор направления
            var dir = (verticalInput * transform.forward + horizontalInput * transform.right).normalized;

            transform.position += dir * _movementData.MoveSpeed * Time.deltaTime;
        }
        
    }
}