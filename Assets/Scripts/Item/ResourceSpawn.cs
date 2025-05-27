using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceSpawn : MonoBehaviour
{
    [SerializeField] List<Bounds> spawnAreas;
    [SerializeField] private Color gizmoColor = new Color(1, 0, 0, .3f);
    [SerializeField] private List<GameObject> resourcePrefabs;

    private void Start()
    {
        StartCoroutine(SpawnTime());
    }
    public void RandomSpawn()
    {
        if (resourcePrefabs.Count == 0)
        {
            Debug.LogWarning("resourcePrefabs 리스트가 비어 있습니다.");
            return;
        }
        if (spawnAreas.Count == 0)
        {
            Debug.LogWarning("spawnAreas 리스트가 비어 있습니다.");
            return;
        }

        GameObject randomPrefab = resourcePrefabs[Random.Range(0, resourcePrefabs.Count)];
        Bounds randomArea = spawnAreas[Random.Range(0, spawnAreas.Count)];

        Vector3 randomPosition = new Vector3(
            Random.Range(randomArea.min.x, randomArea.max.x),
            Random.Range(randomArea.min.y, randomArea.max.y),
            Random.Range(randomArea.min.z, randomArea.max.z)
        );

        Instantiate(randomPrefab, randomPosition, Quaternion.identity);
    }

    private IEnumerator SpawnTime()
    {
        while (true)
        {
            RandomSpawn();
            yield return new WaitForSeconds(1f);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (spawnAreas == null) return;

        Gizmos.color = gizmoColor;
        foreach (var area in spawnAreas)
        {
            Gizmos.DrawWireCube(area.center, area.size);
        }
    }
}
