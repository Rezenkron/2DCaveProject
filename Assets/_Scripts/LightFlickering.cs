using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Light2D))]
public class LightFlickering : MonoBehaviour
{
    private Light2D _light;

    [Header("Flicker Settings")]
    [SerializeField] private float minIntensity = 0.8f;
    [SerializeField] private float maxIntensity = 1.2f;
    [SerializeField] private float speed = 1f;

    private float noiseTime = 0f;

    private void Start()
    {
        _light = GetComponent<Light2D>();
    }

    private void Update()
    {
        noiseTime += Time.deltaTime * speed;
        float noise = Mathf.PerlinNoise(noiseTime, 0f);
        _light.intensity = Mathf.Lerp(minIntensity, maxIntensity, noise);
    }
}
