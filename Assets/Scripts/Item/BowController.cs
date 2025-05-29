using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowController : MonoBehaviour
{
    public Animator bowAnimator;

    void Update()
    {
        // 마우스 왼쪽 클릭 시 발사 애니메이션 트리거
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
