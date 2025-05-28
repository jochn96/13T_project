using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using static UnityEditor.MaterialProperty;

public class Weater : MonoBehaviour
{
    public Weatherdata[] weatherdatas;

    public DropType droptype;

    private void Start()
    {
        //이거 코루틴으로 할꺼임
        int weaterCount = System.Enum.GetValues(typeof(WeaterType)).Length;  //WeaterType의 길이 가져오기
        int randomWeater = Random.Range(0, weaterCount);                     //길이 중 랜덤으로 인덱스 값 하나 가져오기
        WeaterType weater = (WeaterType)randomWeater;                        //가져온 인덱스 값의 WeaterType 추출

        Debug.Log(weater);  //날씨 확인용 디버그

        Weatherdata selectedData = null;  //초기화

        foreach (var data in weatherdatas)
        {
            if (data.Type == weater)  //만약에 데이터 타입이 날씨면 선택한날씨에 데이터 넣기
            {
                selectedData = data; break;  
            }
        }

        if (selectedData != null && selectedData.Event == EventType.Drop)  //데이터가 null이 아니고 이벤트가 드랍이라면!
        {
            droptype.weatherdata = selectedData;  
            droptype.StartWeaterCor();  //드랍코루틴 시작
        }
        else
        {
            Debug.Log("해당 날씨에 Drop 이벤트가 없습니다.");
        }
    }
}
