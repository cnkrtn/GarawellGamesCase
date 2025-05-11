using System.Collections;
using System.Linq;
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
        
        public  bool CanPlaceShape(ShapeData shape, GridBuilder builder, GridLogic logic)
        {
            int size = builder.GridSize;

            for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
            {
                // compute origin if this shape's anchor were at (x,y)
                Vector2Int origin = new Vector2Int(x, y) - shape.anchorPoint;

                // try to fetch the edges for that origin
                var edges = TryGetEdges(origin, shape, builder, logic);
                if (edges != null && edges.All(e => !e.IsFilled))
                    return true;    // found one valid spot
            }

            return false;   // no valid spot anywhere
        }

        // Mirror your TileDrag.GetEdges logic here:
        private  Edge[] TryGetEdges(Vector2Int origin, ShapeData shape,
            GridBuilder builder, GridLogic logic)
        {
            var list = new Edge[shape.edges.Count];
            for (int i = 0; i < list.Length; i++)
            {
                var ed = shape.edges[i];
                var a  = origin + ed.pointA;
                var b  = origin + ed.pointB;

                if (a.x < 0 || a.x >= builder.GridSize ||
                    a.y < 0 || a.y >= builder.GridSize ||
                    b.x < 0 || b.x >= builder.GridSize ||
                    b.y < 0 || b.y >= builder.GridSize)
                    return null;

                var pA = logic.GetPointAt(a.x, a.y);
                var pB = logic.GetPointAt(b.x, b.y);
                var re = logic.GetEdge(pA, pB);
                if (re == null) return null;

                list[i] = re;
            }
            return list;
        }

    }
}