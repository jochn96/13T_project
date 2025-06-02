using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

public class TitleUI : MonoBehaviour
{
    public void OnClickStartGame()
    {
        SceneManager.LoadScene("MainScene"); //메인씬 로드
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
