using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    enum State { Detect, Attack, Die }

    public EnemyAttack hitbox;

    [Header("Enemy Settings")]
    public float moveSpeed = 2f;
    public float rotationSpeed = 100f;
    public Animator animator;

    [Header("Player Settings")]
    public Transform player;
    public float detectDistance = 5f;
    public float attackDistance = 2f;

    [Header("Attack Settings")]
    public float attackCooldown = 1f;
    private float attackTimer = 0f;

    [Header("오브젝트 설정")]
    public string poolName = "원하는 이름";

    private State currentState = State.Detect;
    private bool isDead = false;
    private bool isAttacking = false;

    private bool hasBeenHit = false;

    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;

        ChangeState(State.Detect);
        isAttacking = false;
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
        if (isAttacking || player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        agent.isStopped = false;
        agent.SetDestination(player.position);  //nav mesh로 플레이어 추적

        animator.SetFloat("Speed", agent.velocity.magnitude); // 속도에 따라 애니메이션

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

        // 이동 멈추고 회전
        agent.isStopped = true;
        Vector3 dir = (player.position - transform.position).normalized;
        dir.y = 0;
        if (dir != Vector3.zero)
        {
            Quaternion lookRot = Quaternion.LookRotation(dir, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, rotationSpeed * Time.deltaTime);
        }

        if (attackTimer >= attackCooldown)
        {
            attackTimer = 0f;
            animator.SetTrigger("IsAttack");
            Debug.Log("공격!");
            isAttacking = false;
        }
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
            attackTimer = attackCooldown;
        }

        currentState = newState;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isDead || hasBeenHit) return;

        if (other.CompareTag("Sword") || other.CompareTag("Bullet")) // 둘 다 가능하게
        {
            hasBeenHit = true;
            if (other.CompareTag("Bullet"))
                Destroy(other.gameObject);

            Die();
        }
    }

    public void Die()
    {
        isDead = true;
        currentState = State.Die;
        animator.SetTrigger("IsDie");

        if (agent != null)
            agent.isStopped = true;

        foreach (Collider col in GetComponentsInChildren<Collider>())
        {
            col.enabled = false;
        }

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        StartCoroutine(ReturnToPoolAfterDelay(5f));
    }

    private IEnumerator ReturnToPoolAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (ObjectPoolManager.Instance != null)
        {
            ObjectPoolManager.Instance.ReturnToPool(poolName, gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnEnable()
    {
        isDead = false;
        isAttacking = false;
        attackTimer = 0f;

        animator.ResetTrigger("IsDie");
        animator.ResetTrigger("IsAttack");

        foreach (Collider col in GetComponentsInChildren<Collider>())
        {
            col.enabled = true;
        }

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }

        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        if (agent != null)
        {
            agent.isStopped = false;
            agent.speed = moveSpeed;
        }

        ChangeState(State.Detect);
    }
}
