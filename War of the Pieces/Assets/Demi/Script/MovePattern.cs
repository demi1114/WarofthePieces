using System.Collections.Generic;
using UnityEngine;
public abstract class MovePattern : ScriptableObject
{
    public abstract List<Vector2Int> GetMoves(
        Vector2Int currentPos,
        int owner,
        int boardSize
    );
}
