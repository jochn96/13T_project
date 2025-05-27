using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceSpawn : MonoBehaviour
{
    [SerializeField] List<Bounds> spawnAreas;                            //스폰 영역 입력
    [SerializeField] private Color gizmoColor = new Color(1, 0, 0, .3f); //영역 표시 색 
    [SerializeField] private List<GameObject> resourcePrefabs;           //리소스 프리팹 등록

    private void Start()
    {
        StartCoroutine(SpawnTime());  //스폰타임 코루틴 시작
    }
    public void RandomSpawn()
    {
        if (resourcePrefabs.Count == 0)  //만약에 리소스 프리팹이 없으면 리턴
        {
            return;
        }
        if (spawnAreas.Count == 0)  //만약에 스폰 영역이 없으면 리턴
        {
            return;
        }

        GameObject randomPrefab = resourcePrefabs[Random.Range(0, resourcePrefabs.Count)];  //랜덤 프리팹 선언 = 리소스 프리팹 갯수중에 랜덤으로돌리기
        Bounds randomArea = spawnAreas[Random.Range(0, spawnAreas.Count)];  //랜덤 에어리어 선언 = 스폰 영역 갯수중에 랜덤으로 돌리기

        //설정된 영역의 x, y, z의 좌표의 랜덤값 추출
        Vector3 randomPosition = new Vector3(
            Random.Range(randomArea.min.x, randomArea.max.x),
            Random.Range(randomArea.min.y, randomArea.max.y),
            Random.Range(randomArea.min.z, randomArea.max.z)
        );
        //해당위치의 랜덤 프리팹을 랜덤 포지션에 회전값은 수정하지않고 생성
        Instantiate(randomPrefab, randomPosition, Quaternion.identity);
    }

    private IEnumerator SpawnTime()
    {
        while (true) //무한루프
        {
            RandomSpawn();
            yield return new WaitForSeconds(1f);  //1초간 기다렸다가 생성
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (spawnAreas == null) return;  //스폰 영역이 없다면 리턴

        Gizmos.color = gizmoColor;  //색상 지정 코드
        foreach (var area in spawnAreas)  //스폰 영역 배열에 있는
        {
            Gizmos.DrawWireCube(area.center, area.size);  //중심위치와 사이즈에 맞춰서 그려주는 역할
        }
    }
}
