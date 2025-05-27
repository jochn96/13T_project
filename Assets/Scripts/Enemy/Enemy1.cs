using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    enum State { Detect, Attack, Die }

    public EnemyAttack hitbox;

    [Header("Enemy Settings")]
    public float moveSpeed = 2f;  // ���� �ȵ� �ִϸ����Ϳ��� �ӵ� �����ؾߵ�
    public float rotationSpeed = 100f;
    public Animator animator;

    [Header("Player Settings")]
    public Transform player;
    public float detectDistance = 5f;      // (����� �� ������ ����)
    public float attackDistance = 2f;

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

        // �÷��̾ ���� �̵�
        //MoveTowards(player.position);        
        targetDirection = (player.position - transform.position).normalized;
        targetDirection.y = 0;

        // �̵� �ִϸ��̼� ���
        animator.SetFloat("Speed", moveSpeed + Random.Range(-0.2f, 0.2f));  //���ʹ� ���ǵ� ����

        // ȸ���� ��ũ��Ʈ���� ó��
        if (targetDirection != Vector3.zero)
        {
            Quaternion lookRot = Quaternion.LookRotation(targetDirection, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, rotationSpeed * Time.deltaTime);
        }

        // ���� ������ ������ ���� ���·� ��ȯ
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

        // �÷��̾� �������� ȸ��
        Vector3 dir = (player.position - transform.position).normalized;
        dir.y = 0;
        if (dir != Vector3.zero)
        {
            Quaternion lookRot = Quaternion.LookRotation(dir, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, rotationSpeed * Time.deltaTime);
        }

        // ���߱� 
        // animator.SetFloat("Speed", 0f);

        if (attackTimer >= attackCooldown)
        {
            attackTimer = 0f;
            animator.SetTrigger("IsAttack");
            Debug.Log("����!");
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
            attackTimer = attackCooldown; // ���� ���·� �����ϸ� ��� ���� ����
        }

        currentState = newState;
    }

    //private void OnTriggerEnter(Collider other) // ȭ�� �¾����� 
    //{
    //    if (other.CompareTag("Bullet") && !isDead)
    //    {
    //        Destroy(other.gameObject);  // ȭ�� ����
    //        Die();  //���ʹ� ��� ó��
    //    }
    //}

    public void Die()
    {
        isDead = true;
        currentState = State.Die;
        animator.SetTrigger("IsDie");

        // �ݶ��̴� ���� , ����� �ٸ� ���ʹ̶� �浹ó�� ����
        foreach (Collider col in GetComponentsInChildren<Collider>())
        {
            col.enabled = false;
        }

        Debug.Log("�� ���");

        // 3�� �� ����
        Destroy(gameObject, 3f);
    }

    void OnEnable()
    {
        isDead = false;
        isAttacking = false;
        attackTimer = 0f;

        animator.ResetTrigger("IsDie");
        animator.ResetTrigger("IsAttack");

        // �ȱ� ���·� �ִϸ��̼� ����
        animator.SetFloat("Speed", moveSpeed + Random.Range(-0.2f, 0.2f));

        ChangeState(State.Detect);
    }
}