using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    public Image[] hearts;          // 拖入 Heart1, Heart2, Heart3
    public Sprite fullHeart;        // 滿心圖片
    public Sprite emptyHeart;       // 空心圖片
    public GameObject gameOverPanel;
    public GameTimer gameTimer;     // 拖入 GameTimer 物件

    private int currentHealth = 3;

    public void TakeDamage()
    {
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
        gameOverPanel.SetActive(true);
        gameTimer.StopTimer();
        Time.timeScale = 0f; // 暫停遊戲
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
        Time.timeScale = 1f; // 恢復時間
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}