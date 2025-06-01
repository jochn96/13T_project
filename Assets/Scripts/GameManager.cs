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
            DontDestroyOnLoad(gameObject); // 필요할 경우 유지
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void RestartGame()
    {
        

        foreach (EnemySpawner spawner in enemySpawners)
        {
            if (spawner != null)
            {
                spawner.ClearAllEnemiesForRestart();
            }
        }



        // 잠시 대기 후 씬 로드 (풀 반환 완료 대기)
        StartCoroutine(LoadSceneAfterDelay());
        SceneManager.LoadScene("MainScene");
        
    }
    IEnumerator LoadSceneAfterDelay()
    {
        yield return new WaitForSeconds(0.1f); // 풀 반환 대기
        
        
    }
   
    // 게임 처음 시작 (타이틀에서 호출)
    public void StartGame()
    {
        SceneManager.LoadScene("MainScene");
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainScene")
        {
            DayNightCycle dayNightCycle = FindObjectOfType<DayNightCycle>();
            if (dayNightCycle != null)
            {
                dayNightCycle.ForceUpdateLighting(); // 메인씬 로드 시 강제로 조명 갱신
            }
        }
    }
}
