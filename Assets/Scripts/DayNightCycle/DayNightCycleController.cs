using System;
using UnityEngine;

public class DayNightCycleController : MonoBehaviour
{
    [SerializeField]
    private float timeMultiplier, startHour;

    private DateTime currentTime;

    [SerializeField]
    private Light sun, moon;

    [SerializeField]
    private float sunriseHour, sunsetHour;

    [SerializeField]
    private TimeSpan sunriseTime, sunsetTime;

    [SerializeField]
    private Color dayAmbientLight, nightAmbientLight;

    [SerializeField]
    private AnimationCurve lightChangeCurve, atmosphereChangeCurve;

    [SerializeField]
    private float maxSunlightIntensity, maxMoonlightIntensity;

    void Start()
    {
        currentTime = DateTime.Now.Date + TimeSpan.FromHours(startHour);
        sunriseTime = TimeSpan.FromHours(sunriseHour);
        sunsetTime = TimeSpan.FromHours(sunsetHour);
    }

    void Update()
    {
        UpdateTimeOfDay();
        RotateSun();
        UpdateLightSettings();
    }

    void UpdateTimeOfDay()
    {
        currentTime = currentTime.AddSeconds(Time.deltaTime * timeMultiplier);
    }

    TimeSpan CalculateTimeDifference(TimeSpan fromTime, TimeSpan toTime)
    {
        TimeSpan diff = toTime - fromTime;

        return diff.TotalSeconds < 0 ? diff += TimeSpan.FromHours(24) : diff;
    }

    void RotateSun()
    {
        float sunlightRotation;

        if (currentTime.TimeOfDay > sunriseTime && currentTime.TimeOfDay < sunsetTime)
        {
            TimeSpan sunriseToSunsetDuration = CalculateTimeDifference(sunriseTime, sunsetTime);
            TimeSpan timeSinceSunrise = CalculateTimeDifference(sunriseTime, currentTime.TimeOfDay);

            double percentage = timeSinceSunrise.TotalMinutes / sunriseToSunsetDuration.TotalMinutes;

            sunlightRotation = Mathf.Lerp(0, 180, (float)percentage);
        }
        else
        {
            TimeSpan sunsetToSunriseDuration = CalculateTimeDifference(sunsetTime, sunriseTime);
            TimeSpan timeSinceSunset = CalculateTimeDifference(sunsetTime, currentTime.TimeOfDay);

            double percentage = timeSinceSunset.TotalMinutes / sunsetToSunriseDuration.TotalMinutes;

            sunlightRotation = Mathf.Lerp(180, 360, (float)percentage);
        }

        sun.transform.rotation = Quaternion.AngleAxis(sunlightRotation, Vector3.right); // Thomas: Vector3.right = rotate around X axis
        moon.transform.rotation = Quaternion.AngleAxis(sunlightRotation - 180, Vector3.right);
    }

    void UpdateLightSettings()
    {
        float dotProduct = Vector3.Dot(sun.transform.forward, Vector3.down);
        sun.intensity = Mathf.Lerp(0, maxSunlightIntensity, lightChangeCurve.Evaluate(dotProduct));
        moon.intensity = Mathf.Lerp(maxMoonlightIntensity, 0, lightChangeCurve.Evaluate(dotProduct));
        RenderSettings.ambientLight = Color.Lerp(nightAmbientLight, dayAmbientLight, lightChangeCurve.Evaluate(dotProduct));

        RenderSettings.skybox.SetFloat("_AtmosphereThickness", Mathf.Lerp(0.05f, 0.5f, atmosphereChangeCurve.Evaluate(dotProduct)));
        DynamicGI.UpdateEnvironment();
    }
}
