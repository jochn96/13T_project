using System.Collections;
using System.Linq;
using UnityEngine;

public class Weater : MonoBehaviour
{
    private Coroutine weatherCoroutine;
    public Weatherdata[] weatherdata;  //날씨에 데이터가져오기
    public float ActiveTime = 30f;     //날씨 변화 주기 설정
    private GameObject weatherParticle;//소환할 파티클 선언
    private PlayerCondition playerCondition;

    private void Start()
    {
        playerCondition = GetComponentInParent<PlayerCondition>();

        StartCoroutine(WeaterCor());  //게임이 시작되면 코루틴 시작
    }
    private void StartWeatherCoroutine()  //예비 코루틴
    {
        weatherCoroutine = StartCoroutine(WeaterCor());  
    }

    private IEnumerator WeaterCor()
    {
        while (true)
        {
            int randomWeater = Random.Range(0, weatherdata.Length);  //등록된 날씨 갯수만큼 랜덤돌리기

            foreach (Transform child in transform)  //자식 오브젝트에 위치해 있는 오브젝트 파괴
            {
                Destroy(child.gameObject);
            }

            if (weatherdata[randomWeater].weatherBuffType == WeatherBuffType.Health)
            {
                Debug.Log("Healty Type");
                StartCoroutine(WeatherHealCor(weatherdata[randomWeater].value));
            }
            else if (weatherdata[randomWeater].weatherBuffType == WeatherBuffType.Stamina)
            {
                Debug.Log("Stamina Type");
                StartCoroutine(WeatherStaminaCor(weatherdata[randomWeater].value));
            }

            if (weatherdata[randomWeater].weaterPrefabs == null)  //만약에 프리팹이 없다면
            {
                yield return new WaitForSeconds(ActiveTime);   //설정해놓은 대기시간동안 기다렸다가
                StartWeatherCoroutine();  //코루틴은 다시 실행해주고 (예비코루틴)
                yield break;              //기존 코루틴은 멈추기 (Bloom같은 예외처리)
            }

            weatherParticle = weatherdata[randomWeater].weaterPrefabs;  //랜덤으로 돌린 weatherdata의 프리팹을 weatherParticle에 넣어주기

            Vector3 localPosition = new Vector3(0, 1, 5);  //부모 오브젝트로 부터 0,1,5 좌표 선언
            Quaternion rotation = Quaternion.identity;  //회전값 없음
            Vector3 worldPosition = transform.TransformPoint(localPosition); //월드좌표 선언 (부모 오브젝트가 어디에 있든 위치, 회전, 스케일을 반영해서 계산해주기위함)

            var instance = Instantiate(weatherParticle, worldPosition, rotation, transform); //위에 선언한 값으로 프리팹 소환

            yield return new WaitForSeconds(ActiveTime);  //대기시간동안 기다리기
        }
    }

    private IEnumerator WeatherHealCor(float weatherValue)
    {
        while (true)
        {
            playerCondition.Heal(weatherValue);

            yield return new WaitForSeconds(ActiveTime);
        }
    }

    private IEnumerator WeatherStaminaCor(float weatherValue)
    {
        while (true)
        {
            playerCondition.DrinkWater(weatherValue);

            yield return new WaitForSeconds(ActiveTime);
        }
    }
}
