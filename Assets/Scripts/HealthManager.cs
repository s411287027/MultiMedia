using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    public Image[] hearts;          // ��J Heart1, Heart2, Heart3
    public Sprite fullHeart;        // ���߹Ϥ�
    public Sprite emptyHeart;       // �Ť߹Ϥ�
    public GameObject gameOverPanel;
    public GameObject winPanel;     // 勝利畫面（拿到蟹堡時顯示，Inspector 拖入）
    public GameTimer gameTimer;     // ��J GameTimer ����

    private int currentHealth = 3;
    private bool gameEnded = false; // 已結束（勝或敗）就不再接受傷害／重覆觸發

    // 拿到 Krabby Patty 時呼叫：這才是真正的成功
    public void Win()
    {
        if (gameEnded) return;
        gameEnded = true;

        if (winPanel != null) winPanel.SetActive(true);
        if (gameTimer != null) gameTimer.StopTimer();
        // 不暫停遊戲，拿到蟹堡後皮老闆還能繼續動，不會卡住
    }

    public void TakeDamage()
    {
        if (gameEnded) return;
        if (currentHealth <= 0) return;

        currentHealth--;
        hearts[currentHealth].sprite = emptyHeart;

        if (currentHealth <= 0)
        {
            GameOver();
        }
    }

    void GameOver()
    {
        gameEnded = true;
        gameOverPanel.SetActive(true);
        gameTimer.StopTimer();
        Time.timeScale = 0f; // �Ȱ��C��
    }

    void Start()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].sprite = fullHeart;
        }
    }

    public void Restart()
    {
        Time.timeScale = 1f; // ��_�ɶ�
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}