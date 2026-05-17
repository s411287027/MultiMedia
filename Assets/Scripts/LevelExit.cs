using UnityEngine;

public class LevelExit : MonoBehaviour
{
    // 廚房出口只是「通過第三關」的檢查點，不是最終勝利。
    // 真正的成功是拿到 Krabby Patty（見 Plankton 的 Patty 分支）。
    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        // 只反應一次，避免重覆觸發
        if (triggered) return;

        // 用 Player tag 或 Plankton 元件判斷皆可；這裡寬鬆地只認 Plankton 元件
        if (other.GetComponent<Plankton>() == null) return;

        triggered = true;
        Debug.Log("通過廚房了！繼續去拿蟹堡！");
        // 不暫停、不結束遊戲，讓皮老闆繼續往蟹堡前進
    }
}
