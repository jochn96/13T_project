using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageIbe
{
    void TakePhysiclaDamage(int damage);
}

public class PlayerCondition : MonoBehaviour, IDamageIbe
{
    public UICondition uICondition;
    public float healthRegenRate = 1f; //추가한 변수 

    Condition health { get { return uICondition.health; } }
    Condition hunger { get { return uICondition.hunger; } }
    Condition water { get { return uICondition.water; } }

    public float noHungerHealthDecay;

    public void RestoreFromItem(ItemData item)
    {
        if (item.type != ItemType.Consumable) return;

        foreach (var effect in item.consumables)
        {
            switch (effect.type)
            {
                case ConsumableType.Health:
                    health.Add(effect.value); // 체력 회복
                    break;
                case ConsumableType.Hunger:
                    hunger.Add(effect.value); // 허기 회복
                    break;
                case ConsumableType.Water:
                    water.Add(effect.value); // 수분 회복
                    break;
            }
        }
    }
    
    public event Action onTakeDamage;
    private void Update()
    {
        hunger.Subject(hunger.passiveValue * Time.deltaTime);
        water.Subject(water.passiveValue * Time.deltaTime);

 
        if(hunger.curValue < 0f)
        {
            health.Subject(noHungerHealthDecay * Time.deltaTime);
        }
        if(health.curValue < 0f)
        {
            Die();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Enemy 태그 오브젝트와 충돌시 대미지
        if (other.CompareTag("Enemy"))
        {
            TakePhysiclaDamage(10); //임시 고정 대미지
        }
    }

    public void Heal(float amout)
    {
        health.Add(amout);
    }

    public void Eat(float amout)
    {
        hunger.Add(amout);
    }
    public void Die()
    {
        Debug.Log("die");
    }

    public void TakePhysiclaDamage(int damage)
    {
        health.Subject(damage);
        onTakeDamage?.Invoke();
    }

    public void DrinkWater(float amount)
    {
        water.Add(amount);
    }


    public void SetHealthRegenRate(float rate)
    {
        // 체력 회복 속도 설정
        healthRegenRate = rate;
    }

}
