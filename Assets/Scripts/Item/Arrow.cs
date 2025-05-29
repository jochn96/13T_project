using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float speed = 50f;
    public int damage = 10;

    private void Start()
    {
        GetComponent<Rigidbody>().velocity = transform.forward * speed;
        Destroy(gameObject, 5f); // 화살 제거
    }

    private void OnTriggerEnter(Collider other)
    {
        // 적 또는 오브젝트에 데미지 처리
        Destroy(gameObject);
    }
}
