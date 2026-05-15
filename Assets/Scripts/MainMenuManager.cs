using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void StartGame()
    {
        Debug.Log("StartGame called!"); // ¥[³o¦æ
        SceneManager.LoadScene("SampleScene");
    }

    public GameObject introPanel;


    public void ShowIntro()
    {
        introPanel.SetActive(true);
    }

    public void CloseIntro()
    {
        introPanel.SetActive(false);
    }
}