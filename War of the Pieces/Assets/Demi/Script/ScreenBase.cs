using UnityEngine;

public class ScreenBase : MonoBehaviour
{
    public virtual void Show()
    {
        gameObject.SetActive(true);
    }

    public virtual void Hide()
    {
        gameObject.SetActive(false);
    }

    // ‰æ–Ê•\Ž¦Žž‚ÉŒÄ‚Î‚ê‚é
    public virtual void OnOpen() { }

    // ‰æ–Ê•Â‚¶‚éŽž
    public virtual void OnClose() { }
}