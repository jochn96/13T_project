using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

public class TitleUI : MonoBehaviour
{
    public void OnClickStartGame()
    {
        SceneManager.LoadScene("MainScene"); //���ξ� �ε�
    }

    public void ExitGame()
    {
        EditorApplication.isPlaying = false;
    }
}
