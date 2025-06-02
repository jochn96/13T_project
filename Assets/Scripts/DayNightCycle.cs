using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class DayNightCycle : MonoBehaviour
{
    [Range(0.0f, 1.0f)]
    public float time;
    public float fullDayLenth;
    public float startTime = 0.2f;
    private float timeRate;
    public Vector3 noon; //Vector 90 0 0

    [Header("Sun")]
    public Light sun;
    public Gradient sunColor;
    public AnimationCurve sunIntensity;

    [Header("Moon")]
    public Light moon;
    public Gradient moonColor;
    public AnimationCurve moonIntensity;

    [Header("Other Lighting")]
    public AnimationCurve lightingIntensityMultiplier;
    public AnimationCurve reflectionIntensityMultiplier;

    [Header("BGM")]
    private bool isDayBGMPlaying = true; 
    
    private void Start()
    {
        timeRate = 1.0f / fullDayLenth;
        time = startTime;
    }

    private void Update()
    {
        time = (time + timeRate * Time.deltaTime) % 1.0f;

        UpdateLighting(sun, sunColor, sunIntensity);
        UpdateLighting(moon, moonColor, moonIntensity);

        RenderSettings.ambientIntensity = lightingIntensityMultiplier.Evaluate(time);
        RenderSettings.reflectionIntensity = reflectionIntensityMultiplier.Evaluate(time);

        //if (time >= 0.25f && time < 0.75f)// 0.25 > 정오, 0.75 > 자정 / 그렇기때문에 0.25~ 0.75를 낮이라고 판단
        //{
        //    if (!isDayBGMPlaying)
        //    {
        //        SoundManager.Instance.PlayBGM("BGM_day");
        //        isDayBGMPlaying = true;
        //    }
        //}
        //else
        //{
        //    if (isDayBGMPlaying)
        //    {
        //        SoundManager.Instance.PlayBGM("BGM_night");
        //        isDayBGMPlaying = false;
        //    }
        //}
        
    }

    void UpdateLighting(Light lightSource, Gradient gradient, AnimationCurve intencityCurve)
    {
        float intensity = intencityCurve.Evaluate(time);

        lightSource.transform.eulerAngles = (time - (lightSource == sun ? 0.25f : 0.75f)) * noon * 4f;
        lightSource.color = gradient.Evaluate(time);
        lightSource.intensity = intensity;

        GameObject go = lightSource.gameObject;
        if (lightSource.intensity == 0 && go.activeInHierarchy)
        {
            go.SetActive(false);
        }
        else if(lightSource.intensity > 0 && !go.activeInHierarchy)
        {
            go.SetActive(true);
        }
    }

    public void ResetTime()
    {
        time = startTime;
    }

    public void ForceUpdateLighting() //씬전환후 강제 초기화 메서드
    {
        UpdateLighting(sun, sunColor, sunIntensity);
        UpdateLighting(moon, moonColor, moonIntensity);

        RenderSettings.ambientIntensity = lightingIntensityMultiplier.Evaluate(time);
        RenderSettings.reflectionIntensity = reflectionIntensityMultiplier.Evaluate(time);
    }

    void RotateLights()
    {
        // 하루를 360도로 가정하고, 시간에 따라 회전
        float sunAngle = (time - 0.25f) * 360f;
        float moonAngle = (time - 0.75f) * 360f;

        sun.transform.rotation = Quaternion.Euler(sunAngle, 170f, 0f);
        moon.transform.rotation = Quaternion.Euler(moonAngle, 170f, 0f);
    }

}
