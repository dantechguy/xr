using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class LapsSlider : MonoBehaviour
{
    [SerializeField] private GeneralSettings generalSettings;
    [SerializeField] private TextMeshProUGUI label;

    private Slider slider_;

    private void Start()
    {
        slider_ = GetComponent<Slider>();
        slider_.value = generalSettings.laps;
    }

    public void ApplyLaps()
    {
        generalSettings.laps = (int)slider_.value;
        label.text = $"{generalSettings.laps.ToString()} {(generalSettings.laps > 1 ? "Laps" : "Lap")}";
    }
}