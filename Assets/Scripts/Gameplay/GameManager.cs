using Shape;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TileFactory factory;
    [SerializeField] private float xSpacing = 2f;

    private void Start()
    {
        // for (int i = 0; i < 3; i++)
        // {
        //     TileId id = (TileId)Random.Range(0, 25);          // 0-24
        //     Vector2Int gridPos = new Vector2Int(i, 0);
        //     factory.Spawn(id, gridPos);                       // pooled
        // }
    }
}

