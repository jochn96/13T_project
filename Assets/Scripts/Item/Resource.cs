using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour
{
    static Resource resource;
    public ItemData itemToGive;
    public int quiiantityPerHit = 1;
    public int capacy;
    [SerializeField] List<Rect> spawnAreas;
    [SerializeField] private List<GameObject> resourcePrefabs;

    private void Start()
    {
        if (resource == null)
            resource = this;
        StartCoroutine(SpawnTime());
    }
    public void Gather(Vector3 hitPoint, Vector3 hitNomal)
    {
        for (int i = 0; i < quiiantityPerHit; i++)
        {
            capacy -= 1;
            Instantiate(itemToGive.dropPrefab, hitPoint + Vector3.up, Quaternion.LookRotation(hitNomal, Vector3.up));
            if (capacy <= 0)
            {
                Destroy(gameObject);
                break;
            }
        }
    }

    public void RandomSpawn()
    {
        GameObject randomPrefab = resourcePrefabs[Random.Range(0, resourcePrefabs.Count)];  //리소스 프리팹 갯수만큼 랜덤 돌리기
        Rect randomArea = spawnAreas[Random.Range(0, spawnAreas.Count)];  //랜덤한 설정지역 중 랜덤
        Vector2 randomPosition = new Vector2(                  //랜덤 설정지역중 좌표랜덤
            Random.Range(randomArea.xMin, randomArea.xMax),
            Random.Range(randomArea.yMin, randomArea.yMax));

        GameObject spawnResource = Instantiate(randomPrefab, new Vector3(randomPosition.x, randomPosition.y), Quaternion.identity);  //리소스 소환
    }

    private IEnumerator SpawnTime()
    {
        while (true)
        {
            RandomSpawn();
            yield return new WaitForSeconds(1f);
        }
    }
}
