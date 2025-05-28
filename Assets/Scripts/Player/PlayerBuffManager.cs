using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class BuffEffect
{
    public BuffType buffType;
    public float value;
    public string description;
}

public enum BuffType
{
    HealthRegen,     // ??? ??? ???
    MaxHealth,       // ??? ???
    MoveSpeed,       // ??? ???
    StaminaRegen,    // ???©ö?? ???
    ResourceGather   // ??? ??? ???
}

public class PlayerBuffManager : MonoBehaviour
{
    [Header("Active Buffs")]
    public List<BuffEffect> activeBuffs = new List<BuffEffect>();

    private PlayerCondition playerCondition;
    private float originalHealthRegen = 1f; // ?? ??? ?????

    void Start()
    {
        playerCondition = GetComponent<PlayerCondition>();

        // ?? ??? ??? ????
        InvokeRepeating("ApplyHealthRegen", 1f, 1f);
    }

    public void AddBuff(BuffEffect buff)
    {
        activeBuffs.Add(buff);
        Debug.Log($"???? ?????: {buff.description}");

        // ??? ??? ???? (????? ???)
        ApplyBuffEffects();
    }

    public void RemoveBuff(BuffEffect buff)
    {
        activeBuffs.Remove(buff);
        ApplyBuffEffects();
    }

    void ApplyBuffEffects()
    {
        // ?????? ??? ???? ????
        // ?????? ??? ????? ????
    }

    void ApplyHealthRegen()
    {
        if (playerCondition != null)
        {
            float totalRegen = originalHealthRegen;

            // ??? ??? ??? ???? ???
            foreach (BuffEffect buff in activeBuffs)
            {
                if (buff.buffType == BuffType.HealthRegen)
                {
                    totalRegen += buff.value;
                }
            }

            // ??? ??? ????
            playerCondition.Heal(totalRegen);
        }
    }
}

