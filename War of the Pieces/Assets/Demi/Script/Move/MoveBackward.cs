using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MovePattern/Backward")]
public class MoveBackward : MovePattern
{
    public override List<Vector2Int> GetMoves(Vector2Int currentPos, int owner, int boardSize)
    {
        List<Vector2Int> moves = new List<Vector2Int>();

        int dir = owner == 0 ? -1 : 1;
        int newY = currentPos.y + dir;

        if (newY >= 0 && newY < boardSize)
            moves.Add(new Vector2Int(currentPos.x, newY));

        return moves;
    }
}