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
    ResourceGather,  // 자원 채집 속도
    WaterRegen       // 갈증 회복 속도 (우물용)
}

public class PlayerBuffManager : MonoBehaviour
{
    [Header("Active Buffs")]
    public List<BuffEffect> activeBuffs = new List<BuffEffect>();

    private PlayerCondition playerCondition;
    private float originalHealthRegen = 1f; // 기본 체력 회복량
    private float originalWaterRegen = 0f;  // 기본 갈증 회복량 (평소엔 0)

    void Start()
    {
        playerCondition = GetComponent<PlayerCondition>();

        // 기본 체력 회복 시작
        InvokeRepeating("ApplyHealthRegen", 1f, 1f);

        // 갈증 회복 시작 (우물 건설시 적용)
        InvokeRepeating("ApplyWaterRegen", 1f, 1f);
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
        // 여기서는 체력/갈증 회복만 구현
        Debug.Log($"현재 활성 버프 개수: {activeBuffs.Count}");
    }

    // 체력 회복 처리 (기존)
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
            if (totalRegen > 0)
            {
                playerCondition.Heal(totalRegen);
                Debug.Log($"체력 회복: +{totalRegen}");
            }
        }
    }

    // 갈증 회복 처리 (우물용 새로 추가)
    void ApplyWaterRegen()
    {
        if (playerCondition != null)
        {
            float totalWaterRegen = originalWaterRegen;

            // 모든 갈증 회복 버프 합산
            foreach (BuffEffect buff in activeBuffs)
            {
                if (buff.buffType == BuffType.WaterRegen)
                {
                    totalWaterRegen += buff.value;
                }
            }

            // 갈증 회복 적용
            if (totalWaterRegen > 0)
            {
                playerCondition.DrinkWater(totalWaterRegen);
                Debug.Log($"갈증 회복: +{totalWaterRegen}");
            }
        }
    }

    // 이동 속도 버프 적용 (선택사항. 이동속도가 필요해지면 주석 해제하고 넣겠습니다.)
    /*void ApplyMoveSpeedBuff()
    {
        var playerController = GetComponent<PlayerController>();
        if (playerController != null)
        {
            float speedMultiplier = 1f;

            foreach (BuffEffect buff in activeBuffs)
            {
                if (buff.buffType == BuffType.MoveSpeed)
                {
                    speedMultiplier += buff.value;
                }
            }

            // 이동 속도 적용 (PlayerController에 SetMoveSpeed 메서드가 있다면)
            // playerController.SetMoveSpeed(playerController.baseMoveSpeed * speedMultiplier);
        }
    }*/

    // 현재 버프 상태 확인 (디버그용)
    void Update()
    {
        // B키로 현재 버프 상태 확인
        if (Input.GetKeyDown(KeyCode.B))
        {
            ShowBuffStatus();
        }
    }

    // 버프 상태 출력
    void ShowBuffStatus()
    {
        Debug.Log("=== 현재 활성 버프 ===");

        if (activeBuffs.Count == 0)
        {
            Debug.Log("활성 버프 없음");
            return;
        }

        foreach (BuffEffect buff in activeBuffs)
        {
            Debug.Log($"• {buff.description}");
        }

        Debug.Log("==================");
    }

    // 특정 버프 타입이 있는지 확인
    public bool HasBuff(BuffType buffType)
    {
        foreach (BuffEffect buff in activeBuffs)
        {
            if (buff.buffType == buffType)
            {
                return true;
            }
        }
        return false;
    }

    // 특정 버프 타입의 총 효과량 계산
    public float GetBuffValue(BuffType buffType)
    {
        float totalValue = 0f;

        foreach (BuffEffect buff in activeBuffs)
        {
            if (buff.buffType == buffType)
            {
                totalValue += buff.value;
            }
        }

        return totalValue;
    }
}