using System.Collections.Generic;
using UnityEngine;

public enum PieceType
{
    Red,
    Blue,
    Green
}

public class Piece : MonoBehaviour
{
    public int owner; // 0=下プレイヤー / 1=上プレイヤー
    public PieceType type;
    private void Start()
    {
        ApplyColor();
    }
    public void ApplyColor()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer == null) return;

        switch (type)
        {
            case PieceType.Red:
                renderer.material.color = Color.red;
                break;

            case PieceType.Blue:
                renderer.material.color = Color.blue;
                break;

            case PieceType.Green:
                renderer.material.color = Color.green;
                break;
        }
    }
    public virtual List<Vector2Int> GetMovablePositions(Vector2Int currentPos, int boardSize)
    {
        List<Vector2Int> positions = new List<Vector2Int>();

        int forward = (owner == 0) ? 1 : -1;

        int newY = currentPos.y + forward;

        if (newY >= 0 && newY < boardSize)
        {
            positions.Add(new Vector2Int(currentPos.x, newY));
        }

        return positions;
    }

}
