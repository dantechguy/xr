using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(Slider))]
public class PlaneTransparencySlider : MonoBehaviour
{
    [SerializeField] private GeneralSettings generalSettings;

    private Slider slider_;

    private void Start()
    {
        slider_ = GetComponent<Slider>();
        slider_.value = generalSettings.planeTransparency;
    }

    private void OnEnable()
    {
        if (slider_ != null)
            slider_.value = generalSettings.planeTransparency;
    }

    public void UpdateTransparencyToAllPlanes()
    {
        generalSettings.planeTransparency = slider_.value;
    }
}