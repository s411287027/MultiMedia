using UnityEngine;

public class PersistentMusic : MonoBehaviour
{
    // 用來記錄當前場景中是否已經有背景音樂存在
    private static PersistentMusic instance;

    void Awake()
    {
        // 如果目前還沒有背景音樂實例
        if (instance == null)
        {
            instance = this;
            // 告訴 Unity 在切換場景時，不要銷毀這個遊戲物件
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // 如果玩家退回主選單，避免產生第二個重疊的背景音樂，將多餘的銷毀
            Destroy(gameObject);
        }
    }
}