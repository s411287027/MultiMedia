using UnityEngine;
using TMPro;

public class WinPanelController : MonoBehaviour
{
    public TextMeshProUGUI timeResultText; // 顯示耗時的文字

    public void ShowResult(float elapsedTime)
    {
        int minutes = (int)(elapsedTime / 60);
        int seconds = (int)(elapsedTime % 60);
        timeResultText.text = string.Format("Time: {0:00}:{1:00}", minutes, seconds);
    }
}