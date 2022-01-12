using UnityEngine;

namespace ExtensionMethods
{
    public static class ExtensionMethods
    {
        public static Vector3 ToVector3(this Vector2 vector2)
        {
            return new Vector3(vector2.x, vector2.y, 0f);
        }

        public static Vector2Int ToVector2Int(this Vector2 vector2)
        {
            return new Vector2Int((int) vector2.x, (int) vector2.y);
        }
    }
}
