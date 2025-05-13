using System;
using Core.GridService.Data;

namespace Tile
{
    public static class EventService
    {
        public static Action<TileDrag> TilePlaced;
        public static Action<int> ScoreUpdated;
        public static Action<int> ComboUpdated;
        public static Action GameOver;
        public static Action LineCleared;
        public static Action<int> ExpUpdated;
        public static Action LevelFinished;
        public static Action<Point, int> SquareCompleted;
    }
}