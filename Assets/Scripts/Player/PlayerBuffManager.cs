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
    HealthRegen,     // 체력 회복 속도
    MaxHealth,       // 최대 체력
    MoveSpeed,       // 이동 속도
    StaminaRegen,    // 스태미나 회복
    ResourceGather   // 자원 채집 속도
}

public class PlayerBuffManager : MonoBehaviour
{
    [Header("Active Buffs")]
    public List<BuffEffect> activeBuffs = new List<BuffEffect>();

    private PlayerCondition playerCondition;
    private float originalHealthRegen = 1f; // 기본 체력 회복량

    void Start()
    {
        playerCondition = GetComponent<PlayerCondition>();

        // 기본 체력 회복 시작
        InvokeRepeating("ApplyHealthRegen", 1f, 1f);
    }

    public void AddBuff(BuffEffect buff)
    {
        activeBuffs.Add(buff);
        Debug.Log($"버프 추가됨: {buff.description}");

        // 즉시 효과 적용 (필요한 경우)
        ApplyBuffEffects();
    }

    public void RemoveBuff(BuffEffect buff)
    {
        activeBuffs.Remove(buff);
        ApplyBuffEffects();
    }

    void ApplyBuffEffects()
    {
        // 버프별 효과 적용 로직
        // 여기서는 체력 회복만 구현
    }

    void ApplyHealthRegen()
    {
        if (playerCondition != null)
        {
            float totalRegen = originalHealthRegen;

            // 모든 체력 회복 버프 합산
            foreach (BuffEffect buff in activeBuffs)
            {
                if (buff.buffType == BuffType.HealthRegen)
                {
                    totalRegen += buff.value;
                }
            }

            // 체력 회복 적용
            playerCondition.Heal(totalRegen);
        }
    }
}

