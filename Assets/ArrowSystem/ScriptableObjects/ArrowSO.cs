using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ArrowSO", order = 1)]
public class ArrowSO : ScriptableObject
{
    public float DisableTrailEmissionTime = 0.01f;  // 화살이 도착했을 때 트레일 방출이 비활성화되기까지의 시간 (초) [기본값 = 0.01]
    public float TrailFadeoutTime = 0.4f;           // 화살이 도착했을 때 남은 트레일 생존시간 (초) [기본값 = 0.4]
    public float DisableTrailTime = 0.8f;           // 화살이 도착한 후 트레일이 완전히 비활성화되기까지의 시간 (초) [기본값 = 0.8]
    [Range(0, 1)]
    public float StuckDepthMin = 0.25f;             // 화살이 박힐 수 있는 최소 깊이. 0 = 화살 끝 / 들어가지 않음. [기본값 = 0.25]
    [Range(0, 1)]
    public float StuckDepthMax = 0.6f;              // 화살이 박힐 수 있는 최대 깊이. [기본값 = 0.6]
    public LayerMask CollisionLayerMask;            // 화살 충돌을 위한 LayerMask
}