using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public EnemySpawner[] enemySpawners;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // �ʿ��� ��� ����
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainScene");
        foreach (EnemySpawner spawner in enemySpawners)
        {
            if (spawner != null)
            {
                spawner.ClearAllEnemiesForRestart();
            }
        }



        // ��� ��� �� �� �ε� (Ǯ ��ȯ �Ϸ� ���)
        StartCoroutine(LoadSceneAfterDelay());
    }
    IEnumerator LoadSceneAfterDelay()
    {
        yield return new WaitForSeconds(0.1f); // Ǯ ��ȯ ���
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

}
