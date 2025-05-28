using System.Collections;
using UnityEngine;

public class DropType : MonoBehaviour
{
    public Weatherdata weatherdata;  //데이터 불러오기
    public GameObject DropPrefab;  //프리팹을 불러올 공간
    private Coroutine dropCoroutine;  

    private void Start()
    {
        
    }

    private IEnumerator WeaterCor(GameObject dropFre)
    {
        dropFre = DropPrefab;  //드랍프리팹 넣어주기
        while (true)           //무한 반복인데 수정예정 하루 시간을 계산해서 그게 줄어들면 다음 날씨로 이동하게 만들것임
        {
            Vector3 randomPosition = new Vector3(                                //자신의 포지션에서 오차 5씩 프리팹이 떨어지도록
            Random.Range(transform.position.x - 5, transform.position.x + 5),
            Random.Range(transform.position.y - 5, transform.position.y + 5),
            Random.Range(transform.position.z - 5, transform.position.z + 5));

            Instantiate(dropFre, randomPosition, Quaternion.identity);    //드롭 프리팹을 랜덤포지션에 소환
            yield return new WaitForSeconds(0.1f);  //0.5초마다
        }
    }

    public void StartWeaterCor()
    {
        try
        {
            DropPrefab = weatherdata.weaterPrefabs;  //드랍 프리팹을 데이터에서 불러오기
            if (DropPrefab == null)              //만약에 드랍 프리팹이 없으면 던지기
                throw new System.Exception("프리팹 데이터가 없습니다.");
            if (dropCoroutine != null)  //이미 실행중인 코루틴이 있으면 코루틴을 종료하고 시작
            {
                StopCoroutine(dropCoroutine);
                dropCoroutine = null;
            }
            dropCoroutine = StartCoroutine(WeaterCor(DropPrefab));
            StartWeaterCor();  //그렇지 않으면 코루틴 시작
        }
        catch  //오류처리
        {
            Debug.Log("날씨 오류");
        }
    }
}
