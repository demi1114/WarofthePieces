using UnityEngine;

public class DeckSelectUI : MonoBehaviour
{
    public DeckSelectSlot[] slots;

    void Start()
    {
        Refresh();
    }

    public void Refresh()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].Setup(i);
        }
    }
}