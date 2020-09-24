using UnityEngine;

namespace Gasanov.Utils.GizmosUtilities
{
    public static class GizmosUtils
    {
        public static void DrawQuad( Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3,
            Color color)
        {
            Gizmos.color = color;
            Gizmos.DrawLine(p0,p1);
            Gizmos.DrawLine(p1,p2);
            Gizmos.DrawLine(p2,p3);
            Gizmos.DrawLine(p3,p0);
        }
    }
}