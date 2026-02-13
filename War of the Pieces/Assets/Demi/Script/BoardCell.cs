using UnityEngine;

public class BoardCell : MonoBehaviour
{
    public int x;
    public int y;

    public void Init(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    private void OnMouseDown()
    {
        BoardManager.Instance.OnCellClicked(this);
    }
}
