using UnityEngine;

namespace Raele.Util
{
    public class IntVector2
    {
        public int x;
        public int y;

        public IntVector2(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public IntVector2(Vector2 floatVector)
        {
            this.x = (int)floatVector.x;
            this.y = (int)floatVector.y;
        }
    }
}
