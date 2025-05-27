
using UnityEngine;

public class BuildingSpot : MonoBehaviour, IInteractable
{
    [Header("Building Settings")]
    public string buildingName = "��";
    public GameObject buildingObject; // ���� �ǹ� (Cube)

    [Header("Resource Requirements")]
    public ItemData requiredResource; // ���� ItemData
    public int requiredAmount = 5;    // �ʿ��� ����

    [Header("Buff Effects")]
    public BuffType buffType = BuffType.HealthRegen;
    public float buffValue = 0.5f; // ü�� ȸ�� +0.5/��

    [Header("Materials")]
    public Material previewMaterial;  // ������ �̸����� ��Ƽ����
    public Material completeMaterial; // �ϼ��� �ǹ� ��Ƽ����

    private bool isBuilt = false;
    private Renderer buildingRenderer;

    void Start()
    {
        // �ǹ� ������ ��������
        if (buildingObject != null)
        {
            buildingRenderer = buildingObject.GetComponent<Renderer>();
        }

        // �ǹ��� �̸����� ���� ����
        SetBuildingPreviewMode();
    }

    void SetBuildingPreviewMode()
    {
        if (buildingRenderer != null && previewMaterial != null)
        {
            buildingRenderer.material = previewMaterial;
            Debug.Log($"{buildingName} �Ǽ� ��ġ - �̸����� ��� ������");
        }
        else
        {
            Debug.LogWarning("�̸����� ��Ƽ������ �������� �ʾҽ��ϴ�!");
        }
    }

    public string GetInteractPrompt()
    {
        if (isBuilt)
        {
            return $"{buildingName}\n�̹� �Ǽ��� �ǹ��Դϴ�.";
        }

        return $"{buildingName} �Ǽ�\n�ʿ� �ڿ�: {requiredResource.displayName} {requiredAmount}��\n[E] �Ǽ��ϱ�";
    }

    public void OnInteract()
    {
        if (isBuilt)
        {
            Debug.Log("�̹� �Ǽ��� �ǹ��Դϴ�.");
            return;
        }

        // �κ��丮���� �ڿ� Ȯ��
        if (HasEnoughResources())
        {
            // �ڿ� �Ҹ�
            ConsumeResources();

            // �ǹ� �Ǽ�
            ConstructBuilding();
        }
        else
        {
            // �ڿ� ���� �޽���
            ShowInsufficientResourcesMessage();
        }
    }

    bool HasEnoughResources()
    {
        // TODO: ���� �κ��丮 �ý��۰� ����
        // ����� �ӽ÷� true ��ȯ (�׽�Ʈ��)
        Debug.Log($"�ڿ� Ȯ��: {requiredResource.displayName} {requiredAmount}�� �ʿ�");
        return true; // �׽�Ʈ�� - ���߿� ���� �κ��丮 üũ�� ��ü
    }

    void ConsumeResources()
    {
        // TODO: ���� �κ��丮���� ������ ����
        Debug.Log($"�ڿ� �Ҹ�: {requiredResource.displayName} {requiredAmount}��");
    }

    void ConstructBuilding()
    {
        Debug.Log($"{buildingName} �Ǽ� �Ϸ�! ");

        // �ǹ� ���� ����
        isBuilt = true;

        // �ǹ��� �ϼ��� ������� ���� (SetActive ����)
        SetBuildingCompleteMode();

        // ���� ����
        ApplyBuff();
    }

    void SetBuildingCompleteMode()
    {
        if (buildingRenderer != null && completeMaterial != null)
        {
            buildingRenderer.material = completeMaterial;
            Debug.Log("�ǹ��� �ϼ��� ������� �����!");
        }
        else
        {
            Debug.LogWarning("�ϼ� ��Ƽ������ �������� �ʾҽ��ϴ�!");
        }
    }

    void ApplyBuff()
    {
        PlayerBuffManager buffManager = CharacterManager.Instance.Player.GetComponent<PlayerBuffManager>();
        if (buffManager == null)
        {
            // PlayerBuffManager�� ������ �߰�
            buffManager = CharacterManager.Instance.Player.gameObject.AddComponent<PlayerBuffManager>();
        }

        BuffEffect buff = new BuffEffect
        {
            buffType = buffType,
            value = buffValue,
            description = $"{buildingName} ȿ��: ü�� ȸ�� +{buffValue}/��"
        };

        buffManager.AddBuff(buff);
        Debug.Log($"���� ����: {buff.description}");
    }

    void ShowInsufficientResourcesMessage()
    {
        Debug.Log($"�ڿ��� �����մϴ�! {requiredResource.displayName} {requiredAmount}���� �ʿ��մϴ�.");
        // TODO: UI �޽��� �˾� ǥ��
    }
}