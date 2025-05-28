
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [Range(0.0f, 1.0f)]// 하루를 0~1 로 나타냄
    public float time;
    public float fullDayLength; // 하루가 몇초로 되는지
    public float startTime = 0.4f; // 0~1 사이 기준으로 하루시간이 0.4에서 시작됨
    private float timeRate; // 시간이 가는 속도비율
    public Vector3 noon = new Vector3(90, 0, 0);//Vector 90 0 0 해,달의 회전 기준값

    [Header("Sun")]
    public Light sun;
    public Gradient sunColor;
    public AnimationCurve sunIntensity;

    [Header("Moon")]
    public Light moon;
    public Gradient moonColor;
    public AnimationCurve moonIntensity;
    // light 는 광원, Gradient는 색상, Curve는 밝기
    
    [Header("Other Lighting")] 
    public AnimationCurve lightingIntensityMultiplier;      //환경광
    public AnimationCurve reflectionIntensityMultiplier;    //반사광

    private void Start()
    {
        timeRate = 1.0f / fullDayLength;    // 1초당 설정한 속도만큼 시간이 간다
        time = startTime;

        if (moon != null)
        {
            moon.transform.eulerAngles = noon;
        }
    }

    private void Update()
    {
        time = (time + timeRate * Time.deltaTime) % 1.0f;

        UpdateSunLighting();
        UpdateMoonLighting();

        RenderSettings.ambientIntensity = lightingIntensityMultiplier.Evaluate(time);
        RenderSettings.reflectionIntensity = reflectionIntensityMultiplier.Evaluate(time);
    }

    void UpdateSunLighting()
    {
        if (sun == null) return;
        
        float intensity = sunIntensity.Evaluate(time);
        Vector3 euler = (time - 0.25f) * noon * 4f;
        sun.transform.eulerAngles = euler;
        sun.color = sunColor.Evaluate(time);
        sun.intensity = intensity;

        if (intensity <= 0.01f && sun.gameObject.activeInHierarchy)
            sun.gameObject.SetActive(false);
        else if (intensity > 0.01f && !sun.gameObject.activeInHierarchy)
            sun.gameObject.SetActive(true);
    }
    void UpdateMoonLighting()
    {
        if (moon == null) return;

        float intensity = moonIntensity.Evaluate(time);
        intensity = Mathf.Max(intensity, 0.05f);
        moon.color = moonColor.Evaluate(time);
        moon.intensity = intensity;

        // 태양의 각도를 기준으로 달의 활성화/비활성화 결정
        float sunAngle = ((time - 0.25f) * 360f) % 360f;
        if ((sunAngle >= 269f && sunAngle <= 271f) && moon.gameObject.activeInHierarchy)
            moon.gameObject.SetActive(false);
        else if (!(sunAngle >= 269f && sunAngle <= 271f) && !moon.gameObject.activeInHierarchy)
            moon.gameObject.SetActive(true);
    }
}

