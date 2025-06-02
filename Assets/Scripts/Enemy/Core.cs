using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;
using UnityEngine.AI;

public class Core : MonoBehaviour
{
    public PlayerCondition playerCondition;
    
    public Animator animator;
    public GameObject cam;


    [Header("Health Settings")]    
    public int currentHp = 500;

    [Header("Death Effect Settings")]

    private SkinnedMeshRenderer tempSkinnedMeshRenderer;
    private Material originalMaterial;
    private Material redMaterial;

    public float redEffectDuration = 1f; // 빨간색 효과 지속시간
                                         // temp 자식 오브젝트의 SkinnedMeshRenderer 참조

       

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Core"))
            {
            Debug.Log("뚱좀공격함@@@@@@@@@@@@");
            TakeDamage(10);
        }
        
    }

    public void TakeDamage(int damage)
    {
        currentHp -= damage;
        

        if (currentHp <= 0)
        {
            cam.SetActive(true);
            StartRedEffect();
            animator.SetTrigger("IsDie");
            playerCondition.Die(); 
            
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
    //public void CoreDestory()
    //{
    //    playerCondition.Die();
    //}
}
