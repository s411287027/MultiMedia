using UnityEngine;

public class LevelExit : MonoBehaviour
{
    [Header("勝利設定")]
    public bool pauseOnExit = false; // 是否在玩家抵達出口時暫停遊戲

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        // 只反應一次，避免重覆觸發
        if (triggered) return;

        // 用 Player tag 或 Plankton 元件判斷皆可；這裡寬鬆地只認 Plankton 元件
        if (other.GetComponent<Plankton>() == null) return;

        triggered = true;
        Debug.Log("凱倫，我成功了！");

        if (pauseOnExit)
        {
            Time.timeScale = 0f;
        }
    }
}
