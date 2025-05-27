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
        // 플레이어를 자동으로 찾음
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
            // 밤 시간 체크 (예: 0.75 ~ 1.0 또는 0.0 ~ 0.25)
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
        Debug.Log("밤이 시작됨 - 몬스터 스폰 시작");

        if (spawnCoroutine != null)
            StopCoroutine(spawnCoroutine);
        if (despawnCoroutine != null)
            StopCoroutine(despawnCoroutine);

        spawnCoroutine = StartCoroutine(SpawnEnemiesRoutine());
        despawnCoroutine = StartCoroutine(ManageActiveEnemies());
    }

    void OnDayStart()
    {
        Debug.Log("낮이 시작됨 - 모든 몬스터 제거");

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
        // 해당 존에 있는 활성 적들 수 체크
        int enemiesInZone = GetEnemiesInZone(zone);

        // 최대 수에 도달했으면 스폰하지 않음
        if (enemiesInZone >= zone.maxEnemiesInZone)
            return false;

        // 확률적으로 스폰 결정
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

        // 랜덤 스폰 포인트 선택
        Transform spawnPoint = zone.spawnPoints[Random.Range(0, zone.spawnPoints.Length)];

        // 플레이어와의 거리 체크를 위한 랜덤 위치 생성
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

                // 적 스크립트에 플레이어 참조 설정
                Enemy enemyScript = enemy.GetComponent<Enemy>();
                if (enemyScript != null && player != null)
                {
                    enemyScript.player = player;
                }

                Debug.Log($"몬스터가 {zone.zoneName}에 스폰됨: {spawnPosition}");
            }
        }
    }

    Vector3 GetValidSpawnPosition(Vector3 centerPosition, float radius)
    {
        int attempts = 10; // 최대 시도 횟수

        for (int i = 0; i < attempts; i++)
        {
            // 스폰 포인트 주변의 랜덤 위치
            Vector3 randomOffset = new Vector3(
                Random.Range(-radius, radius),
                0,
                Random.Range(-radius, radius)
            );

            Vector3 candidatePosition = centerPosition + randomOffset;

            // 플레이어와의 거리 체크
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
                // 플레이어 참조가 없으면 그냥 반환
                return candidatePosition;
            }
        }

        return Vector3.zero; // 유효한 위치를 찾지 못함
    }

    IEnumerator ManageActiveEnemies()
    {
        while (isNight)
        {
            // 비활성화된 적들을 리스트에서 제거
            activeEnemies.RemoveAll(enemy => enemy == null || !enemy.activeInHierarchy);

            // 플레이어로부터 너무 멀어진 적들 제거
            if (player != null)
            {
                for (int i = activeEnemies.Count - 1; i >= 0; i--)
                {
                    GameObject enemy = activeEnemies[i];
                    if (enemy != null && enemy.activeInHierarchy)
                    {
                        float distance = Vector3.Distance(enemy.transform.position, player.position);
                        if (distance > maxDistanceFromPlayer * 1.5f) // 조금 더 여유있게
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

    // 디버그용 기즈모 씬에서 스폰 범위 보여줌
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

    // 런타임에서 상태 확인용
    void OnGUI()
    {
        if (!Application.isPlaying) return;

        GUILayout.BeginArea(new Rect(10, 10, 300, 200));
        GUILayout.Label($"현재 시간: {dayNightCycle.time:F2}");
        GUILayout.Label($"밤 여부: {isNight}");
        GUILayout.Label($"활성 몬스터 수: {activeEnemies.Count}");

        if (ObjectPoolManager.Instance != null)
        {
            GUILayout.Label($"풀에서 활성화된 적: {ObjectPoolManager.Instance.GetActiveCount(enemyPoolName)}");
            GUILayout.Label($"풀 전체 크기: {ObjectPoolManager.Instance.GetPoolSize(enemyPoolName)}");
        }

        GUILayout.EndArea();
    }
}