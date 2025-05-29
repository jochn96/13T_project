using System;

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

    //public void RestoreFromItem(ItemData item)
    //{
    //    if (item.type != ItemType.Consumable || item.consumables.Length == 0) return;

    //    var effect = item.consumables[0]; //단일회복효과, 체력이면 체력, 허기면 허기, 수분이면 수분 회복
    //    {
    //        switch (effect.type)
    //        {
    //            case ConsumableType.Health:
    //                health.Add(effect.value); // 체력 회복
    //                break;
    //            case ConsumableType.Hunger:
    //                hunger.Add(effect.value); // 허기 회복
    //                break;
    //            case ConsumableType.Water:
    //                water.Add(effect.value); // 수분 회복
    //                break;
    //        }
    //    }
    //}
    
    public event Action onTakeDamage;
    /*private void Update()
    {
<<<<<<< HEAD
        if (IsPlayerMoving())
        {
            hunger.Subject(hunger.passiveValue * Time.deltaTime);
            water.Subject(water.passiveValue * Time.deltaTime);
        }
=======
        //hunger.Subject(hunger.passiveValue * Time.deltaTime);
        //water.Subject(water.passiveValue * Time.deltaTime);
>>>>>>> dev

 

        //if(hunger.curValue < 0f)
        //{
        //    health.Subject(noHungerHealthDecay * Time.deltaTime);
        //}
        //if(health.curValue < 0f)
        //{
        //    Die();
        //}
    } */ 

<<<<<<< HEAD
    private bool IsPlayerMoving()
    {
        Vector3 velocity = CharacterManager.Instance.Player.controller.GetMoveDirection();
        return velocity.magnitude > 0.1f; // 이동 중이면 true
    }
=======
>>>>>>> dev

    //private void OnTriggerEnter(Collider other)
    //{
    //    Enemy 태그 오브젝트와 충돌시 대미지
    //    if (other.CompareTag("Enemy"))
    //    {
    //        TakePhysiclaDamage(10); //임시 고정 대미지
    //    }
    //} */

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
