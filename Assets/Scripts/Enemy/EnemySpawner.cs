using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public int poolSize = 10;
    public Transform[] spawnPoints;
    private List<GameObject> enemyPool = new List<GameObject>();

    [Header("DayNight Cycle Reference")]
    public DayNightCycle dayNightCycle;

    public float spawnDelay = 0.5f; // �� ������ ���� ����

    private bool isNight = false;

    void Start()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject enemy = Instantiate(enemyPrefab);
            enemy.SetActive(false);
            enemyPool.Add(enemy);
        }

        StartCoroutine(CheckDayNightCycle());
    }

    IEnumerator CheckDayNightCycle()
    {
        while (true)
        {
            // �� �ð���: 0.75 ~ 1.0 �Ǵ� 0.0 ~ 0.25
            bool nowNight = (dayNightCycle.time >= 0.75f || dayNightCycle.time <= 0.25f);

            if (nowNight && !isNight)
            {
                isNight = true;
                StartCoroutine(SpawnEnemiesSequentially());
            }
            else if (!nowNight && isNight)
            {
                isNight = false;
                KillAllEnemies();
            }

            yield return new WaitForSeconds(1f); // 1�ʸ��� Ȯ��
        }
    }

    IEnumerator SpawnEnemiesSequentially()
    {
        foreach (Transform point in spawnPoints)
        {
            GameObject enemy = GetPooledEnemy();
            if (enemy != null)
            {
                Vector3 offset = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
                enemy.transform.position = point.position + offset;
                enemy.transform.rotation = point.rotation;
                enemy.SetActive(true);
            }

            yield return new WaitForSeconds(spawnDelay); // ���� �� �������� ��
        }
    }

    GameObject GetPooledEnemy()
    {
        foreach (var enemy in enemyPool)
        {
            if (!enemy.activeInHierarchy)
                return enemy;
        }
        return null;
    }

    void KillAllEnemies()
    {
        foreach (var enemy in enemyPool)
        {
            if (enemy.activeInHierarchy)
            {
                Enemy enemyScript = enemy.GetComponent<Enemy>();
                if (enemyScript != null)
                {
                    enemyScript.Die(); 
                }
            }
        }
    }
}