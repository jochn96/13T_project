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
        hasHitEnemy = false;   // 공격 시작 시 초기화
        col.enabled = true;
    }

    public void DisableCollider()
    {
        col.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasHitEnemy) return;  // 이미 적 한 명을 맞췄으면 무시

        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null && !enemy.IsDead)
            {
                hasHitEnemy = true;   // 적 한 명만 맞게 함
                enemy.Die();          // 에너미 사망 처리
            }
        }
    }
}
