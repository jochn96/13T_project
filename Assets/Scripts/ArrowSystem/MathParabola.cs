using UnityEngine;

// 포물선 계산을 위한 수학 유틸리티 클래스
public class MathParabola
{
    private static float xpos;
    private static float zpos;

    /// <summary>
    /// 시작점과 끝점 사이의 포물선 경로상의 점을 계산
    /// </summary>
    /// <param name="start">시작 위치</param>
    /// <param name="end">끝 위치</param>
    /// <param name="height">포물선의 높이</param>
    /// <param name="t">시간 (0~1 사이의 값)</param>
    /// <returns>포물선 경로상의 위치</returns>
    public static Vector3 Parabola(Vector3 start, Vector3 end, float height, float t)
    {
        // 포물선 함수: f(x) = -4 * height * x^2 + 4 * height * x
        float f(float x) => -4 * height * x * x + 4 * height * x;

        if (t > 0)
        {
            // X축과 Z축 위치를 시간에 따라 선형 보간
            xpos = (end.x - start.x) * t + start.x;
            zpos = (end.z - start.z) * t + start.z;
        }

        // 포물선 Y값과 Y축 선형 보간을 결합하여 최종 위치 반환
        return new Vector3(xpos, f(t) + Mathf.Lerp(start.y, end.y, t), zpos);
    }
}