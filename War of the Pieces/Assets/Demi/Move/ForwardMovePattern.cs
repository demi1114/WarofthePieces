using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/MovePattern/Forward")]
public class ForwardMovePattern : MovePattern
{
    public override List<Vector2Int> GetMoves(
        Vector2Int currentPos,
        int owner,
        int boardSize)
    {
        List<Vector2Int> moves = new List<Vector2Int>();

        int forward = (owner == 0) ? 1 : -1;
        int newY = currentPos.y + forward;

        if (newY >= 0 && newY < boardSize)
        {
            moves.Add(new Vector2Int(currentPos.x, newY));
        }

        return moves;
    }
}
