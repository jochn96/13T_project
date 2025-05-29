using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationEventHandler : MonoBehaviour
{
    public EnemyAttack enemyAttack;
        

    public void AttackStart()
    {
        enemyAttack.EnableHit(); // 공격 시작 시 콜라이더 활성화
    }

    public void AttackEnd()
    {
        enemyAttack.ResetHit();  // 공격 종료 시 콜라이더 비활성화
    }
}

    



