using UnityEngine;

public class EquipBow : Equip
{
    [Header("활 설정")]
    public GameObject ArrowPrefab;                  // 발사할 화살 프리팹
    public float MaximalShootRange = 100f;          // 최대 사격 거리
    public float MinimalShootRange = 4f;            // 최소 사격 거리
    [Range(0, 10)]
    public float SpreadFactor = 0.5f;               // 정확도 (낮을수록 정확함)
    [Range(0f, 0.4f)]
    public float SpreadFactorDistanceImpact = 0.1f; // 거리가 정확도에 미치는 영향
    [Range(0f, 3f)]
    public float HeightMultiplier = 0.8f;           // 비행 경로의 포물선 높이 (낮을수록 직선에 가까움)
    public float ArrowFlightSpeed = 6f;             // 화살의 속도
    public float ArrowLifeTime = 120f;              // 화살이 파괴되기까지의 시간 (초)

    [Header("발사 위치")]
    public Transform arrowSpawnPoint;               // 화살 생성 위치 (활의 시위 부분)

    private Camera playerCamera;

    public Animator animator;

    private void Start()
    {
        playerCamera = Camera.main;

        // arrowSpawnPoint가 설정되지 않았으면 현재 Transform 사용
        if (arrowSpawnPoint == null)
            arrowSpawnPoint = transform;
    }

    public override void OnAttackInput()
    {
        // 인벤토리에 화살이 있는지 확인
        if (UIInventory.Instance?.GetArrowCount() <= 0)
        {
            Debug.Log("화살이 없습니다!");
            return;
        }

        animator.SetTrigger("Attack");
    }

    public void TryToShoot()
    {
        // 마우스 위치로 레이캐스트해서 타겟 지점 찾기
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // "Projectile" 태그가 붙은 오브젝트는 무시 (화살끼리 충돌 방지)
            if (!hit.collider.CompareTag("Bullet"))
            {
                ShootArrow(hit.point);
            }
        }
        else
        {
            // 레이가 아무것도 맞지 않으면 멀리 있는 지점으로 발사
            Vector3 farPoint = playerCamera.transform.position + playerCamera.transform.forward * MaximalShootRange;
            ShootArrow(farPoint);
        }
    }

    private void ShootArrow(Vector3 targetPos)
    {
        // 사격 가능 거리 체크
        var distance = Vector3.Distance(arrowSpawnPoint.position, targetPos);
        if (distance < MinimalShootRange)
        {
            Debug.Log("너무 가까운 목표입니다!");
            return;
        }

        if (distance > MaximalShootRange)
        {
            // 최대 거리로 제한
            Vector3 direction = (targetPos - arrowSpawnPoint.position).normalized;
            targetPos = arrowSpawnPoint.position + direction * MaximalShootRange;
            distance = MaximalShootRange;
        }

        // 인벤토리에서 화살 소비
        if (!UIInventory.Instance.ConsumeArrow())
        {
            Debug.Log("화살 소비에 실패했습니다!");
            return;
        }

        // 거리에 따른 정확도 계산
        float spreadFactorByDistance = SpreadFactor * (1f + (SpreadFactorDistanceImpact * distance));

        // 부정확한 타겟 계산 (원래 타겟 주변 어딘가)
        Vector3 inaccurateTarget = (Random.insideUnitSphere * spreadFactorByDistance) + targetPos;

        // 새로운 화살 생성
        var Arrow = Instantiate(ArrowPrefab, arrowSpawnPoint.position, arrowSpawnPoint.rotation);

        // 화살 이름 설정
        Arrow.name = "Arrow";

        // ArrowController에게 발사 명령
        var arrowController = Arrow.GetComponent<ArrowController>();
        if (arrowController != null)
        {
            arrowController.Shoot(inaccurateTarget, gameObject, ArrowFlightSpeed, HeightMultiplier, ArrowLifeTime);
        }
        else
        {
            Debug.LogError("화살 프리팹에 ArrowController 컴포넌트가 없습니다!");
            Destroy(Arrow);
        }

        Debug.Log($"화살 발사! 남은 화살: {UIInventory.Instance.GetArrowCount()}개");
    }
}
