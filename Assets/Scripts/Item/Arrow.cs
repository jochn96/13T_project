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
        Destroy(gameObject, 5f); // ȭ�� ����
    }

    private void OnTriggerEnter(Collider other)
    {
        // �� �Ǵ� ������Ʈ�� ������ ó��
        Destroy(gameObject);
    }
}
