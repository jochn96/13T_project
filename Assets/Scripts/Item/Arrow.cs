using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float speed = 20f;
    public float maxLifeTime = 5f;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        rb.velocity = transform.forward * speed;
        Invoke(nameof(ReturnToPool), maxLifeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            // 적 죽이기 (적 스크립트에 맞는 함수 호출)
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.Die();
            }

            // 화살 비활성화 또는 파괴
            ReturnToPool();
        }
        else if (other.CompareTag("Wall"))
        {
            // 벽에 맞았을 때 처리
            ReturnToPool();
        }
    }

    void ReturnToPool()
    {
        // 풀링 쓰면 반환, 아니면 Destroy
        gameObject.SetActive(false);
    }
}
