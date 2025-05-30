using UnityEngine;

public class ArrowController : MonoBehaviour
{
    [Header("트레일")]
    public TrailRenderer trailRenderer; // 프리팹에서 설정해야 하며, 또는 Start 메서드에서 설정

    public ArrowSO ArrowSO;             // 화살의 대부분 설정을 담고 있는 ScriptableObject
    public GameObject MeshParent;       // 메시들의 부모 GameObject

    private Vector3 target;             // 화살이 맞추려고 하는 위치
    private Collider ownerCollider;     // 화살을 발사하는 GameObject의 Collider
    private float flightSpeed;          // 화살의 속도
    private float heightMultiplier;     // 비행 경로의 포물선 높이 (화살이 더 높은 호를 그림)
    private float lifeTime;             // 사망까지의 시간 (초)

    private float flightTimer;          // 비행 계산에 사용되는 타이머
    private Vector3 startPoint;         // 화살이 발사된 위치
    private float targetDistance;       // startPoint에서 타겟까지의 거리
    private float speedToDistance;      // 거리에 대한 화살의 속도 (거리에 관계없이 화살에게 일정한 속도를 줌)
    private Vector3 lastPosition;       // 지난 FixedUpdate 단계에서의 화살 위치
    private bool readyToFly;            // 화살이 무언가에 맞았는지 여부
    private bool isInitialized;         // 화살이 비행을 시작하기 위한 조건
    private bool hasArrived = false;    // 화살이 이미 박혔는지 여부 (중복 충돌 방지)

    private void Awake()
    {
        // 화살 초기화
        target = Vector3.zero;
        startPoint = transform.position;
        lastPosition = transform.position;
        readyToFly = false;
        isInitialized = false;

        // 경로를 시작하기 전 프레임에서 화살이 보이지 않도록 모든 렌더러 비활성화
        MeshRenderer[] meshRenderers = transform.GetComponentsInChildren<MeshRenderer>();
        foreach (var renderer in meshRenderers)
        {
            renderer.enabled = false;
        }
    }

    private void Start()
    {
        // x(LifeTime)초 후에 화살이 스스로 파괴되도록 설정
        Destroy(gameObject, lifeTime);

        // 타겟까지의 거리 계산
        targetDistance = Vector3.Distance(transform.position, target);

        // 거리에 상대적인 비행 속도 계산
        speedToDistance = flightSpeed / targetDistance * flightSpeed;
    }

    private void FixedUpdate()
    {
        if (isInitialized)
        {
            // 렌더러 다시 활성화
            MeshRenderer[] meshRenderers = transform.GetComponentsInChildren<MeshRenderer>();
            foreach (var renderer in meshRenderers)
            {
                renderer.enabled = true;
            }

            readyToFly = true;
            isInitialized = false;
        }

        if (readyToFly && target != Vector3.zero && !hasArrived)
        {
            flightTimer += Time.deltaTime;

            SoundManager.Instance.PlaySFX("arrow_sound");
            // 포물선을 따라 화살 이동 (더 평평한 궤도로 수정)
            float adjustedHeight = (targetDistance / 10f) * heightMultiplier; // 기존 5f에서 10f로 변경하여 높이 절반으로 감소
            transform.position = MathParabola.Parabola(startPoint, target, adjustedHeight, flightTimer * speedToDistance);

            // 화살이 현재 날아가고 있는 방향
            Vector3 direction = transform.position - lastPosition;

            // 레이캐스트를 사용한 충돌 감지
            RaycastHit hit = new RaycastHit();
            Ray ray = new Ray(lastPosition, direction);
            // "Projectile" 태그가 붙지 않은 오브젝트와 소유자가 아닌 오브젝트와만 충돌
            if (Physics.Raycast(ray, out hit, direction.magnitude, ArrowSO.CollisionLayerMask, QueryTriggerInteraction.Ignore))
            {
                // 여기서 화살이 무시해야 할 더 많은 것들을 추가할 수 있음
                // 플레이어 태그도 무시하도록 추가
                if (!hit.collider.CompareTag("Projectile") &&
                    !hit.collider.CompareTag("Player") &&
                    hit.collider != ownerCollider)
                {
                    Arrive(hit);
                    return;
                }
            }

            // 화살 회전
            transform.rotation = Quaternion.LookRotation(direction);

            // lastPosition 업데이트
            lastPosition = transform.position;
        }
    }

    /// <summary>
    /// 화살 발사
    /// </summary>
    /// <param name="target">화살이 발사될 타겟</param>
    /// <param name="owner">화살을 발사하는 GameObject</param>
    /// <param name="flightSpeed">화살의 속도</param>
    /// <param name="heightMultiplier">비행 경로의 포물선 높이</param>
    /// <param name="lifeTime">화살이 파괴되기까지의 시간</param>
    public void Shoot(Vector3 target, GameObject owner, float flightSpeed, float heightMultiplier, float lifeTime)
    {
        this.target = target;
        ownerCollider = owner.GetComponent<Collider>();
        this.flightSpeed = flightSpeed;
        this.heightMultiplier = heightMultiplier;
        this.lifeTime = lifeTime;

        // 다음 FixedUpdate 단계에서 화살 비행 시작
        isInitialized = true;
    }

    private void Arrive(RaycastHit hit)
    {
        // 이미 박혔으면 더 이상 처리하지 않음
        if (hasArrived) return;

        // 화살이 도착했다고 표시
        hasArrived = true;
        readyToFly = false;

        // 화살 자체의 콜라이더 처리
        Collider arrowCollider = GetComponent<Collider>();
        if (arrowCollider != null)
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                // Enemy에 박힌 경우: isTrigger만 비활성화 (상호작용 유지)
                arrowCollider.isTrigger = false;
            }
            else
            {
                // Enemy가 아닌 곳에 박힌 경우: 콜라이더 완전 제거
                Destroy(arrowCollider);
            }
        }

        // 박혔을 때 트레일 방출 중지 (박힌 화살이 부모(예: 적)와 함께 움직일 때 트레일을 방출하지 않도록)
        // 원하지 않는 아티팩트를 피하기 위해 짧은 지연으로 수행됨
        Invoke("DisableTrailEmission", ArrowSO.DisableTrailEmissionTime);
        // 트레일이 빠르게 페이드 아웃되도록 하고 비활성화
        trailRenderer.time = ArrowSO.TrailFadeoutTime;
        Invoke("DisableTrail", ArrowSO.DisableTrailTime);

        // 화살을 랜덤한 깊이로 박히게 함
        // 화살이 오브젝트에 얼마나 깊이 박히는지 변경하려면 이 값들을 수정 (0은 화살 끝)
        transform.position = hit.point += transform.forward * Random.Range(ArrowSO.StuckDepthMin, ArrowSO.StuckDepthMax);

        // 화살을 맞은 오브젝트의 자식으로 만듦 (박힌 화살이 부모와 함께 움직이게 함)
        MakeChildOfHitObject(hit.transform);
    }

    // 화살 트레일 방출 비활성화
    private void DisableTrailEmission()
    {
        trailRenderer.emitting = false;
    }

    // 화살 트레일 비활성화
    private void DisableTrail()
    {
        trailRenderer.enabled = false;
    }

    private void MakeChildOfHitObject(Transform parentTransform)
    {
        // 오브젝트가 '부모가 되기에' 적합한 경우에만 화살을 자식으로 만듦
        // 자세한 내용은 문서 참조
        if (IsSuitedParent(parentTransform))
        {
            Quaternion originalRotation = transform.rotation;

            // 부모로 만들 때 메시 변형을 없애기 위해 화살의 회전 리셋
            transform.rotation = new Quaternion();

            // 부모 설정하고 월드 위치 유지
            transform.SetParent(parentTransform, true);

            // 메시와 트레일 회전
            MeshParent.transform.rotation = originalRotation;
        }
    }

    // transform이 화살의 부모가 되기에 적합한지 확인
    private bool IsSuitedParent(Transform parent)
    {
        if (IsUniformScaled(parent) || IsUniformRotated(parent))
            return true;
        else
        {
            // 부모가 균등하지 않게 스케일되고 회전되어 있을 때 자식을 주면 화살의 이상한 메시 변형이 발생함
            return false;
        }
    }

    private bool IsUniformScaled(Transform parent)
    {
        // x, y, z가 모두 같으면 스케일이 균등함을 의미
        if (parent.localScale.x == parent.localScale.y && parent.localScale.x == parent.localScale.z)
            return true;
        else
            return false;
    }

    private bool IsUniformRotated(Transform parent)
    {
        var rotation = parent.rotation.eulerAngles;

        // x, y, z가 모두 같으면 회전이 균등함을 의미
        if (parent.rotation.x == parent.rotation.y && parent.rotation.x == parent.rotation.z)
            return true;
        else
        {
            // 각 축이 90°의 배수이면 균등하기도 함 (또는 최소한 적합함)
            if (Mathf.Round(rotation.x) % 90f == 0 && Mathf.Round(rotation.y) % 90f == 0 && Mathf.Round(rotation.z) % 90f == 0)
                return true;
            else
                return false;
        }
    }
}