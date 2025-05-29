using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowController : MonoBehaviour
{
    public Animator bowAnimator;

    void Update()
    {
        // ���콺 ���� Ŭ�� �� �߻� �ִϸ��̼� Ʈ����
        if (Input.GetMouseButtonDown(0))
        {
            Fire();
        }
    }

    void Fire()
    {
        if (bowAnimator != null)
        {
            bowAnimator.SetTrigger("IsFire");
        }
    }
}
