using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;

public class BuildingSpot : MonoBehaviour, IInteractable
{
    [Header("Building Settings")]
    public string buildingName = "집";
    public GameObject buildingObject; // 실제 건물 (Cube)

    [Header("Resource Requirements")]
    public ItemData requiredResource; // 나무 ItemData
    public int requiredAmount = 5;    // 필요한 개수

    [Header("Buff Effects")]
    public BuffType buffType = BuffType.HealthRegen;
    public float buffValue = 0.5f; // 체력 회복 +0.5/초

    [Header("Materials")]
    public Material previewMaterial;  // 반투명 미리보기 머티리얼
    public Material completeMaterial; // 완성된 건물 머티리얼

    private bool isBuilt = false;
    private Renderer buildingRenderer;

    void Start()
    {
        // 건물 렌더러 가져오기
        if (buildingObject != null)
        {
            buildingRenderer = buildingObject.GetComponent<Renderer>();
        }

        // 건물을 미리보기 모드로 설정
        SetBuildingPreviewMode();
    }

    void SetBuildingPreviewMode()
    {
        if (buildingRenderer != null && previewMaterial != null)
        {
            buildingRenderer.material = previewMaterial;
            Debug.Log($"{buildingName} 건설 위치 - 미리보기 모드 설정됨");
        }
        else
        {
            Debug.LogWarning("미리보기 머티리얼이 설정되지 않았습니다!");
        }
    }

    public string GetInteractPrompt()
    {
        if (isBuilt)
        {
            return $"{buildingName}\n이미 건설된 건물입니다.";
        }

        return $"{buildingName} 건설\n필요 자원: {requiredResource.displayName} {requiredAmount}개\n[E] 건설하기";
    }

    public void OnInteract()
    {
        if (isBuilt)
        {
            Debug.Log("이미 건설된 건물입니다.");
            return;
        }

        // 인벤토리에서 자원 확인
        if (HasEnoughResources())
        {
            // 자원 소모
            ConsumeResources();

            // 건물 건설
            ConstructBuilding();
        }
        else
        {
            // 자원 부족 메시지
            ShowInsufficientResourcesMessage();
        }
    }

    bool HasEnoughResources()
    {
        // TODO: 실제 인벤토리 시스템과 연결
        // 현재는 임시로 true 반환 (테스트용)
        Debug.Log($"자원 확인: {requiredResource.displayName} {requiredAmount}개 필요");
        return true; // 테스트용 - 나중에 실제 인벤토리 체크로 교체
    }

    void ConsumeResources()
    {
        // TODO: 실제 인벤토리에서 아이템 제거
        Debug.Log($"자원 소모: {requiredResource.displayName} {requiredAmount}개");
    }

    void ConstructBuilding()
    {
        Debug.Log($"{buildingName} 건설 완료! ");

        // 건물 상태 변경
        isBuilt = true;

        // 건물을 완성된 모습으로 변경 (SetActive 제거)
        SetBuildingCompleteMode();

        // 버프 적용
        ApplyBuff();
    }

    void SetBuildingCompleteMode()
    {
        if (buildingRenderer != null && completeMaterial != null)
        {
            buildingRenderer.material = completeMaterial;
            Debug.Log("건물이 완성된 모습으로 변경됨!");
        }
        else
        {
            Debug.LogWarning("완성 머티리얼이 설정되지 않았습니다!");
        }
    }

    void ApplyBuff()
    {
        PlayerBuffManager buffManager = CharacterManager.Instance.Player.GetComponent<PlayerBuffManager>();
        if (buffManager == null)
        {
            // PlayerBuffManager가 없으면 추가
            buffManager = CharacterManager.Instance.Player.gameObject.AddComponent<PlayerBuffManager>();
        }

        BuffEffect buff = new BuffEffect
        {
            buffType = buffType,
            value = buffValue,
            description = $"{buildingName} 효과: 체력 회복 +{buffValue}/초"
        };

        buffManager.AddBuff(buff);
        Debug.Log($"버프 적용: {buff.description}");
    }

    void ShowInsufficientResourcesMessage()
    {
        Debug.Log($"자원이 부족합니다! {requiredResource.displayName} {requiredAmount}개가 필요합니다.");
        // TODO: UI 메시지 팝업 표시
    }
}