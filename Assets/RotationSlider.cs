using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class RotationSlider : MonoBehaviour
{
    [SerializeField] private SelectedTransformer transformer_;
    private Slider slider_;

    private void OnEnable()
    {
        transformer_.GetSelectionInfo().onSelected += OnSelected;
    }

    private void OnSelected()
    {
        slider_.value = transformer_.GetSelectionInfo().GetSelected()?.GetComponent<ARSpawnedTransformable>()
            ?.GetRotationAngle() ?? 0;
    }

    void Start()
    {
        slider_ = GetComponent<Slider>();
    }

    public void ApplyRotation()
    {
        transformer_.ApplyRotation(slider_.value);
    }
}