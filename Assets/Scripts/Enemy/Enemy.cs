using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    enum State { Idle, Patrol, Detect, Attack }

    [Header("Enemy Settings")]
    public Transform[] movePos;
    public float moveSpeed = 5f;
    public float rotationSpeed = 100f;
    public float reachDistance = 0.2f;
    public float waitTime = 3f;
    public Animator animator;

    [Header("Detect Setting")]
    public Transform player;
    public float detectDistance = 5f;
    public float forgetDistance = 7f;
    public float attackDistance = 1.5f;


    // ���� �� ������
    private Transform targetMovePos;

    private State currentState = State.Patrol;
    private bool isWaiting = false;

    // Start is called before the first frame update
    void Start()
    {
        SetNewDestination();
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case State.Idle:
                break;
            case State.Patrol:
                Patrol();
                break;
            case State.Detect:
                Detect();
                break;
            case State.Attack:
                Attack();
                break;
        }
        if (targetMovePos == null) return;
        MoveTowards(targetMovePos.position);
    }

    void Idle()
    {
        animator.SetFloat("Speed", 0f);
        if (InRange(detectDistance)) ChangeState(State.Detect);
    }

    void Patrol()
    {
        if (isWaiting || targetMovePos == null)
        {
            animator.SetFloat("Speed", 0f);
            return;
        }


        animator.SetFloat("Speed", moveSpeed);
        MoveTowards(targetMovePos.position);

        if (Vector3.Distance(transform.position, targetMovePos.position) <= reachDistance)
        {
            WaitAndResume();
        }


        if (InRange(detectDistance)) ChangeState(State.Detect);
    }

    void Detect()
    {
        animator.SetFloat("Speed", moveSpeed);
        // 1. �̵�Ÿ�� ����(�÷��̾�)
        MoveTowards(player.position);

        // 2. ���ݹ����� ���ų� �Ǵ� �÷��̾� �Ҿ������ �ٽ� ����
        if (InRange(attackDistance))
            ChangeState(State.Attack);
        if (InRange(forgetDistance) == false) ChangeState(State.Patrol);

    }

    void Attack()
    {
        animator.SetBool("IsAttack", true);
        Debug.Log("������...");
        animator.SetBool("IsAttack", true);
        if (InRange(attackDistance) == false)
        {
            animator.SetBool("IsAttack", false);
            ChangeState(State.Detect);
        }



    }

    void WaitAndResume()
    {
        isWaiting = true;
        Invoke("SetNewDestination", waitTime);
    }
    void ChangeState(State newState)
    {
        if (currentState != newState)
            currentState = newState;
    }

    bool InRange(float range)
    {
        if (player == null) return false;

        return Vector3.Distance(transform.position, player.position) <= range;
    }
    void MoveTowards(Vector3 target)
    {
        Vector3 dir = (target - transform.position).normalized;
        if (dir != Vector3.zero)
        {
            dir.y = 0;
            Quaternion LookRot = Quaternion.LookRotation(dir, Vector3.up);

            transform.rotation = Quaternion.Slerp(transform.rotation, LookRot, rotationSpeed * Time.deltaTime);
        }

        transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);

    }
    void SetNewDestination()
    {
        Transform next;

        do
        {
            next = movePos[Random.Range(0, movePos.Length)];
        }
        while (next == targetMovePos);

        isWaiting = false;
        targetMovePos = next;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Bullet") //ȭ��� �ٲٱ�
        {
            Destroy(other.gameObject);
            Destroy(transform.parent.gameObject, t: 2f); //�θ�� 2���� �ı�
            Die();
        }
    }

    void Die() //��� �ִϸ��̼� �߰�����
    {
        for (int i = 0; i < transform.childCount; i++) //�ڽ� ���������
        {
            Transform child = transform.GetChild(i);// ���� �ڽİ�������

            Rigidbody rb = child.gameObject.AddComponent<Rigidbody>(); //������ٵ� ���� �߰��ϱ�

            Vector3 randomdir = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;

            float power = Random.Range(50f, 100f);

            rb.AddForce(randomdir * power);
            rb.AddTorque(new Vector3(360, 360, 360));
        }
    }
}

