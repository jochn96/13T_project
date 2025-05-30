using UnityEngine;

public class SwordColliderController : MonoBehaviour
{
    private Collider col;
    private bool hasHitEnemy = false;

    void Awake()
    {
        col = GetComponent<Collider>();
        col.enabled = false;
    }

    public void EnableCollider()
    {
        hasHitEnemy = false;   // ���� ���� �� �ʱ�ȭ
        col.enabled = true;
    }

    public void DisableCollider()
    {
        col.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasHitEnemy) return;  // �̹� �� �� ���� �������� ����

        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null && !enemy.IsDead)
            {
                hasHitEnemy = true;   // �� �� �� �°� ��
                enemy.Die();          // ���ʹ� ��� ó��
            }
        }
    }
}
