using System.Collections;
using Grid;
using Tile;
using UnityEngine;
namespace Gameplay
{
    
   

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
            

            for (int i = 0; i < slots.Length; i++)
            {
                TileId id = TileId.U_R_2x;
                GameObject go = factory.Spawn(id, Vector2Int.zero);

                go.transform.SetParent(slots[i], false);

                var drag = go.AddComponent<TileDrag>();
                drag.Init(
                    gridBuilder, 
                    highlighter,
                    uiCam,
                    slots[i],factory,id
                );
            }
        }

    }
}