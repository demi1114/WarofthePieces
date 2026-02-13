using UnityEngine;

public enum PieceType
{
    Soldier,
    Knight,
    Archer
}
public class Piece : MonoBehaviour
{
    public int owner; // 0=下プレイヤー / 1=上プレイヤー
    public PieceType type;
}
