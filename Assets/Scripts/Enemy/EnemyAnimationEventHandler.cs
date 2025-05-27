using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationEventHandler : MonoBehaviour
{
    public EnemyAttack enemyAttack;
        

    public void AttackStart()
    {
        enemyAttack.EnableHit(); // ���� ���� �� �ݶ��̴� Ȱ��ȭ
    }

    public void AttackEnd()
    {
        enemyAttack.ResetHit();  // ���� ���� �� �ݶ��̴� ��Ȱ��ȭ
    }
}

    



