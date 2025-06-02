using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public int damage = 10; // 공격피해
    private bool hasHit = false;

    public Collider hitCollider;

    public void ResetHit() //애니메이션 이벤트 호출 초기화
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
        if (hasHit) return; // 중복 타격 방지

        if (other.CompareTag("Player"))
        {
            // 플레이어의 Condition 컴포넌트 찾기
            PlayerCondition condition = other.GetComponent<PlayerCondition>();
            if (condition != null)
            {
                SoundManager.Instance.PlaySFX("zombie_attack");
                condition.TakePhysiclaDamage(damage); // 공격피해 입히기
                //Debug.Log("플레이어에게 피해 입힘: " + damage);
            }

            hasHit = true;
        }

    }



}
    
