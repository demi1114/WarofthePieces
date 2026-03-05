using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MovePattern/Left2")]
public class MoveLeft2 : MovePattern
{
    public override List<Vector2Int> GetMoves(Vector2Int currentPos, int owner, int boardSize)
    {
        List<Vector2Int> moves = new List<Vector2Int>();

        int newX = currentPos.x - 2;

        if (newX >= 0 && newX < boardSize)
        {
            moves.Add(new Vector2Int(newX, currentPos.y));
        }

        return moves;
    }
}