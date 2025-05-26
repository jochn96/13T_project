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
    HealthRegen,     // ü�� ȸ�� �ӵ�
    MaxHealth,       // �ִ� ü��
    MoveSpeed,       // �̵� �ӵ�
    StaminaRegen,    // ���¹̳� ȸ��
    ResourceGather   // �ڿ� ä�� �ӵ�
}

public class PlayerBuffManager : MonoBehaviour
{
    [Header("Active Buffs")]
    public List<BuffEffect> activeBuffs = new List<BuffEffect>();

    private PlayerCondition playerCondition;
    private float originalHealthRegen = 1f; // �⺻ ü�� ȸ����

    void Start()
    {
        playerCondition = GetComponent<PlayerCondition>();

        // �⺻ ü�� ȸ�� ����
        InvokeRepeating("ApplyHealthRegen", 1f, 1f);
    }

    public void AddBuff(BuffEffect buff)
    {
        activeBuffs.Add(buff);
        Debug.Log($"���� �߰���: {buff.description}");

        // ��� ȿ�� ���� (�ʿ��� ���)
        ApplyBuffEffects();
    }

    public void RemoveBuff(BuffEffect buff)
    {
        activeBuffs.Remove(buff);
        ApplyBuffEffects();
    }

    void ApplyBuffEffects()
    {
        // ������ ȿ�� ���� ����
        // ���⼭�� ü�� ȸ���� ����
    }

    void ApplyHealthRegen()
    {
        if (playerCondition != null)
        {
            float totalRegen = originalHealthRegen;

            // ��� ü�� ȸ�� ���� �ջ�
            foreach (BuffEffect buff in activeBuffs)
            {
                if (buff.buffType == BuffType.HealthRegen)
                {
                    totalRegen += buff.value;
                }
            }

            // ü�� ȸ�� ����
            playerCondition.Heal(totalRegen);
        }
    }
}

