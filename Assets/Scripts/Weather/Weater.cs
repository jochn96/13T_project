using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weater : MonoBehaviour
{
    public DropType droptype;

    private void Start()
    {
        //이거 코루틴으로 할꺼임
        int weaterCount = System.Enum.GetValues(typeof(WeaterType)).Length;  //WeaterType의 길이 가져오기
        int randomWeater = Random.Range(0, weaterCount);                     //길이 중 랜덤으로 인덱스 값 하나 가져오기
        WeaterType weater = (WeaterType)randomWeater;                        //가져온 인덱스 값의 WeaterType 추출

        Debug.Log(weater);  //날씨 확인용 디버그


    }
}
