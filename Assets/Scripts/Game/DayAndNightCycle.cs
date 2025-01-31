using UnityEngine;

public class DayAndNightCycle : MonoBehaviour
{
    [SerializeField] private Light sun;
    [SerializeField] private Light moon;
    [SerializeField] private float dayDurationInSeconds = 480f;
    [SerializeField] private float timeOfDay = 0.4f;

    [Header("Lighting Settings")]
    [SerializeField] private Gradient sunColor;
    [SerializeField] private AnimationCurve sunIntensity;
    [SerializeField] private Gradient ambientColor;

    private void Update()
    {
        timeOfDay += Time.deltaTime / dayDurationInSeconds;
        if (timeOfDay >= 1f)
        {
            timeOfDay -= 1f;
        }

        // Rotate the sun
        float sunRotation = (timeOfDay * 360f) - 90f;
        sun.transform.rotation = Quaternion.Euler(sunRotation, -30f, 0f);

        // Rotate the moon (180 degrees offset from the sun)
        float moonRotation = sunRotation + 180f;
        moon.transform.rotation = Quaternion.Euler(moonRotation, -30f, 0f);

        UpdateLighting();
    }

    private void UpdateLighting()
    {
        // Enable/disable sun and moon based on time of day
        if (timeOfDay > 0.25f && timeOfDay < 0.75f)
        {
            sun.gameObject.SetActive(true);
            moon.gameObject.SetActive(false);
        }
        else
        {
            sun.gameObject.SetActive(false);
            moon.gameObject.SetActive(true);
        }

        // Smoothly transition sun and moon intensity
        float sunIntensityValue = sunIntensity.Evaluate(timeOfDay);

        sun.intensity = sunIntensityValue * 200000f;
        moon.intensity = sunIntensityValue * 25f;

        // Set sun/moon color and ambient color
        sun.color = sunColor.Evaluate(timeOfDay);
        RenderSettings.ambientLight = ambientColor.Evaluate(timeOfDay);
    }
}