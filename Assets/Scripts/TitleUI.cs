using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

public class TitleUI : MonoBehaviour
{
    public void OnClickStartGame()
    {
        SceneManager.LoadScene("MainScene"); //¸ÞÀÎ¾À ·Îµå
    }

    public void ExitGame()
    {
        EditorApplication.isPlaying = false;
    }
}
