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

    //追加
    public void AddPiece(int owner, PieceData piece)
    {
        if (piece == null) return;

        GetReserve(owner).Add(piece);
        OnReserveChanged?.Invoke();
    }

    //指定削除
    public void RemovePiece(int owner, int index)
    {
        var reserve = GetReserve(owner);
        if (index < 0 || index >= reserve.Count) return;

        reserve.RemoveAt(index);
        OnReserveChanged?.Invoke();
    }
    //race指定ランダム削除
    public void RemoveRandomPieceByRace(int owner, PieceRace race)
    {
        var reserve = GetReserve(owner);

        List<int> candidates = new List<int>();

        for (int i = 0; i < reserve.Count; i++)
        {
            if (reserve[i].race == race)
                candidates.Add(i);
        }

        if (candidates.Count == 0) return;

        int rand = UnityEngine.Random.Range(0, candidates.Count);
        RemovePiece(owner, candidates[rand]);
    }

    //attribute指定ランダム削除
    public void RemoveRandomPieceByAttribute(int owner, PieceAttribute attribute)
    {
        var reserve = GetReserve(owner);

        List<int> candidates = new List<int>();

        for (int i = 0; i < reserve.Count; i++)
        {
            if (reserve[i].attribute == attribute)
                candidates.Add(i);
        }

        if (candidates.Count == 0) return;

        int rand = UnityEngine.Random.Range(0, candidates.Count);
        RemovePiece(owner, candidates[rand]);
    }

    //ランダム削除
    public void RemoveRandomPiece(int owner)
    {
        var reserve = GetReserve(owner);
        if (reserve.Count == 0) return;

        int index = UnityEngine.Random.Range(0, reserve.Count);
        RemovePiece(owner, index);
    }
}