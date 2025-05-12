using UnityEngine;

namespace Core.GridService.Data
{
    public class Point
    {
        public int X, Y;
        public Vector2 WorldPos;
        public bool IsFilledColor;
        public SpriteRenderer Renderer; 
        public Point(int x, int y, Vector2 worldPos)
        {
            X = x;
            Y = y;
            IsFilledColor = false;
            WorldPos = worldPos;
        
        }
    }
}