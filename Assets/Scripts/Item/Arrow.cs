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
            // �� ���̱� (�� ��ũ��Ʈ�� �´� �Լ� ȣ��)
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.Die();
            }

            // ȭ�� ��Ȱ��ȭ �Ǵ� �ı�
            ReturnToPool();
        }
        else if (other.CompareTag("Wall"))
        {
            // ���� �¾��� �� ó��
            ReturnToPool();
        }
    }

    void ReturnToPool()
    {
        // Ǯ�� ���� ��ȯ, �ƴϸ� Destroy
        gameObject.SetActive(false);
    }
}
