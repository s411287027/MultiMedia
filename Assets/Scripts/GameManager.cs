using UnityEngine;

public class GameManager : MonoBehaviour
{
    // ⚠️ 注意：前面一定要加 public，Unity 的按鈕才看得到這個功能！
    public void QuitGame()
    {
        Debug.Log("玩家按下了離開遊戲！");

        // 實際關閉遊戲的語法 (打包成 .exe 或 App 後才有作用)
        Application.Quit();

        // 為了方便在 Unity 編輯器裡面測試，加上這段讓 Play 模式停止
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}