using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Core : MonoBehaviour
{
    public PlayerCondition playerCondition;
    public BEnemy red;
    public Animator animator;
    public GameObject cam;


    [Header("Health Settings")]
    public int maxHp = 20;
    private int currentHp;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("BEnemy"))
            {
            TakeDamage(10);
        }
        
    }

    public void TakeDamage(int damage)
    {
        currentHp -= damage;
        

        if (currentHp <= 0)
        {
            cam.SetActive(true);
            red.StartRedEffect();
            animator.SetTrigger("IsDie");
            playerCondition.Die(); 
            
        }
    }
    //public void CoreDestory()
    //{
    //    playerCondition.Die();
    //}
}
