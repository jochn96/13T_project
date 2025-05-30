using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    protected enum State { Detect, Attack, Die }

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

    [Header("Death Effect Settings")]
    public float redEffectDuration = 1f; // 빨간색 효과 지속시간
                                         // temp 자식 오브젝트의 SkinnedMeshRenderer 참조
    private SkinnedMeshRenderer tempSkinnedMeshRenderer;
    private Material originalMaterial;
    private Material redMaterial;

    public bool IsDead => isDead;

    private NavMeshAgent agent;

    protected virtual void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;

        ChangeState(State.Detect);
        isAttacking = false;

        // temp 자식 오브젝트의 SkinnedMeshRenderer 찾기
        Transform tempTransform = transform.Find("temp");
        if (tempTransform != null)
        {
            tempSkinnedMeshRenderer = tempTransform.GetComponent<SkinnedMeshRenderer>();
            if (tempSkinnedMeshRenderer != null)
            {
                // 원본 메터리얼 저장
                originalMaterial = tempSkinnedMeshRenderer.material;

                // 빨간색 메터리얼 생성 (원본 메터리얼을 복사해서 사용)
                redMaterial = new Material(originalMaterial);
            }
        }
    }

    protected virtual void Update()
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

    protected virtual void Detect()
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

    protected virtual void Attack()
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
            isAttacking = false;
        }
    }

    protected virtual bool InRange(float range)
    {
        return player != null && Vector3.Distance(transform.position, player.position) <= range;
    }

    protected virtual void ChangeState(State newState)
    {
        if (currentState == newState) return;

        if (newState == State.Attack)
        {
            attackTimer = attackCooldown;
        }

        currentState = newState;
    }

    protected virtual void OnTriggerEnter(Collider other)
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

    public virtual void Die()
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

    protected virtual IEnumerator ReturnToPoolAfterDelay(float delay)
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

    protected virtual void OnEnable()
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

        hasBeenHit = false; // 새로 풀에서 나올 때 초기화

        if (tempSkinnedMeshRenderer != null && originalMaterial != null)
        {
            tempSkinnedMeshRenderer.material = originalMaterial;
        }

        ChangeState(State.Detect);
    }

      


}
