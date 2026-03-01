using System;
using System.Collections.Generic;
using UnityEngine;

public class ReserveManager : MonoBehaviour
{
    public static ReserveManager Instance;

    private List<PieceData> playerReserve = new List<PieceData>();
    private List<PieceData> enemyReserve = new List<PieceData>();

    public event Action OnReserveChanged;

    private void Awake()
    {
        Instance = this;
    }

    public List<PieceData> GetReserve(int owner)
    {
        return owner == 0 ? playerReserve : enemyReserve;
    }

    public int GetReserveCount(int owner)
    {
        return GetReserve(owner).Count;
    }

    // ===== ’Ç‰Á =====
    public void AddPiece(int owner, PieceData piece)
    {
        if (piece == null) return;

        GetReserve(owner).Add(piece);
        OnReserveChanged?.Invoke();
    }

    // ===== w’èíœ =====
    public void RemovePiece(int owner, int index)
    {
        var reserve = GetReserve(owner);
        if (index < 0 || index >= reserve.Count) return;

        reserve.RemoveAt(index);
        OnReserveChanged?.Invoke();
    }

    // ===== ƒ‰ƒ“ƒ_ƒ€íœ =====
    public void RemoveRandomPiece(int owner)
    {
        var reserve = GetReserve(owner);
        if (reserve.Count == 0) return;

        int index = UnityEngine.Random.Range(0, reserve.Count);
        RemovePiece(owner, index);
    }
}