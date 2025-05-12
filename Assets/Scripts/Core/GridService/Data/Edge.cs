using UnityEngine;

namespace Core.GridService.Data
{
    public class Edge
    {
        public Point A, B;
        public bool IsFilled;
        public SpriteRenderer Renderer; 
        public Edge(Point a, Point b)
        {
            A = a;
            B = b;
            IsFilled = false;
        }

        public Vector2 GetCenter() => (A.WorldPos + B.WorldPos) / 2f;
    }
}