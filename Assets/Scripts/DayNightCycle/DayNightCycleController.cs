using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class DayNightCycleController : MonoBehaviour
{
    [Header("Time")]
    [Tooltip("Day lenth in minutes")]
    [SerializeField] private float m_TargetDayLength = 0.5f;
    public float TargetDayLength
    {
        get { return m_TargetDayLength; }
    }

    [SerializeField] private TMPro.TMP_Text m_TimeText;

    private float m_ElapsedTime;

    [SerializeField] [Range(0.0f, 1.0f)] private float m_TimeOfDay;
    public float TimeOfDay
    {
        get { return m_TimeOfDay; }
    }

    [SerializeField] private int m_DayNumber = 1;
    public int DayNumber
    {
        get { return m_DayNumber; }
    }

    [SerializeField] private int m_YearNumber = 1;
    public int YearNumber
    {
        get { return m_YearNumber; }
    }

    [SerializeField] private int m_YearLength = 365;
    public int YearLength
    {
        get { return m_YearLength; }
    }

    [SerializeField] private float m_TimeScale = 100.0f;

    public bool m_IsPaused = false;
    public bool m_IsUsing24HourClock = false;

    [SerializeField] private AnimationCurve m_TimeCurve;
    private float m_TimeCurveNormalisation;

    [Header("Sun Light")]
    [SerializeField] private Transform m_DailyRotation;
    [SerializeField] private Light m_Sun;

    private float m_Intensity;
    [SerializeField] private float m_SunBaseIntensity = 1.0f, m_SunVariation = 1.5f;
    [SerializeField] private Gradient m_SunColour;
    [SerializeField] private AnimationCurve m_AtmosphereCurve;

    [Header("Moon Light")]
    [SerializeField] private Light m_Moon;
    [SerializeField] private Gradient m_MoonColour;
    [SerializeField] private float m_MoonBaseIntensity;

    [Header("Sky Light")]
    [SerializeField] private Gradient m_SkyColour;
    [SerializeField] private Gradient m_HorizonColour;
    [SerializeField] private Color m_DayAmbientLight, m_NightAmbientLight;

    [Header("Seasonal Variables")]
    [SerializeField] private Transform m_SeasonalRotation;
    [SerializeField] [Range(-45.0f, 45.0f)] private float m_MaxSeasonalTilt;

    private void UpdateTimeScale()
    {
        m_TimeScale = 24 / (m_TargetDayLength / 60);
        m_TimeScale *= m_TimeCurve.Evaluate(m_ElapsedTime / (m_TargetDayLength * 60)); // Thomas: Gets value between 0 & 1
        m_TimeScale /= m_TimeCurveNormalisation;
    }

    private void NormaliseTimeCurve()
    {
        float stepSize = 0.01f;
        int numberOfSteps = Mathf.FloorToInt(1.0f / stepSize);
        float curveTotal = 0;

        for (int i = 0; i < numberOfSteps; i++)
        {
            curveTotal += m_TimeCurve.Evaluate(i * stepSize);
        }

        m_TimeCurveNormalisation = curveTotal / numberOfSteps;
    }

    private void UpdateTime()
    {
        m_TimeOfDay += Time.deltaTime * m_TimeScale / 86400; // Thomas: Seconds in a day
        m_ElapsedTime = m_TimeOfDay;

        if (m_TimeOfDay > 1)
        {
            m_ElapsedTime = 0;
            m_DayNumber++;
            m_TimeOfDay -= 1;

            if(m_DayNumber > m_YearLength)
            {
                m_YearNumber++;
                m_DayNumber = 1;
            }
        }
    }

    private void UpdateClock()
    {
        float time = m_ElapsedTime;
        float hour = Mathf.FloorToInt(time * 24);
        float minute = Mathf.FloorToInt(((time * 24) - hour) * 60);

        string hourString, minuteString;

        if (!m_IsUsing24HourClock && hour > 12)
            hour -= 12;

        hourString = hour < 10 ? "0" + hour.ToString() : hour.ToString();
        minuteString = minute < 10 ? "0" + minute.ToString() : minute.ToString();

        if(m_IsUsing24HourClock)
            m_TimeText.text = "Time: " + hourString + ":" + minuteString;
        else if (time > 0.5f)
            m_TimeText.text = "Time: " + hourString + ":" + minuteString + "pm";
        else
            m_TimeText.text = "Time: " + hourString + ":" + minuteString + "am";
    }

    private void UpdateSunRotation()
    {
        float sunAngle = m_TimeOfDay * 360.0f;
        m_DailyRotation.transform.localRotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, -sunAngle));

        float seasonalAngle = -m_MaxSeasonalTilt * Mathf.Sin((float)m_DayNumber / (float)m_YearLength * Mathf.PI);
        m_SeasonalRotation.localRotation = Quaternion.Euler(new Vector3(seasonalAngle - 180.0f, 0.0f, 0.0f));
    }

    private void UpdateSunIntensity()
    {
        m_Intensity = Vector3.Dot(m_Sun.transform.forward, Vector3.down);
        m_Intensity = Mathf.Clamp01(m_Intensity);

        m_Sun.intensity = m_Intensity * m_SunVariation * m_SunBaseIntensity;

        float dotProduct = Vector3.Dot(m_Sun.transform.forward, Vector3.down);
        RenderSettings.ambientLight = Color.Lerp(m_NightAmbientLight, m_DayAmbientLight, m_TimeCurve.Evaluate(dotProduct));

        RenderSettings.skybox.SetFloat("_AtmosphereThickness", Mathf.Lerp(0.05f, 0.5f, m_AtmosphereCurve.Evaluate(m_TimeOfDay)));
        DynamicGI.UpdateEnvironment();
    }

    private void UpdateSunColour()
    {
        m_Sun.color = m_SunColour.Evaluate(m_Intensity);
    }

    private void UpdateMoon()
    {
        m_Moon.color = m_MoonColour.Evaluate(1 - m_Intensity);
        m_Moon.intensity = (1 - m_Intensity) * m_MoonBaseIntensity + 0.01f;
    }

    private void UpdateSkyColour()
    {
        RenderSettings.skybox.SetColor("_SkyTint", m_SkyColour.Evaluate(m_Intensity));
        RenderSettings.skybox.SetColor("_GroundColor", m_HorizonColour.Evaluate(m_Intensity));
    }

    public void SetTime(TimeObject _timeOfDay)
    {
        m_TimeOfDay = (_timeOfDay.hours * 60 + _timeOfDay.minutes) * 60 / 86400;
    }

    private void Start()
    {
        NormaliseTimeCurve();
    }

    private void Update()
    {
        if (m_IsPaused)
            return;

        UpdateTimeScale();
        UpdateTime();
        UpdateClock();

        UpdateSunRotation();
        UpdateSunIntensity();
        UpdateSunColour();

        UpdateMoon();

        UpdateSkyColour();
    }
}
