using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpawnZone
{
    public string zoneName;
    public Transform[] spawnPoints;
    public float spawnRadius = 5f;
    public int maxEnemiesInZone = 3;
    [Range(0f, 1f)] public float spawnChance = 0.8f;
}

public class EnemySpawner : MonoBehaviour
{
    [Header("Pool Settings")]
    public string enemyPoolName = "Enemy";

    [Header("Day/Night Cycle")]
    public DayNightCycle dayNightCycle;
    [Range(0f, 1f)] public float nightStartTime = 0.7f;
    [Range(0f, 1f)] public float nightEndTime = 0.15f;

    [Header("Spawn Settings")]
    public List<SpawnZone> spawnZones = new List<SpawnZone>();
    public float spawnInterval = 2f;
    public float despawnCheckInterval = 1f;

    [Header("Player Distance Check")]
    public Transform player;
    public float minDistanceFromPlayer = 10f;
    public float maxDistanceFromPlayer = 50f;

    private bool isNight = false;
    private bool wasNight = false;
    private List<GameObject> activeEnemies = new List<GameObject>();
    private Coroutine spawnCoroutine;
    private Coroutine despawnCoroutine;

    void Start()
    {
        // �÷��̾ �ڵ����� ã��
        if (player == null)
        {
            GameObject playerGO = GameObject.FindGameObjectWithTag("Player");
            if (playerGO != null)
                player = playerGO.transform;
        }

        StartCoroutine(MonitorDayNightCycle());
    }

    IEnumerator MonitorDayNightCycle()
    {
        while (true)
        {
            // �� �ð� üũ (��: 0.75 ~ 1.0 �Ǵ� 0.0 ~ 0.25)
            isNight = (dayNightCycle.time >= nightStartTime || dayNightCycle.time <= nightEndTime);

            if (isNight && !wasNight)
            {
                OnNightStart();
            }
            else if (!isNight && wasNight)
            {
                OnDayStart();
            }

            wasNight = isNight;
            yield return new WaitForSeconds(1f);
        }
    }

    void OnNightStart()
    {
        Debug.Log("���� ���۵� - ���� ���� ����");

        if (spawnCoroutine != null)
            StopCoroutine(spawnCoroutine);
        if (despawnCoroutine != null)
            StopCoroutine(despawnCoroutine);

        spawnCoroutine = StartCoroutine(SpawnEnemiesRoutine());
        despawnCoroutine = StartCoroutine(ManageActiveEnemies());
    }

    void OnDayStart()
    {
        Debug.Log("���� ���۵� - ��� ���� ����");

        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
        if (despawnCoroutine != null)
        {
            StopCoroutine(despawnCoroutine);
            despawnCoroutine = null;
        }

        DespawnAllEnemies();
    }

    IEnumerator SpawnEnemiesRoutine()
    {
        while (isNight)
        {
            foreach (SpawnZone zone in spawnZones)
            {
                if (ShouldSpawnInZone(zone))
                {
                    SpawnEnemyInZone(zone);
                }
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    bool ShouldSpawnInZone(SpawnZone zone)
    {
        // �ش� ���� �ִ� Ȱ�� ���� �� üũ
        int enemiesInZone = GetEnemiesInZone(zone);

        // �ִ� ���� ���������� �������� ����
        if (enemiesInZone >= zone.maxEnemiesInZone)
            return false;

        // Ȯ�������� ���� ����
        return Random.value <= zone.spawnChance;
    }

    int GetEnemiesInZone(SpawnZone zone)
    {
        int count = 0;
        foreach (GameObject enemy in activeEnemies)
        {
            if (enemy != null && enemy.activeInHierarchy)
            {
                foreach (Transform spawnPoint in zone.spawnPoints)
                {
                    if (Vector3.Distance(enemy.transform.position, spawnPoint.position) <= zone.spawnRadius)
                    {
                        count++;
                        break;
                    }
                }
            }
        }
        return count;
    }

    void SpawnEnemyInZone(SpawnZone zone)
    {
        if (ObjectPoolManager.Instance == null || zone.spawnPoints.Length == 0)
            return;

        // ���� ���� ����Ʈ ����
        Transform spawnPoint = zone.spawnPoints[Random.Range(0, zone.spawnPoints.Length)];

        // �÷��̾���� �Ÿ� üũ�� ���� ���� ��ġ ����
        Vector3 spawnPosition = GetValidSpawnPosition(spawnPoint.position, zone.spawnRadius);

        if (spawnPosition != Vector3.zero)
        {
            GameObject enemy = ObjectPoolManager.Instance.SpawnFromPool(
                enemyPoolName,
                spawnPosition,
                Quaternion.identity
            );

            if (enemy != null)
            {
                activeEnemies.Add(enemy);

                // �� ��ũ��Ʈ�� �÷��̾� ���� ����
                Enemy enemyScript = enemy.GetComponent<Enemy>();
                if (enemyScript != null && player != null)
                {
                    enemyScript.player = player;
                }

                Debug.Log($"���Ͱ� {zone.zoneName}�� ������: {spawnPosition}");
            }
        }
    }

    Vector3 GetValidSpawnPosition(Vector3 centerPosition, float radius)
    {
        int attempts = 10; // �ִ� �õ� Ƚ��

        for (int i = 0; i < attempts; i++)
        {
            // ���� ����Ʈ �ֺ��� ���� ��ġ
            Vector3 randomOffset = new Vector3(
                Random.Range(-radius, radius),
                0,
                Random.Range(-radius, radius)
            );

            Vector3 candidatePosition = centerPosition + randomOffset;

            // �÷��̾���� �Ÿ� üũ
            if (player != null)
            {
                float distanceToPlayer = Vector3.Distance(candidatePosition, player.position);

                if (distanceToPlayer >= minDistanceFromPlayer && distanceToPlayer <= maxDistanceFromPlayer)
                {
                    return candidatePosition;
                }
            }
            else
            {
                // �÷��̾� ������ ������ �׳� ��ȯ
                return candidatePosition;
            }
        }

        return Vector3.zero; // ��ȿ�� ��ġ�� ã�� ����
    }

    IEnumerator ManageActiveEnemies()
    {
        while (isNight)
        {
            // ��Ȱ��ȭ�� ������ ����Ʈ���� ����
            activeEnemies.RemoveAll(enemy => enemy == null || !enemy.activeInHierarchy);

            // �÷��̾�κ��� �ʹ� �־��� ���� ����
            if (player != null)
            {
                for (int i = activeEnemies.Count - 1; i >= 0; i--)
                {
                    GameObject enemy = activeEnemies[i];
                    if (enemy != null && enemy.activeInHierarchy)
                    {
                        float distance = Vector3.Distance(enemy.transform.position, player.position);
                        if (distance > maxDistanceFromPlayer * 1.5f) // ���� �� �����ְ�
                        {
                            ReturnEnemyToPool(enemy);
                            activeEnemies.RemoveAt(i);
                        }
                    }
                }
            }

            yield return new WaitForSeconds(despawnCheckInterval);
        }
    }

    void DespawnAllEnemies()
    {
        foreach (GameObject enemy in activeEnemies)
        {
            if (enemy != null && enemy.activeInHierarchy)
            {
                Enemy enemyScript = enemy.GetComponent<Enemy>();
                if (enemyScript != null)
                {
                    enemyScript.Die();
                }
                else
                {
                    ReturnEnemyToPool(enemy);
                }
            }
        }

        activeEnemies.Clear();
    }

    void ReturnEnemyToPool(GameObject enemy)
    {
        if (ObjectPoolManager.Instance != null)
        {
            ObjectPoolManager.Instance.ReturnToPool(enemyPoolName, enemy);
        }
    }

    // ����׿� ����� ������ ���� ���� ������
    void OnDrawGizmosSelected()
    {
        foreach (SpawnZone zone in spawnZones)
        {
            if (zone.spawnPoints != null)
            {
                Gizmos.color = Color.yellow;
                foreach (Transform point in zone.spawnPoints)
                {
                    if (point != null)
                    {
                        Gizmos.DrawWireSphere(point.position, zone.spawnRadius);
                        Gizmos.DrawWireCube(point.position, Vector3.one * 0.5f);
                    }
                }
            }
        }

        if (player != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(player.position, minDistanceFromPlayer);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(player.position, maxDistanceFromPlayer);
        }
    }

    // ��Ÿ�ӿ��� ���� Ȯ�ο�
    void OnGUI()
    {
        if (!Application.isPlaying) return;

        GUILayout.BeginArea(new Rect(10, 10, 300, 200));
        GUILayout.Label($"���� �ð�: {dayNightCycle.time:F2}");
        GUILayout.Label($"�� ����: {isNight}");
        GUILayout.Label($"Ȱ�� ���� ��: {activeEnemies.Count}");

        if (ObjectPoolManager.Instance != null)
        {
            GUILayout.Label($"Ǯ���� Ȱ��ȭ�� ��: {ObjectPoolManager.Instance.GetActiveCount(enemyPoolName)}");
            GUILayout.Label($"Ǯ ��ü ũ��: {ObjectPoolManager.Instance.GetPoolSize(enemyPoolName)}");
        }

        GUILayout.EndArea();
    }
}