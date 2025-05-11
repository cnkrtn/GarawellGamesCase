using System.Collections;
using Grid;

namespace Gameplay
{
    using UnityEngine;
    using Shape;

    public class HandDealer : MonoBehaviour
    {
        [Header("Refs")] [SerializeField] private TileFactory factory; // scene object
        [SerializeField] private GridBuilder gridBuilder; // from GridBuilder
        [SerializeField] private GridHighlighter highlighter; // scene object
        [SerializeField] private Camera uiCam; // UI / main camera
        [SerializeField] private Transform[] slots; // size = 3
        
        private IEnumerator Start()
        {
            yield return new WaitForSeconds(4);
            Deal();
        }

        private void Deal()
        {
            int enumCount = System.Enum.GetValues(typeof(TileId)).Length;

            for (int i = 0; i < slots.Length; i++)
            {
                TileId id = (TileId)24;
                GameObject go = factory.Spawn(id, Vector2Int.zero);

                go.transform.SetParent(slots[i], false);

                var drag = go.AddComponent<TileDrag>();
                drag.Init(
                    gridBuilder, // now pass the entire builder
                    highlighter,
                    uiCam,
                    slots[i]
                );
            }
        }

    }
}