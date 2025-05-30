using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BEnemy : Enemy
{
           
    private float attackTimer = 0f;

    [Header("오브젝트 설정")]
    

    private State currentState = State.Detect;
    private bool isDead = false;
    private bool isAttacking = false;

    [Header("Health Settings")]
    private int maxHp = 20;
    public int currentHp;

    [Header("Death Effect Settings")]
    
    private SkinnedMeshRenderer tempSkinnedMeshRenderer;
    private Material originalMaterial;
    private Material redMaterial;
        
    private NavMeshAgent agent;

    protected override void Start()
    {
        base.Start();
        

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

    protected override void OnEnable()
    {
        base.OnEnable();
        currentHp = maxHp;
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (isDead) return;

        if (other.CompareTag("Sword") || other.CompareTag("Bullet"))
        {
            Debug.Log("맞음@@@@@@@@@@@@@@");
            // 빨간색 반응 먼저 시작
            StartRedEffect();

            // 피격 처리
            TakeDamage(10);

            if (other.CompareTag("Bullet"))
                Destroy(other.gameObject);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHp -= damage;
        

        if (currentHp <= 0)
        {
            
            Die();
        }
    }
        

    
    public void StartRedEffect()
    {
        if (tempSkinnedMeshRenderer != null)
        {
            StartCoroutine(RedEffect());
        }
    }

    private IEnumerator RedEffect()
    {
        if (tempSkinnedMeshRenderer == null || redMaterial == null) yield break;

        float elapsedTime = 0f;
        Color originalColor = originalMaterial.color;
        Color targetColor = Color.red;

        while (elapsedTime < redEffectDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / redEffectDuration;

            Color currentColor = Color.Lerp(originalColor, targetColor, progress);
            redMaterial.color = currentColor;
            tempSkinnedMeshRenderer.material = redMaterial;

            yield return null;
        }

        redMaterial.color = targetColor;
        tempSkinnedMeshRenderer.material = redMaterial;
    }
}
