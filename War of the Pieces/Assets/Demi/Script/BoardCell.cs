using UnityEngine;

public class BoardCell : MonoBehaviour
{
    public Vector2Int GridPosition { get; private set; }

    public void Init(int x, int y)
    {
        GridPosition = new Vector2Int(x, y);
        name = $"Cell_{x}_{y}";
    }
}
