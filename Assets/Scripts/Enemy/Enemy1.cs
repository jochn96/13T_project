using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    enum State { Detect, Attack, Die }

    public EnemyAttack hitbox;

    [Header("Enemy Settings")]
    public float moveSpeed = 2f;  // 적용 안됨 애니메이터에서 속도 조절해야됨
    public float rotationSpeed = 100f;
    public Animator animator;

    [Header("Player Settings")]
    public Transform player;
    public float detectDistance = 5f;      // (현재는 안 쓰지만 유지)
    public float attackDistance = 1.5f;

    [Header("Attack Settings")]
    public float attackCooldown = 1f;
    private float attackTimer = 0f;

    private State currentState = State.Detect;
    private bool isDead = false;

    private bool isAttacking = false;
    private Vector3 targetDirection;

    void Start()
    {
        ChangeState(State.Detect);
        isAttacking = false ;
    }

    void Update()
    {
        if (isDead) return;

        switch (currentState)
        {
            case State.Detect:
                Detect();
                break;
            case State.Attack:
                Attack();
                break;
        }

        if (currentState == State.Attack)
            attackTimer += Time.deltaTime;
    }

    void Detect()
    {
     
        if(isAttacking) return;
        
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        // 플레이어를 향해 이동
        //MoveTowards(player.position);        
        targetDirection = (player.position - transform.position).normalized;
        targetDirection.y = 0;

        // 이동 애니메이션 재생
        animator.SetFloat("Speed", moveSpeed);  //에너미 스피드 조절

        // 회전만 스크립트에서 처리
        if (targetDirection != Vector3.zero)
        {
            Quaternion lookRot = Quaternion.LookRotation(targetDirection, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, rotationSpeed * Time.deltaTime);
        }

        // 공격 범위에 들어오면 공격 상태로 전환
        if (distance <= attackDistance)
        {
            ChangeState(State.Attack);
        }
    }

    void Attack()
    {
        isAttacking = true;

        if (!InRange(attackDistance))
        {
            ChangeState(State.Detect);
            isAttacking = false;
            return;
        }

        // 플레이어 방향으로 회전
        Vector3 dir = (player.position - transform.position).normalized;
        dir.y = 0;
        if (dir != Vector3.zero)
        {
            Quaternion lookRot = Quaternion.LookRotation(dir, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, rotationSpeed * Time.deltaTime);
        }

        // 멈추기 
        // animator.SetFloat("Speed", 0f);

        if (attackTimer >= attackCooldown)
        {
            attackTimer = 0f;
            animator.SetTrigger("IsAttack");
            Debug.Log("공격!");
            isAttacking = false;

        }
        //isAttacking= false;

        
    }

    
    

    void MoveTowards(Vector3 target)
    {
        Vector3 dir = (target - transform.position).normalized;
        dir.y = 0;

        if (dir != Vector3.zero)
        {
            Quaternion lookRot = Quaternion.LookRotation(dir, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, rotationSpeed * Time.deltaTime);
        }

        transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);

        
        
    }

    bool InRange(float range)
    {
        return player != null && Vector3.Distance(transform.position, player.position) <= range;
    }

    void ChangeState(State newState)
    {
        if (currentState == newState) return;

        if (newState == State.Attack)
        {
            attackTimer = attackCooldown; // 공격 상태로 진입하면 즉시 공격 가능
        }

        currentState = newState;
    }

    //private void OnTriggerEnter(Collider other) // 화살 맞았을떄 
    //{
    //    if (other.CompareTag("Bullet") && !isDead)
    //    {
    //        Destroy(other.gameObject);
    //        Die();
    //    }
    //}

    void Die()
    {
        isDead = true;
        currentState = State.Die;
        animator.SetTrigger("IsDie");

        // 적 삭제 (3초 후)
        Destroy(gameObject, 3f);
        Debug.Log("적 사망");
    }
}