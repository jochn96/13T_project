using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public int damage = 10; // ��������
    private bool hasHit = false;

    public Collider hitCollider;

    public void ResetHit() //�ִϸ��̼� �̺�Ʈ ȣ�� �ʱ�ȭ
    {
        if (hitCollider != null)
        {
            hitCollider.enabled = false;
        }
    }

    public void EnableHit()
    {
        if (hitCollider != null)
        {
            hitCollider.enabled = true;
            hasHit = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasHit) return; // �ߺ� Ÿ�� ����

        if (other.CompareTag("Player"))
        {
            // �÷��̾��� Condition ������Ʈ ã��
            PlayerCondition condition = other.GetComponent<PlayerCondition>();
            if (condition != null)
            {
                condition.TakePhysiclaDamage(damage); // �������� ������
                Debug.Log("�÷��̾�� ���� ����: " + damage);
                SoundManager.Instance.PlaySFX("zombie_attack");
            }

            hasHit = true;
        }

    }



}
    
