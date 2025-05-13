using System.Collections.Generic;
using Core.GridService.Data;
using UnityEngine;
using UnityEngine.Serialization;

namespace Core.GridService.Data

{
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(menuName = "Game/LevelData")]
    public class LevelData : ScriptableObject
    {
        [Header("Which 1×1 cells start filled (0..N-2)")]
        public List<Vector2Int> closedCells;

        
    }

}