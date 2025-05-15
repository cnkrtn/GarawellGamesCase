using UnityEngine;

namespace Core.GridService.Data
{
    public struct GridSettings
    {
        // public int     size;          // 5×5
        public int width;
        public int height;
        public float spacing; // 1f (logical)
        public Vector3 origin; // world origin
        public float visualScale; // 0.8f
    }
}