using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using UnityEngine;

public class Resource : MonoBehaviour
{
    public ItemData itemToGive;  //주는 아이템
    public int quiiantityPerHit = 1; //타격수
    public int capacy;  //채취할수 있는 갯수
    public Resource resource;

    private void Start()
    {
        resource = GetComponent<Resource>();
    }
    public void Gather(Vector3 hitPoint, Vector3 hitNomal)
    {
        for (int i = 0; i < quiiantityPerHit; i++)  //한번때리면
        {
            capacy -= 1;  //채취할수 있는 갯수 감소
            Instantiate(itemToGive.dropPrefab, hitPoint + Vector3.up, Quaternion.LookRotation(hitNomal, Vector3.up));  //주는 아이템을 랜덤위치에 드랍
            if (capacy <= 0)  //만약에 채취할 수 있는 갯수가 없으면
            {
                Destroy(gameObject);  //게임오브젝트는 파괴
                break;
            }
        }
    }
}
