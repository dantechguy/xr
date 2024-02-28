using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(Light))]
public class LightEstimator : MonoBehaviour
{
    [SerializeField] private ARCameraManager cameraManager;
    private Light light_;

    public float? brightness { get; private set; }
    public float? colorTemperature { get; private set; }
    public Color? colorCorrection { get; private set; }
    public Vector3? mainLightDirection { get; private set; }
    public Color? mainLightColor { get; private set; }
    public float? mainLightIntensityLumens { get; private set; }
    public SphericalHarmonicsL2? sphericalHarmonics { get; private set; }

    void Awake()
    {
        light_ = GetComponent<Light>();
    }

    void OnEnable()
    {
        if (cameraManager != null)
            cameraManager.frameReceived += FrameChanged;
    }

    void OnDisable()
    {
        if (cameraManager != null)
            cameraManager.frameReceived -= FrameChanged;
    }

    void FrameChanged(ARCameraFrameEventArgs args)
    {
        if (args.lightEstimation.averageBrightness.HasValue)
        {
            brightness = args.lightEstimation.averageBrightness.Value;
            light_.intensity = brightness.Value;
        }
        else
        {
            brightness = null;
        }

        if (args.lightEstimation.averageColorTemperature.HasValue)
        {
            colorTemperature = args.lightEstimation.averageColorTemperature.Value;
            light_.colorTemperature = colorTemperature.Value;
        }
        else
        {
            colorTemperature = null;
        }

        if (args.lightEstimation.colorCorrection.HasValue)
        {
            colorCorrection = args.lightEstimation.colorCorrection.Value;
            light_.color = colorCorrection.Value;
        }
        else
        {
            colorCorrection = null;
        }

        if (args.lightEstimation.mainLightDirection.HasValue)
        {
            mainLightDirection = args.lightEstimation.mainLightDirection;
            light_.transform.rotation = Quaternion.LookRotation(mainLightDirection.Value);
        }
        else
        {
            mainLightDirection = null;
        }

        if (args.lightEstimation.mainLightColor.HasValue)
        {
            mainLightColor = args.lightEstimation.mainLightColor;
            light_.color = mainLightColor.Value;
        }
        else
        {
            mainLightColor = null;
        }

        if (args.lightEstimation.mainLightIntensityLumens.HasValue)
        {
            mainLightIntensityLumens = args.lightEstimation.mainLightIntensityLumens;
            light_.intensity = args.lightEstimation.averageMainLightBrightness.Value;
        }
        else
        {
            mainLightIntensityLumens = null;
        }

        if (args.lightEstimation.ambientSphericalHarmonics.HasValue)
        {
            sphericalHarmonics = args.lightEstimation.ambientSphericalHarmonics;
            RenderSettings.ambientMode = AmbientMode.Skybox;
            RenderSettings.ambientProbe = sphericalHarmonics.Value;
        }
        else
        {
            sphericalHarmonics = null;
        }
    }
}